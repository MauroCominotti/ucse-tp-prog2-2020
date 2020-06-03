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

        public static Resultado PermisosDirectora(Roles rol) //Acá para ver si tiene los permisos necesarios
        {
            Resultado result = new Resultado();
            if ((int)rol != 1)
            {
                result.Errores.Add("Permisos insuficientes");
            }
            return result;
        } 

        public static Boolean MismaInstitucion(int Id, string Email) // toma un docente, alumno, etc,(Id) y un usuario(Email)
        {
            string institucionAComparar = Archivo.Instancia.Leer<Registros>().First(x => x.Id == Id).Institucion.Nombre;
            string institucionDelUsuario = Archivo.Instancia.Leer<Registros>().First(x => x.Email == Email).Institucion.Nombre;
            return institucionAComparar == institucionDelUsuario;
        }
    }
    
}
