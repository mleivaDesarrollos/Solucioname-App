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
            Console.WriteLine("Conexion establecida. Modo administrador mediante comando:");
            while (true)
            {
                string comando = Console.ReadLine();
                using (Command cmd = Command.Get(comando))
                {
                    if (cmd != null)
                    {
                        foreach (var ExecutionMessage in Entidades.Service.Commands.Execution.CommandExecution.lstCmdExec)
                        {

                        }
                    }
                }
            }
            /*
            // Consola de prueba - Conexión a servicio Solucioname
            Console.WriteLine("Consola de verificación de servicio : Solucioname Gestion. aguarde sorete.");
            // Generamos una nueva entidad vinculada con el Callback de cliente
            InteraccionServicio intServicio = new InteraccionServicio();
            try
            {
                // Solicitamos conexión nueva
                if(intServicio.Conectarse())
                {
                    
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
            */
        }
    }
}
