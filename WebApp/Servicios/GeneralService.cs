using System;
using System.Collections.Generic;
using System.Linq;
using Contratos;
using Lógica_de_Negocios;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace Servicios
{
    class GeneralService : IServicioWeb
    {
        public Resultado AltaAlumno(Hijo hijo, UsuarioLogueado usuarioLogueado)
        {
            Resultado resul = new Resultado();
            List<LogicaHijo> Alumno = Archivo.Instancia.Leer<LogicaHijo>();
            if (Alumno != null && Alumno.Count() > 0)
                hijo.Id = Alumno.LastOrDefault().Id + 1;
            else
                resul.Errores.Add("Error 404: Alumno no encontrado en la base de datos.");
            resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado); //Validacion de permisos de usuario
            if (resul.EsValido)
            {
                var hijoCasteado = AutoMapper.Instancia.Mapear<Hijo, LogicaHijo>(hijo);
                Archivo.Instancia.Guardar(hijoCasteado, false);
            }
            return resul;
        }

        public Resultado AltaDirectora(Directora directora, UsuarioLogueado usuarioLogueado)
        {

            bool bandera = false;
            if (!Empresa.RegistroUsuario(directora.Email)) //Directora inexistente
            {
                bandera = true;
                List<LogicaUsuario> usuarios = Archivo.Instancia.Leer<LogicaUsuario>();
                if (usuarios != null)
                {
                    LogicaUsuario us = usuarios.LastOrDefault();
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
                Registros regis = Archivo.Instancia.Leer<Registros>().Where(x => x.Email == directora.Email).FirstOrDefault();
                regis.Roles.ToList().Add(usuarioLogueado.RolSeleccionado);
                Archivo.Instancia.Guardar(regis, false);
            }
                Resultado ress = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado); // validamos los permisos
            if (ress.EsValido)
            {
                var directoraCasteada = AutoMapper.Instancia.Mapear<Directora, LogicaDirectora>(directora);
                Archivo.Instancia.Guardar(directoraCasteada, false);
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
                    Archivo.Instancia.Guardar(nuevoregistro, false);
                }
            }
            return ress;
        }

        public Resultado AltaDocente(Docente docente, UsuarioLogueado usuarioLogueado)
        {
            Resultado resul = new Resultado();
            List<LogicaDocente> Docente = Archivo.Instancia.Leer<LogicaDocente>();
            if (Docente != null && Docente.Count() > 0)
                docente.Id = Docente.LastOrDefault().Id + 1;
            else
                resul.Errores.Add("Error 404: Docente no encontrado en la base de datos.");
            resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado); //Validacion de permisos de usuario
            if (resul.EsValido)
            {
                var docenteCasteado = AutoMapper.Instancia.Mapear<Docente, LogicaDocente>(docente);
                Archivo.Instancia.Guardar(docenteCasteado, false);
            }
            return resul;
        }

        public Resultado AltaNota(Nota nota, Sala[] salas, Hijo[] hijos, UsuarioLogueado usuarioLogueado)
        {
            // si yo tengo algun elemento dentro del array de hijos creo una nota a cada uno de esos hijos.
            // si viene vacio tengo q usar el array de salas y tengo q buscar en cada sala todos los alumnos 
            // q esten ahi adentro y mandarle las notas a todos los alumnos q esten ahi
            // 1 nota por alumno

            throw new NotImplementedException();
        }

        public Resultado AltaPadreMadre(Padre padre, UsuarioLogueado usuarioLogueado)
        {
            Resultado resul = new Resultado();
            List<LogicaPadre> Padre = Archivo.Instancia.Leer<LogicaPadre>(); // TODO > Leer<>()
            if (Padre != null && Padre.Count() > 0)
                padre.Id = Padre.LastOrDefault().Id + 1;
            else
                resul.Errores.Add("Error 404: Padre no encontrado en la base de datos.");
            resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado); //Validacion de permisos de usuario
            if (resul.EsValido)
            {
                var padreCasteado = AutoMapper.Instancia.Mapear<Padre, LogicaPadre>(padre);
                Archivo.Instancia.Guardar(padreCasteado, false);
            }
            return resul;
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
            Resultado resul = new Resultado();
            List<LogicaHijo> Alumno = Archivo.Instancia.Leer<LogicaHijo>();
            if (Alumno != null && Alumno.Count() > 0)
                resul.Errores.Add("Error 404: Alumno no encontrado en la base de datos.");
            resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado); //Validacion de permisos de usuario
            if (resul.EsValido)
            {
                var hijoCasteado = AutoMapper.Instancia.Mapear<Hijo, LogicaHijo>(hijo);
                Archivo.Instancia.Guardar(hijoCasteado, false);
            }
            return resul;
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
            List<LogicaHijo> lista = Archivo.Instancia.Leer<LogicaHijo>();
            //transformar el resultado de la logica de negocios a la clase de contratos
            return new Grilla<Hijo>()
            {
                Lista = AutoMapper.Instancia.ConvertirLista<LogicaHijo, Hijo>(lista)
                .Where(x => string.IsNullOrEmpty(busquedaGlobal) || x.Nombre.Contains(busquedaGlobal) || x.Apellido.Contains(busquedaGlobal))
                .Skip(paginaActual * totalPorPagina).Take(totalPorPagina).ToArray(),
                CantidadRegistros = lista.Count()
            };
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
            List<LogicaDirectora> lista = Archivo.Instancia.Leer<LogicaDirectora>(); // TODO > Preguntar Maxi permisos para leer lista
            //transformar el resultado de la logica de negocios a la clase de contratos
            return new Grilla<Directora>()
            {
                Lista = AutoMapper.Instancia.ConvertirLista<LogicaDirectora, Directora>(lista)
                .Where(x => string.IsNullOrEmpty(busquedaGlobal) || x.Nombre.Contains(busquedaGlobal) || x.Apellido.Contains(busquedaGlobal))
                .Skip(paginaActual * totalPorPagina).Take(totalPorPagina).ToArray(),
                CantidadRegistros = lista.Count()
            };
        }
        
        public Docente ObtenerDocentePorId(UsuarioLogueado usuarioLogueado, int id)
        {
            throw new NotImplementedException();
        }

        public Grilla<Docente> ObtenerDocentes(UsuarioLogueado usuarioLogueado, int paginaActual, int totalPorPagina, string busquedaGlobal)
        {
            List<LogicaDocente> lista = Archivo.Instancia.Leer<LogicaDocente>();
            //transformar el resultado de la logica de negocios a la clase de contratos
            return new Grilla<Docente>()
            {
                Lista = AutoMapper.Instancia.ConvertirLista<LogicaDocente, Docente>(lista)
                .Where(x => string.IsNullOrEmpty(busquedaGlobal) || x.Nombre.Contains(busquedaGlobal) || x.Apellido.Contains(busquedaGlobal))
                .Skip(paginaActual * totalPorPagina).Take(totalPorPagina).ToArray(),
                CantidadRegistros = lista.Count()
            };
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
            List<LogicaPadre> lista = Archivo.Instancia.Leer<LogicaPadre>();
            //transformar el resultado de la logica de negocios a la clase de contratos
            return new Grilla<Padre>()
            {
                Lista = AutoMapper.Instancia.ConvertirLista<LogicaPadre, Padre>(lista)
                .Where(x => string.IsNullOrEmpty(busquedaGlobal) || x.Nombre.Contains(busquedaGlobal) || x.Apellido.Contains(busquedaGlobal))
                .Skip(paginaActual * totalPorPagina).Take(totalPorPagina).ToArray(),
                CantidadRegistros = lista.Count()
            };
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
    }
}
