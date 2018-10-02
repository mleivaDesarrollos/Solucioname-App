using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicio_Principal
{
    public class AsuntoDeliverList
    {
        private List<Entidades.Asunto> _listOfAsuntoToDeliver;

        private readonly ReadOnlyCollection<Entidades.Asunto> _readonlyListOfAsuntoToDeliver;

        public int Count {
            get {
                return _listOfAsuntoToDeliver.Count;
            }
        }

        public bool IsEmpty {
            get {
                return Count == 0;
            }
        }

        public AsuntoDeliverList(List<Entidades.Asunto> initialListToLoad)
        {
            _listOfAsuntoToDeliver = initialListToLoad;
            _readonlyListOfAsuntoToDeliver = new ReadOnlyCollection<Entidades.Asunto>(_listOfAsuntoToDeliver); 
        }

        /// <summary>
        /// Dispose a readonly list for read purposes
        /// </summary>
        public IEnumerable<Entidades.Asunto> Get {
            get {
                return _readonlyListOfAsuntoToDeliver;
            }
        }

        /// <summary>
        /// Add a asunto to the list
        /// </summary>
        /// <param name="asuntoToAdd"></param>
        public void Add(Entidades.Asunto asuntoToAdd)
        {
            bool isAsuntoAlreadyLoaded = _listOfAsuntoToDeliver.Exists(asunto => asunto.Equals(asuntoToAdd));
            if (isAsuntoAlreadyLoaded) throw new Exception(Error.ASUNTO_ALREADY_IN_DELIVER_LIST);
            // Add the asunto to the list
            _listOfAsuntoToDeliver.Add(asuntoToAdd);
        }
        
        /// <summary>
        /// Add a batch of asuntos to the list
        /// </summary>
        /// <param name="lstOfAsuntosToAdd"></param>
        public void Add(List<Entidades.Asunto> lstAsuntosToAdd)
        {
            bool isAsuntoAlreadyLoaded = _listOfAsuntoToDeliver.Exists(asuntoInList =>
                lstAsuntosToAdd.Exists(asuntoToAdd => asuntoInList.Equals(asuntoToAdd))
            );
            if (isAsuntoAlreadyLoaded) throw new Exception(Error.ASUNTO_ALREADY_IN_DELIVER_LIST);
            // Add the asunto to the list
            lstAsuntosToAdd.ForEach(asunto => _listOfAsuntoToDeliver.Add(asunto));
        }
        
        /// <summary>
        /// Asunto to remove from the list
        /// </summary>
        /// <param name="asuntoToRemove"></param>
        public void Remove(Entidades.Asunto asuntoToRemove)
        {
            // Removes the asunto from the list
            _listOfAsuntoToDeliver.RemoveAll(asuntoInList => asuntoToRemove.Numero == asuntoInList.Numero);
        }

        /// <summary>
        /// List of asunto to remove from pending deliver list
        /// </summary>
        /// <param name="lstAsuntosToRemove"></param>
        public void Remove(List<Entidades.Asunto> lstAsuntosToRemove)
        {
            // Removes the asunto from the list
            foreach (var asuntoToRemove in lstAsuntosToRemove) {
                // Removes all matches from the list
                _listOfAsuntoToDeliver.RemoveAll(asuntoInList => asuntoInList.Numero == asuntoToRemove.Numero);
            }
        }

    }
}
