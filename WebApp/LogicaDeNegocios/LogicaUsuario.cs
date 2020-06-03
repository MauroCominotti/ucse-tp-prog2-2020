using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contratos;

namespace LogicaDeNegocios
{
    public class LogicaUsuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public bool Eliminado { get; set; }
        public string Password { get; set; }
        public Roles[] Roles { get; set; }
        public Roles RolSeleccionado { get; set; }
        public int IdInstitucion { get; set; }
    }
}
