using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace Servicio_Principal
{
    public static class Program
    {
        /// <summary>
        /// Punto de partida para el inicio del servidor
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Mostramos un mensaje donde se evidencie el inicio del aplicativo
            Log.Info("MainService", "Starting service operation...");
            // Controlamos el funcionamiento del aplicativo con un bloque try Catch
            try
            {                
                // Inicializamos una nueva instancia del servicio
                ServiceHost hosting = new ServiceHost(typeof(Servicio));
                // Abrimos la conexión
                hosting.Open();
                // Informamos que la conexión está abierta y mantenemos en ejecución consola hasta que se presione una tecla
                Log.Info("MainService", "Service started normally.");
                Console.WriteLine("Press any key to stop service.");
                Console.ReadKey();
                // Cerramos la conexión luego de recibir la confirmación de finalización
                hosting.Close();
                Log.Info("MainService", "Service stopped normally.");
            }
            catch (Exception ex)
            {
                Log.Error("MainService", "details: " + ex.Message);
            }
            // Pausa para poder leer el mensaje que brinda el sistema
            Console.ReadKey();
        }
    }
}
