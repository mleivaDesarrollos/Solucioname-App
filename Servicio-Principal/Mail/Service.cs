using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Exchange.WebServices.Data;

namespace Servicio_Principal.Mail
{
    public class Service
    {
        /// <summary>
        /// Mail service to get information 
        /// </summary>
        ExchangeService mailServiceConnection;

        /// <summary>
        /// location of asked asuntos
        /// </summary>
        FolderId asuntoFolder;

        /// <summary>
        /// Name of folder located over inbox
        /// </summary>
        string asuntoFolderName = "Asuntos";

        /// <summary>
        /// Filter used in subject for match asuntos
        /// </summary>
        string subjectAsuntoFiltering = "Portal Solucioname: Asunto Nº ";

        /// <summary>
        /// Start Criteria for cutting asunto number
        /// </summary>
        string startSubstringCut = "Asunto Nº ";

        /// <summary>
        /// End Criteria for cutting asunto number
        /// </summary>
        string endSubstringCut = "  Categoría";

        /// <summary>
        /// On current configuration, the service takes credentials from user executing
        /// main service
        /// </summary>
        public Service()
        {
            // Instanciate a new objecto of mail service
            mailServiceConnection = new ExchangeService(ExchangeVersion.Exchange2013);
            // Configure credentials use to default
            mailServiceConnection.UseDefaultCredentials = true;
            // Setup autodiscover url
            mailServiceConnection.AutodiscoverUrl("maleiva@servexternos.isban-santander.com.ar", RedirectUrlValidationCallback);
            // Save folderId of asuntos in a proper variable
            asuntoFolder = getFolderOfSavedAsuntos();
        }
        
        /// <summary>
        /// Check on mail folder last received asuntos
        /// </summary>
        /// <param name="prmLastDateTimeChecked">Last date registered of check to filter folder</param>
        /// <returns></returns>
        public List<Entidades.Asunto> GetLastAsuntosAdded(DateTime prmLastDateTimeChecked)
        {
            // Generate a new asunto list to return
            List<Entidades.Asunto> lstAsuntoToProcess = new List<Entidades.Asunto>();
            // Get filtering properties for search on inbox
            SearchFilter filterCriteria = getFilterForAsuntosSearch(prmLastDateTimeChecked.AddSeconds(10));
            // Get view with specify properties for search
            ItemView viewContainer = getResultViewForAsuntoList();
            // Generate a collection for matching items
            FindItemsResults<Item> resultOfSearch = mailServiceConnection.FindItems(asuntoFolder, filterCriteria, viewContainer);
            if(resultOfSearch.TotalCount > 0) {
                // Generate a new ServiceResponseCollection with items saved
                ServiceResponseCollection<GetItemResponse> itemWithBodyAndDateReceived = mailServiceConnection.BindToItems(resultOfSearch.Select(item => item.Id), new PropertySet(BasePropertySet.FirstClassProperties, ItemSchema.DateTimeReceived));
                // Iterates over all results finded
                foreach (var item in itemWithBodyAndDateReceived.Select(item => item.Item).ToArray()) {
                    // Save temporaly in a variable subject result
                    string subjectWithAsunto = item.Subject;
                    // Get Start point for cutting string resullt
                    int intStartCutString = subjectWithAsunto.IndexOf(startSubstringCut) + startSubstringCut.Length;
                    int intEndCutString = subjectWithAsunto.LastIndexOf(endSubstringCut);
                    // With end and start can obtain asunto number
                    string asuntoNumber = subjectWithAsunto.Substring(intStartCutString, (intEndCutString - intStartCutString));
                    string shortDescription = getShortDescriptionByFiltering(item.Body);
                    // Generate a new entidades of asunto and add to list
                    lstAsuntoToProcess.Add(new Entidades.Asunto() { Numero = asuntoNumber, DescripcionBreve = shortDescription, LoadedOnSolucionameDate = item.DateTimeReceived });
                }
            }            
            // Return processed list 
            return lstAsuntoToProcess;
        }

        /// <summary>
        /// Method disposed for autodiscover response
        /// </summary>
        /// <param name="redirUrl"></param>
        /// <returns></returns>
        private bool RedirectUrlValidationCallback(string redirUrl)
        {
            bool result = false;
            Uri redirectionUri = new Uri(redirUrl);
            if (redirectionUri.Scheme == "https") {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Check on service for folderId of asuntos
        /// </summary>
        /// <returns></returns>
        private FolderId getFolderOfSavedAsuntos()
        {
            // Gets root folder of Inbox
            Folder rootFolder = Folder.Bind(mailServiceConnection, WellKnownFolderName.Inbox);
            // Iterates over all folders getted
            foreach (var folder in rootFolder.FindFolders(new FolderView(5))) {
                if(folder.DisplayName == asuntoFolderName) {
                    // If the folder is match, return the id
                    return folder.Id;
                }
            }
            throw new Exception("error getting folder of asuntos over inbox.");
        }

        /// <summary>
        /// Get a filter object for search mails purposes
        /// </summary>
        /// <param name="prmTimeForFiltering"></param>
        /// <returns></returns>
        private SearchFilter getFilterForAsuntosSearch(DateTime prmTimeForFiltering)
        {
            // Generate a new filter to return on process
            SearchFilter finalFilter;
            // Generate a list of search filter to generate a criteria for looking asuntos
            List<SearchFilter> filterList = new List<SearchFilter>();
            // Add filtering for mails
            filterList.Add(new SearchFilter.ContainsSubstring(ItemSchema.Subject, subjectAsuntoFiltering));
            filterList.Add(new SearchFilter.IsGreaterThan(ItemSchema.DateTimeReceived, prmTimeForFiltering));
            // Instanciate search filter with generated criteria
            finalFilter = new SearchFilter.SearchFilterCollection(LogicalOperator.And, filterList.ToArray());
            // Return processed filter
            return finalFilter;
        }

        private ItemView getResultViewForAsuntoList()
        {
            // Generate a new ItemView for processing
            ItemView viewForFiltering = new ItemView(100);
            // Establish order for filtering
            viewForFiltering.OrderBy.Add(ItemSchema.DateTimeReceived, SortDirection.Ascending);
            // Establish property to show in the view
            viewForFiltering.PropertySet = new PropertySet(BasePropertySet.IdOnly, ItemSchema.Subject, ItemSchema.DateTimeReceived);
            // Establish configuration for search criteria
            viewForFiltering.Traversal = ItemTraversal.Shallow;
            // Return processed view 
            return viewForFiltering;
        }

        private string getShortDescriptionByFiltering(string strBody)
        {
            // Prepare string for return
            string shortDescription = "Sin descripción";
            int filterIndex = 0;
            // Iterates over all loaded filters
            do {
                if (strBody.Contains(SQL.Asunto.lstFiltersForMails[filterIndex].Phrase)) {
                    shortDescription = SQL.Asunto.lstFiltersForMails[filterIndex].ShortDescription;
                    break;
                }
                filterIndex++;
            } while (filterIndex != SQL.Asunto.lstFiltersForMails.Count);

            // Return processed string
            return shortDescription;
        }

    }
}
