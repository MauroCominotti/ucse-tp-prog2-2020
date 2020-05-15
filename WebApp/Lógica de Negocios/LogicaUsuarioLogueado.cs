using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lógica_de_Negocios
{
    class LogicaUsuarioLogueado
    {
        public enum Roles
        {
            Padre, Directora, Docente
        }

        public class UsuarioLogueado
        {
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string Email { get; set; }
            public Roles[] Roles { get; set; }

            public Roles RolSeleccionado { get; set; }
        }
    }
}
