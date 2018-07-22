using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Entidades;

namespace Servicio_Principal
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class Servicio : IServicio
    {
        /// <summary>
        /// Obtenemos el callback correspondiente al cliente que esta interactuando con el servicio en este momento
        /// </summary>
        public IServicioCallback CallbackActual
        {
            get
            {
                return OperationContext.Current.GetCallbackChannel<IServicioCallback>();
            }
        }

        /// <summary>
        /// Procesa una solicitud de conexión al servicio
        /// </summary>
        /// <param name="oper"></param>
        /// <returns></returns>
        public bool Conectar(Operador oper)
        {
            if (oper.UserName != null)
            {
                SQL.Operador connOperador = new SQL.Operador();
                if (connOperador.ValidarIngreso(oper))
                {
                    CallbackActual.Mensaje(new Mensaje() { Contenido = "Se ha autorizado el acceso de manera correcta" });
                    return true;
                }
                return false;
            }
            else
            {
                // Si no cuenta con nombre de usuario se rechaza la conexión
                Console.WriteLine("El servidor ha rechazado la conexión debido a que no se ha comunicado el nombre de usuario. ");
                // Devolvemos un mensaje al Callback informando que se rechazó la conexión
                CallbackActual.Mensaje(new Mensaje()
                {
                    Contenido = "El servidor ha rechazado la conexión."
                });
                return false;
            }            
        }
    }
}
