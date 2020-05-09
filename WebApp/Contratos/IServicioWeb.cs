using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contratos
{
    public interface IServicioWeb
    {
        /// <summary>
        /// Retornar el apellido de cada integrante del grupo de trabajo.
        /// </summary>
        /// <returns></returns>
        string ObtenerNombreGrupo();

        UsuarioLogueado ObtenerUsuario(string email, string clave);        
    }
}
