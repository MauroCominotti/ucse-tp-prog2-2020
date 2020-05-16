using System;
using Contratos;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lógica_de_Negocios
{
    public static class Empresa
    {
        public static bool RegistroUsuario(string email) //Para ver si ya se logueo anteriormente
        {
            List<Usuario> Usuarios = Archivos.Instancia.AdquirirUsuario();
            if (Usuarios != null)
            {
                Usuario us = Usuarios.Where(x => x.Email == email).FirstOrDefault();
                if (us != null)
                {
                    return true;
                }
            }
            return false;
        }



        public static Resultado PermisosDirectora(Roles rol)//TODO Acá para ver si tiene los permisos necesarios, hay que hacerlo con la clase Resultados
        {
            Resultado resultado = new Resultado();
            return resultado;
        } 
    }
    
}
