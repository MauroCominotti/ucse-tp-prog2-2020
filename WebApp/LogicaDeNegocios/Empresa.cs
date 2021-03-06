﻿using System;
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
            try
            {
                int institucionAComparar = Archivo.Instancia.Leer<LogicaUsuario>().FirstOrDefault(x => x.Id == Id && x.Eliminado == false).IdInstitucion;
                int institucionDelUsuario = Archivo.Instancia.Leer<LogicaUsuario>().FirstOrDefault(x => x.Email == Email && x.Eliminado == false).IdInstitucion;
                return institucionAComparar == institucionDelUsuario;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static int ? IDInstitucionUsuarioLogueado(string Email) // toma un docente, alumno, etc,(Id) y un usuario(Email)
        {
            try
            {
                int institucionDelUsuario = Archivo.Instancia.Leer<LogicaUsuario>().FirstOrDefault(x => x.Email == Email && x.Eliminado == false).IdInstitucion;
                return institucionDelUsuario;
            }
            catch (Exception)
            {
                return 0; // no hay instituciones con id = 0, por eso se puede devolver
            }
        }
    }

}
