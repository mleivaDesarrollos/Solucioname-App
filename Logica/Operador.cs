using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica
{
    public class Operador
    {
        // Disponemos de un objeto datos operador
        Datos.Operador datOper  = new Datos.Operador();

        /// <summary>
        /// Valida los datos pasados por parametros sobre la base de datos
        /// </summary>
        /// <param name="pUser"></param>
        /// <param name="pPass"></param>
        /// <returns></returns>
        public Entidades.Operador Ingresar(Entidades.Operador pOperador)
        {
            try
            {
                return datOper.Ingresar(pOperador);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Procesa una solicitud de conexión. Si el proceso es correcto la entidad operador viene completa
        /// </summary>
        /// <param name="pOperator"></param>
        /// <returns></returns>
        public async Task<Entidades.Operador> ConnectBackoffice(Entidades.Operador pOperator, Entidades.Service.Interface.IServicioCallback paramCallback)
        {
            try
            {
                return await datOper.LogOnServiceBackoffice(pOperator, paramCallback);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Sent a connection request to service.
        /// </summary>
        /// <param name="pOperator"></param>
        /// <param name="paramCallback"></param>
        /// <returns>true if the connection is established, false if rejected</returns>
        public async Task<bool> ConnectOperatorToService(Entidades.Operador pOperator, Entidades.Service.Interface.IServicioCallback paramCallback)
        {
            try
            {
                // Calls Datos Project for get the results from service
                return await datOper.ConnectOperatorToService(pOperator, paramCallback);
            }
            catch (Exception ex)
            {
                // Bubbles the exception to the next level
                throw ex;
            }
        }

        /// <summary>
        /// Sent a signal for service disconnection
        /// </summary>
        /// <param name="pOperator">Operator to disconnect</param>
        /// <returns></returns>
        public async Task DisconnectFromService(Entidades.Operador pOperator)
        {
            try {
                // Sent to data information of disconnected operator
                await datOper.DisconnectFromService(pOperator);
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        /// <summary>
        /// Sent a request to service for changing current status on the service
        /// </summary>
        /// <param name="pOperator"></param>
        /// <param name="newStatus"></param>
        /// <returns></returns>
        public async Task ChangeCurrentStatus(Entidades.Operador pOperator, Entidades.AvailabiltyStatus newStatus)
        {
            try {
                // Sents to data the request to change status
                await datOper.ChangeCurrentStatus(pOperator, newStatus);
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        /// <summary>
        /// Call service methods to get a list with service logged operator
        /// </summary>
        /// <returns></returns>
        public async Task<List<Entidades.Operador>> GetFullOperatorList()
        {
            try
            {
                return await datOper.GetFullOperatorList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
