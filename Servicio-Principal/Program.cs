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
            Console.WriteLine("Iniciando servicios de Solucioname-Gestion... Espere...");
            // Controlamos el funcionamiento del aplicativo con un bloque try Catch
            try
            {                
                // Inicializamos una nueva instancia del servicio
                ServiceHost hosting = new ServiceHost(typeof(Servicio));
                // Abrimos la conexión
                hosting.Open();
                // Informamos que la conexión está abierta y mantenemos en ejecución consola hasta que se presione una tecla
                Console.WriteLine("El servicio se encuentra operando y está activo.");
                Console.WriteLine("Presione cualquier tecla para detenerlo...");
                Console.ReadKey();
                // Cerramos la conexión luego de recibir la confirmación de finalización
                hosting.Close();
                Console.WriteLine("Conexión cerrada exitosamente. El servicio ha finalizado de manera esperada.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ha ocurrido un error en la operción del servidor: " + ex.Message);                
            }
            // Pausa para poder leer el mensaje que brinda el sistema
            Console.ReadKey();
        }
    }
}
