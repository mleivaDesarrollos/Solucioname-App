﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class GrupoResolutor
    {
        public int Id
        {
            get; set;
        }

        public String Descripcion
        {
            get; set;
        }

        public ActuacionTipo Tipo
        {
            get; set;
        }
    }
}
