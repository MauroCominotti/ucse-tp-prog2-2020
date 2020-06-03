using System;
using Contratos;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaDeNegocios
{
    public static class Empresa
    {
        public static bool RegistroUsuario(string email) //Para ver si ya se logueo anteriormente
        {
            List<LogicaUsuario> Usuarios = Archivo.Instancia.Leer<LogicaUsuario>();
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

        public static Resultado PermisosDirectora(Roles rol, UsuarioLogueado usuarioLogueado) //Acá para ver si tiene los permisos necesarios y si existe en la DB
        {
            Resultado result = new Resultado();
            if ((int)rol != 1)
            {
                result.Errores.Add("Error 403: Permisos insuficientes");
            }
            else
            {
                List<LogicaDirectora> Directora = Archivo.Instancia.Leer<LogicaDirectora>();
                if (Directora == null || Directora.Count() == 0 || Directora.Find(x => x.Email == usuarioLogueado.Email) == null)
                    result.Errores.Add("Error 404: Directora no encontrada en la base de datos.");
            }
            return result;
        }

        public static Boolean MismaInstitucion(int Id, string Email) // toma un docente, alumno, etc,(Id) y un usuario(Email)
        {
            int institucionAComparar = Archivo.Instancia.Leer<LogicaUsuario>().First(x => x.Id == Id).IdInstitucion;
            int institucionDelUsuario = Archivo.Instancia.Leer<LogicaUsuario>().First(x => x.Email == Email).IdInstitucion;
            return institucionAComparar == institucionDelUsuario;
        }
    }

}
