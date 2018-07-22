using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class TipoEstado
    {
        public enum SolicitaActuacion
        {
            No,
            Si,
            SiPredeterminado,
            Obligatoria
        }

        public int Id
        {
            get; set;
        }

        public String Descripcion
        {
            get; set;
        }

        public Boolean RequiereDetalle
        {
            get; set;
        }

        public Boolean InicioHabilitado
        {
            get; set;
        }

        public Boolean Unico
        {
            get; set;
        }

        public Gestion Gestion
        {
            get; set;
        }

        public Boolean CorteCiclo
        {
            get; set;
        }

        public SolicitaActuacion RequiereActuacion
        {
            get; set;
        }

        public List<TipoEstado> RelacionesPermitidas
        {
            get; set;
        }
    }
}
