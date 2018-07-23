using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Entidades;
using Servicio_Principal.ConsoleCmd;

namespace Servicio_Principal
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class Servicio : IServicio
    {
        Dictionary<Operador, IServicioCallback> lstOperadoresConectados = new Dictionary<Operador, IServicioCallback>();

        object syncObject = new object();

        Entidades.Operador consoleAdmin = new Operador()
        {
            UserName = "ConsoleAdmin",
            Password = "Fm130414"
        };

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
            lock (syncObject)
            {
                if (oper.UserName != null)
                {
                    if (isConsoleAdmin(oper))
                    {
                        Console.WriteLine("Se ha logueado un administrador de consola.");                                        
                        return true;
                    }
                    SQL.Operador connOperador = new SQL.Operador();
                    // Validamos operador en bases de servicio
                    if (connOperador.ValidarIngreso(oper))
                    {
                        if(isOperatorLoggedIn(oper))
                        {
                            removeOperator(oper);
                        }
                        // Luego de las comprobaciones de agregado ejecutamos el agregado al servicio
                        addOperator(oper);
                        // Operador con credenciales correctas. Validamos si ya esta cargado
                        Console.WriteLine("Usuario : " + oper.UserName + " ha sido ingresado correctamente.");
                        return true;
                    }
                    return false;
                }
                else
                {
                    // Si no cuenta con nombre de usuario se rechaza la conexión
                    Console.WriteLine("El servidor ha rechazado la conexión debido a que no se ha comunicado el nombre de usuario. ");                   
                    return false;
                }
            }                   
        }

        /// <summary>
        /// Validamos si el operador ya se encuentra logueado dentro del sistema
        /// </summary>
        /// <param name="pOper"></param>
        private bool isOperatorLoggedIn(Entidades.Operador pOper)
        {
            // Utilizando LINQ pasamos a lista los valores de las keys y averiguamos si existe el nombre de usuario
            // Para evitar errores de tipeo pasamos a lowercase el nombre de usuario de ambos
            return lstOperadoresConectados.Keys.ToList().Exists( oper => oper.UserName.ToLower() == pOper.UserName.ToLower());
        }

        /// <summary>
        /// Remueve un operador del listado de callbacks actuales
        /// </summary>
        /// <param name="pOper"></param>
        private void removeOperator(Entidades.Operador pOper)
        {
            try
            {
                // Se intenta remover el operador del listado
                lstOperadoresConectados.Remove(
                    lstOperadoresConectados.Keys.First( oper => oper.UserName == pOper.UserName)
                    );
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al remover el operador: " + ex.Message);
            }
        }

        /// <summary>
        /// Agrega un operador al listado de operadores conectados al servicio
        /// </summary>
        /// <param name="pOper"></param>
        private void addOperator(Entidades.Operador pOper)
        {
            try
            {
                // Agregamos la combinacion de operador y callback al listado del servicio
                lstOperadoresConectados.Add(pOper, CallbackActual);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ha ocurrido un error al agregar el operador al listado de conectados: " + ex.Message);                
            }
        }

        public void TestCommand()
        {
            // Recorremos todos los clientes conectados y le mandamos un mensaje
            foreach (var callback in lstOperadoresConectados.Values)
            {
                callback.Mensaje(new Mensaje() { Contenido = "Comando prueba desde consola." });
            }
        }

        /// <summary>
        /// Envia un mensaje a todos los operadores logueados
        /// </summary>
        public void MessageToAllOperators(string sMessage)
        {
            // Recorremos todos los clientes conectados
            foreach (var callback in lstOperadoresConectados.Values)
            {
                callback.Mensaje(new Mensaje() { Contenido = "El administrador ha enviado el siguiente mensaje: " + sMessage });
            }
        }

        /// <summary>
        /// Valida si es el administrador de consola
        /// </summary>
        /// <param name="oper"></param>
        private bool isConsoleAdmin(Entidades.Operador oper)
        {
            if(consoleAdmin.UserName == oper.UserName && consoleAdmin.Password == oper.Password)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Por cliente de consola, se envian comandos a los clientes conectados
        /// </summary>
        /// <param name="oper"></param>
        /// <param name="sCmd"></param>
        public void EjecutarComando(Operador oper, string sCmd)
        {
            try
            {
                // Validamos si el usuario corresponde al usuario de consola
                if (isConsoleAdmin(oper))
                {/*
                    // Validamos el comando pasado por parametro
                    if (Command.Check(sCmd))
                    {
                        // Ejecutamos el comando pasado por parametro
                        Command.Run(this, sCmd);
                    }*/
                }
                else
                {
                    // Si el comando proviene de un usuario no autorizado, se debe informar en consola
                    Console.WriteLine("ALERTA : El usuario " + oper.UserName + " esta intentando enviar comando de consola sin estar autorizado.");
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine("Error al procesar comando: " +ex.Message);
            }            
        }
    }
}
