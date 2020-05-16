using System;
using System.Collections.Generic;
using System.Linq;
using Contratos;
using Lógica_de_Negocios;
using System.Text;
using System.Threading.Tasks;

namespace Servicios
{
    class GeneralService : IServicioWeb
    {
        public Resultado AltaAlumno(Hijo hijo, UsuarioLogueado usuarioLogueado)
        {
            List<Hijo> Alumno = Archivos.Instancia.AdquirirAlumnos();
            if (Alumno != null && Alumno.Count() > 0)
                hijo.Id = Alumno.LastOrDefault().Id + 1;
            else
                hijo.Id = 100;
            Resultado resul = Empresa.Permisos(usuarioLogueado.RolSeleccionado); //Validacion de permisos de usuario
            if (resul.EsValido)
            {
                Archivos.Instancia.EscribirAlumno(hijo, false);
            }
            return resul;
        }

        public Resultado AltaDirectora(Directora directora, UsuarioLogueado usuarioLogueado)
        {
            bool bandera = false;
            if (!Empresa.RegistroUsuario(directora.Email)) //Directora inexistente
            {
                bandera = true;
                List<Usuario> usuarios = Archivos.Instancia.AdquirirUsuario();
                if (usuarios != null)
                {
                    Usuario us = usuarios.LastOrDefault();
                    if (us != null)
                        directora.Id = us.Id + 1;
                    else
                        directora.Id = 1;
                }
                else
                {
                    directora.Id = 1;
                }
            }
            else //Directora existente
            {
                Registros regis = Archivos.Instancia.AdquirirRegistros().Where(x => x.Email == directora.Email).FirstOrDefault();
                regis.Roles.ToList().Add(usuarioLogueado.RolSeleccionado);
                Archivos.Instancia.EscribirRegistro(regis, false);
            }
                Resultado ress = Empresa.Permisos(usuarioLogueado.RolSeleccionado); // validamos los permisos
            if (ress.EsValido)
            {

                Archivos.Instancia.EscribirDirectora(directora, false);
                Archivos.Instancia.EscribirUsuario(directora, false);
                if (bandera)
                {
                    List<Roles> roles = new List<Roles>();
                    roles.Add(Roles.Directora);
                    Registros nuevoregistro = new Registros()
                    {
                        Email = directora.Email,
                        Id = directora.Id,
                        Password = new Random().Next(100000, 999999).ToString(),
                        Roles = roles.ToArray(),
                    };
                    Archivos.Instancia.EscribirRegistro(nuevoregistro, false);
                }
            }
            return ress;
        }

        public Resultado AltaDocente(Docente docente, UsuarioLogueado usuarioLogueado)
        {
            throw new NotImplementedException();
        }

        public Resultado AltaNota(Nota nota, Sala[] salas, Hijo[] hijos, UsuarioLogueado usuarioLogueado)
        {
            throw new NotImplementedException();
        }

        public Resultado AltaPadreMadre(Padre padre, UsuarioLogueado usuarioLogueado)
        {
            throw new NotImplementedException();
        }

        public Resultado AsignarDocenteSala(Docente docente, Sala sala, UsuarioLogueado usuarioLogueado)
        {
            throw new NotImplementedException();
        }

        public Resultado AsignarHijoPadre(Hijo hijo, Padre padre, UsuarioLogueado usuarioLogueado)
        {
            throw new NotImplementedException();
        }

        public Resultado DesasignarDocenteSala(Docente docente, Sala sala, UsuarioLogueado usuarioLogueado)
        {
            throw new NotImplementedException();
        }

        public Resultado DesasignarHijoPadre(Hijo hijo, Padre padre, UsuarioLogueado usuarioLogueado)
        {
            throw new NotImplementedException();
        }

        public Resultado EditarAlumno(int id, Hijo hijo, UsuarioLogueado usuarioLogueado)
        {
            throw new NotImplementedException();
        }

        public Resultado EditarDirectora(int id, Directora directora, UsuarioLogueado usuarioLogueado)
        {
            throw new NotImplementedException();
        }

        public Resultado EditarDocente(int id, Docente docente, UsuarioLogueado usuarioLogueado)
        {
            throw new NotImplementedException();
        }

        public Resultado EditarPadreMadre(int id, Padre padre, UsuarioLogueado usuarioLogueado)
        {
            throw new NotImplementedException();
        }

        public Resultado EliminarAlumno(int id, Hijo hijo, UsuarioLogueado usuarioLogueado)
        {
            throw new NotImplementedException();
        }

