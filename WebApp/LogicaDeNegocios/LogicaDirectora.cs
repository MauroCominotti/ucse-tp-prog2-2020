using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaDeNegocios
{
    public class LogicaDirectora : LogicaUsuario
    {
        public LogicaInstitucion Institucion { get; set; }
        public string Cargo { get; set; }
        public DateTime? FechaIngreso { get; set; }
    }
}
