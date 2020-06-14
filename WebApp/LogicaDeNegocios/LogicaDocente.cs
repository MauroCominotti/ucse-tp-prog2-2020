using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaDeNegocios
{
    public class LogicaDocente : LogicaUsuario
    {
        public List<LogicaSala> Salas { get; set; }

        public void CopiarParametrosFaltantes(LogicaDocente docenteSeleccionada)
        {
            Salas = docenteSeleccionada.Salas;
            Password = docenteSeleccionada.Password;
            Roles = docenteSeleccionada.Roles;
            RolSeleccionado = docenteSeleccionada.RolSeleccionado;
            IdInstitucion = docenteSeleccionada.IdInstitucion;
        }
    }
}
