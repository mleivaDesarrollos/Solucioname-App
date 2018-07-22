using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleHostTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            // Consola de prueba - Conexión a servicio Solucioname
            Console.WriteLine("Consola de verificación de servicio : Solucioname Gestion. aguarde.");
            // Generamos una nueva entidad vinculada con el Callback de cliente
            InteraccionServicio intServicio = new InteraccionServicio();
            try
            {
                // Solicitamos conexión nueva
                intServicio.Conectarse();
                Console.ReadKey();
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
