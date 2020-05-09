using Contratos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocks
{
    public class MockService : IServicioWeb
    {
        public string ObtenerNombreGrupo()
        {
            return $"Repo original";
        }

     
        public UsuarioLogueado ObtenerUsuario(string email, string clave)
        {
            if (email == "" || clave == "")
                return null;

            if (email == "directora@ucse.com" && clave == "123456")
                return new UsuarioLogueado() { Email = email, Nombre = "Usuario", Apellido = "Directora", Roles = new Roles[] { Roles.Directora }, RolSeleccionado = Roles.Directora };

            if (email == "docente@ucse.com" && clave == "123456")
                return new UsuarioLogueado() { Email = email, Nombre = "Usuario", Apellido = "Docente", Roles = new Roles[] { Roles.Docente }, RolSeleccionado = Roles.Docente };

            if (email == "padre@ucse.com" && clave == "123456")
                return new UsuarioLogueado() { Email = email, Nombre = "Usuario", Apellido = "Padre", Roles = new Roles[] { Roles.Padre }, RolSeleccionado = Roles.Padre };

            return null;
        }
        
    }
}
