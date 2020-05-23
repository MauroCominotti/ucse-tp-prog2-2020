using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaDeNegocios
{
    public class LogicaHijo : LogicaUsuario
    {
        public LogicaInstitucion Institucion { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public int ResultadoUltimaEvaluacionAnual { get; set; }
        public LogicaSala Sala { get; set; }
        public LogicaNota[] Notas { get; set; }
    }
}
