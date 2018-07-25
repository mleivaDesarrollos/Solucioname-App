using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades.Service;

namespace ConsoleHostTesting
{
    class Program
    {
        public static Entidades.Operador consoleAdm = new Entidades.Operador()
        {
            UserName = "ConsoleAdmin",
            Password = "Fm130414"
        };

        static void Main(string[] args)
        {            
            // Consola de prueba - Conexión a servicio Solucioname
            Console.WriteLine("Consola de verificación de servicio : Solucioname Gestion. aguarde sorete.");
            // Generamos una nueva entidad vinculada con el Callback de cliente
            InteraccionServicio intServicio = new InteraccionServicio();
            try
            {
                // Solicitamos conexión nueva
                if(intServicio.Conectarse())
                {
                    Console.WriteLine("Connection completed. Send a command:");
                    while (true)
                    {
                        try
                        {
                            // Esperamos el comando
                            string strNewCommand = Console.ReadLine();
                            // Obtenemos el comando desde entidades
                            // Por restricciones de SOAP, no es posible enviar el objeto heredado al servicio de manera nativa
                            // Por tal motivo este proceso es utilizado únicamente como un medio de validación
                            // que permita comprobar si el comando esta correctamente construido
                            // El servicio se encargará de construir la entidad con sus herencias utilizando
                            // las mismas clases que ejecutan las validaciones
                            Command newCommand = Command.Get(strNewCommand);
                            // Ejecutamos el comando
                            intServicio.EnviarComando(strNewCommand);                            
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }                        
                    }
                }
            }
            catch (Exception ex)
            {
                // Procesamos los errores que puedan producirse
                Console.WriteLine("Se ha producido un error al intentar conectar: " + ex.Message);
                Console.ReadKey();
            }
            // Informamos la finalización de la tarea
            Console.WriteLine("Se ha finalizado las tareas programadas");
            Console.ReadKey();            
        }
    }
}
