using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaDeNegocios
{
    public class LogicaPadre : LogicaUsuario
    {
        public List<LogicaHijo> Hijos { get; set; }

        public void CopiarParametrosFaltantes(LogicaPadre padreSeleccionado)
        {
            Hijos = padreSeleccionado.Hijos;
            Password = padreSeleccionado.Password;
            Roles = padreSeleccionado.Roles;
            RolSeleccionado = padreSeleccionado.RolSeleccionado;
            IdInstitucion = padreSeleccionado.IdInstitucion;
        }
    }
}
