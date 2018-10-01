using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicio_Principal
{
    public partial class Servicio : IServicio
    {
        private static string _AsuntosServiceCheckingClass = "AsuntoServiceCheckingTask";

        private void ConfigureAsuntoServiceCheckTimer() {
            // Instance a new timer object
            tmrAsuntoServiceCheck = new System.Timers.Timer();
            // Loads asuntos from database
            lstAsuntoFromServiceUnassigned = SQL.Asunto.GetAsuntosWithoutAssignation();
            SQL.Asunto.LoadFiltersPhraseForMails();
            Log.Info(_AsuntosServiceCheckingClass, "Loaded asuntos without assignation...");
            // Establish event related to elapsed time
            tmrAsuntoServiceCheck.Elapsed += TmrAsuntoServiceCheck_Elapsed;
            // Configure time for recurrent timer
            tmrAsuntoServiceCheck.Interval = Config.ASUNTO_SERVICE_CHECK_CICLE_TIME;
        }

        private void StartAsuntoCheckTimer(bool notify = false) {
            if(tmrAsuntoServiceCheck != null) {
                if (!tmrAsuntoServiceCheck.Enabled) {
                    tmrAsuntoServiceCheck.Enabled = true;
                    if(notify) Log.Info(_AsuntosServiceCheckingClass, "service started...");
                }
            }
        }

        private void StopAsuntoCheckTimer() {
            if(tmrAsuntoServiceCheck != null) {
                if (tmrAsuntoServiceCheck.Enabled) {
                    tmrAsuntoServiceCheck.Enabled = false;
                }
            }
        }
        
        private void TmrAsuntoServiceCheck_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Stop Timer
            StopAsuntoCheckTimer();
            lock (syncObject) {
                // if update is true refresh the list of asuntos unassigned
                if (SQL.Asunto.CheckNewAsuntosOnSolucioname()) {
                    lstAsuntoFromServiceUnassigned = SQL.Asunto.GetAsuntosWithoutAssignation();
                    Log.Info(_AsuntosServiceCheckingClass, "Updated unassigned asuntos. Total: " + lstAsuntoFromServiceUnassigned.Count);
                }
            }
            // Start timer again
            StartAsuntoCheckTimer();
        }

    }
}
