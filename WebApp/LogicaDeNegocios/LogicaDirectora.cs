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

        public void CopiarParametrosFaltantes(LogicaDirectora directoraSeleccionada)
        {
            Cargo = directoraSeleccionada.Cargo;
            Institucion = directoraSeleccionada.Institucion;
            Password = directoraSeleccionada.Password;
            Roles = directoraSeleccionada.Roles;
            RolSeleccionado = directoraSeleccionada.RolSeleccionado;
            IdInstitucion = directoraSeleccionada.IdInstitucion;
        }
    }
}
