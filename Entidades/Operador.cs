﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Entidades
{
    [DataContract]
    public class Operador
    {

        [DataMember]
        public String UserName
        {
            get; set;
        }

        [DataMember]
        public String Password
        {
            get; set;
        }

        [DataMember]
        public String Nombre
        {
            get; set;
        }

        [DataMember]
        public String Apellido
        {
            get; set;
        }

        [DataMember]
        public String DNI
        {
            get; set;
        }
    }
}
