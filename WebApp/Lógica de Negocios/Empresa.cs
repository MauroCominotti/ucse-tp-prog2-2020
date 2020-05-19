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
            List<LogicaUsuario> Usuarios = Archivo.Instancia.LeerUsuario();
            if (Usuarios != null)
            {
                LogicaUsuario us = Usuarios.Where(x => x.Email == email).FirstOrDefault();
                if (us != null)
                {
                    return true;
                }
            }
            return false;
        }

        public static Resultado PermisosDirectora(Roles rol)   //Acá para ver si tiene los permisos necesarios
        {
            Resultado result = new Resultado();
            if ((int)rol != 1)  // TODO > PREGUNTAR EMA ZOJA
            {
                result.Errores.Add("Permisos insuficientes");
            }
            return result;
        } 
    }
    
}
