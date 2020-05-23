using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaDeNegocios
{
    public class LogicaPadre : LogicaUsuario
    {
        public LogicaHijo[] Hijos { get; set; }
    }
}
