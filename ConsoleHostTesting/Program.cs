using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleHostTesting
{
    class Program
    {
        public static ConsoleHostTesting.ServicioSolucioname.Operador consoleAdm = new ServicioSolucioname.Operador()
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
                    Console.WriteLine("Conexion establecida. Modo administrador mediante comando:");
                    while (true)
                    {
                        string comando = Console.ReadLine();
                        intServicio.EnviarComandoServidor(comando);
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
