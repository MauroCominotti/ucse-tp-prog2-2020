﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaDeNegocios
{
    public class LogicaSala
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool Eliminado { get; set; }
        public int IdInstitucion { get; set; }
    }
}