        public Resultado EliminarDirectora(int id, Directora directora, UsuarioLogueado usuarioLogueado)
        {
            throw new NotImplementedException();
        }

        public Resultado EliminarDocente(int id, Docente docente, UsuarioLogueado usuarioLogueado)
        {
            throw new NotImplementedException();
        }

        public Resultado EliminarPadreMadre(int id, Padre padre, UsuarioLogueado usuarioLogueado)
        {
            throw new NotImplementedException();
        }

        public Resultado MarcarNotaComoLeida(Nota nota, UsuarioLogueado usuarioLogueado)
        {
            throw new NotImplementedException();
        }

        public Hijo ObtenerAlumnoPorId(UsuarioLogueado usuarioLogueado, int id)
        {
            throw new NotImplementedException();
        }

        public Grilla<Hijo> ObtenerAlumnos(UsuarioLogueado usuarioLogueado, int paginaActual, int totalPorPagina, string busquedaGlobal)
        {
            throw new NotImplementedException();
        }

        public Nota[] ObtenerCuadernoComunicaciones(int idPersona, UsuarioLogueado usuarioLogueado)
        {
            throw new NotImplementedException();
        }

        public Directora ObtenerDirectoraPorId(UsuarioLogueado usuarioLogueado, int id)
        {
            throw new NotImplementedException();
        }

        public Grilla<Directora> ObtenerDirectoras(UsuarioLogueado usuarioLogueado, int paginaActual, int totalPorPagina, string busquedaGlobal)
        {
            // var lista = Empresa.ObtenerDirectoras();
            //transformar el resultado de la logica de negocios a la clase de contratos
            //   return new Grilla<Directora>() { 
            //       Lista = ConvertirLista(lista).ToArray(), 
            //      CantidadRegistros = lista.Count() 
            //   };
            throw new NotImplementedException();
        }
        // TODO > terminar metodos y hacer metodos genericos para transformar listas de LogicaDeNegocios en Contrato
        public Docente ObtenerDocentePorId(UsuarioLogueado usuarioLogueado, int id)
        {
            throw new NotImplementedException();
        }

        public Grilla<Docente> ObtenerDocentes(UsuarioLogueado usuarioLogueado, int paginaActual, int totalPorPagina, string busquedaGlobal)
        {
            throw new NotImplementedException();
        }

        public Institucion[] ObtenerInstituciones()
        {
            throw new NotImplementedException();
        }

        public string ObtenerNombreGrupo()
        {
            return "Cominotti Mauro, Yacovino Juan, Zoja Emanuel";
        }

        public Padre ObtenerPadrePorId(UsuarioLogueado usuarioLogueado, int id)
        {
            throw new NotImplementedException();
        }

        public Grilla<Padre> ObtenerPadres(UsuarioLogueado usuarioLogueado, int paginaActual, int totalPorPagina, string busquedaGlobal)
        {
            throw new NotImplementedException();
        }

        public Hijo[] ObtenerPersonas(UsuarioLogueado usuarioLogueado)
        {
            throw new NotImplementedException();
        }

        public Sala[] ObtenerSalasPorInstitucion(UsuarioLogueado usuarioLogueado)
        {
            throw new NotImplementedException();
        }

        public UsuarioLogueado ObtenerUsuario(string email, string clave)
        {
            throw new NotImplementedException();
        }

        public Resultado ResponderNota(Nota nota, Comentario nuevoComentario, UsuarioLogueado usuarioLogueado)
        {
            throw new NotImplementedException();
        }



        // METODOS

        public List<Directora> ConvertirLista(List<LogicaDirectora> lista)
        {
            List<Directora> resultado = new List<Directora>();
            foreach (var elem in lista)
            {
                resultado.Add(new Directora()
                {
                    Apellido = elem.Apellido,
                    Email = elem.Email,
                    Cargo = elem.Cargo,
                    FechaIngreso = elem.FechaIngreso,
                    Id = elem.Id,
                    Institucion = new Institucion() { 
                        Ciudad = elem.Institucion.Ciudad,
                        Direccion = elem.Institucion.Direccion,
                        Id = elem.Institucion.Id,
                        Nombre = elem.Institucion.Nombre,
                        Provincia = elem.Institucion.Provincia,
                        Telefono = elem.Institucion.Telefono
                    },
                    Nombre = elem.Nombre
                });
            }
            return resultado;
        }
    }
}
