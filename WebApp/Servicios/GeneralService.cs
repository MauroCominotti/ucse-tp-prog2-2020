<<<<<<< HEAD
﻿using System;
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
            List<LogicaHijo> Alumno = Archivo.Instancia.LeerAlumnos();
            if (Alumno != null && Alumno.Count() > 0)
                hijo.Id = Alumno.LastOrDefault().Id + 1;
            else
                resul.Errores.Add("Error 404: Alumno no encontrado en la base de datos.");
                //hijo.Id = 100; // TODO > Aca tendria q dar un error
            resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado); //Validacion de permisos de usuario
            if (resul.EsValido)
            {
                var hijoCasteado = AutoMapper.Instancia.Mapear(hijo, new LogicaHijo());
                Archivo.Instancia.Guardar(hijoCasteado, false);
            }
            
            return resul;
=======
﻿using Contratos;
using Logica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios
{

    public class GeneralService : IServicioWeb
    {
        private static Empresa Empresa = new Empresa();

        public Resultado AltaAlumno(Hijo hijo, UsuarioLogueado usuarioLogueado)
        {
            throw new NotImplementedException();
>>>>>>> f556d7b46368999b6cb92bf1f91f10dfc8988210
        }

        public Resultado AltaDirectora(Directora directora, UsuarioLogueado usuarioLogueado)
        {
<<<<<<< HEAD

            bool bandera = false;
            if (!Empresa.RegistroUsuario(directora.Email)) //Directora inexistente
            {
                bandera = true;
                List<LogicaUsuario> usuarios = Archivo.Instancia.LeerUsuario();
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
                Registros regis = Archivo.Instancia.LeerRegistros().Where(x => x.Email == directora.Email).FirstOrDefault();
                regis.Roles.ToList().Add(usuarioLogueado.RolSeleccionado);
                Archivo.Instancia.Guardar(regis, false);
            }
                Resultado ress = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado); // validamos los permisos
            if (ress.EsValido)
            {
                var directoraCasteada = AutoMapper.Instancia.Mapear(directora, new LogicaDirectora());
                Archivo.Instancia.Guardar(directoraCasteada, false);
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
=======
            throw new NotImplementedException();
>>>>>>> f556d7b46368999b6cb92bf1f91f10dfc8988210
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
<<<<<<< HEAD
            var lista = Archivo.Instancia.LeerAlumnos();
            //transformar el resultado de la logica de negocios a la clase de contratos
            return new Grilla<Hijo>()
            {
                Lista = AutoMapper.Instancia.ConvertirLista(lista, new Hijo())
                .Where(x => string.IsNullOrEmpty(busquedaGlobal) || x.Nombre.Contains(busquedaGlobal) || x.Apellido.Contains(busquedaGlobal))
                .Skip(paginaActual * totalPorPagina).Take(totalPorPagina).ToArray(),
                CantidadRegistros = lista.Count()
            };
=======
            throw new NotImplementedException();
>>>>>>> f556d7b46368999b6cb92bf1f91f10dfc8988210
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
<<<<<<< HEAD
            var lista = Archivo.Instancia.LeerDirectoras(); // TODO > Preguntar Maxi permisos para leer lista
            //transformar el resultado de la logica de negocios a la clase de contratos
            return new Grilla<Directora>()
            {
                Lista = AutoMapper.Instancia.ConvertirLista(lista, new Directora())
                .Where(x => string.IsNullOrEmpty(busquedaGlobal) || x.Nombre.Contains(busquedaGlobal) || x.Apellido.Contains(busquedaGlobal))
                .Skip(paginaActual * totalPorPagina).Take(totalPorPagina).ToArray(),
                CantidadRegistros = lista.Count()
            };
        }
        
=======
            var lista = Empresa.ObtenerDIrectoras();

            //transformar el resultado de la logica de negocios a la clase de contratos
            List<Directora> resultado = new List<Directora>();
            foreach (var item in lista)
            {
                resultado.Add(new Directora()
                {
                    Id = item.Id
                });
            }

            return new Grilla<Directora>() { Lista = resultado.ToArray(), CantidadRegistros = lista.Count };
        }

>>>>>>> f556d7b46368999b6cb92bf1f91f10dfc8988210
        public Docente ObtenerDocentePorId(UsuarioLogueado usuarioLogueado, int id)
        {
            throw new NotImplementedException();
        }

        public Grilla<Docente> ObtenerDocentes(UsuarioLogueado usuarioLogueado, int paginaActual, int totalPorPagina, string busquedaGlobal)
        {
<<<<<<< HEAD
            var lista = Archivo.Instancia.LeerDirectoras(); // TODO > Cambiar metodo LeerDirectoras() a LeerDocentes()
            //transformar el resultado de la logica de negocios a la clase de contratos
            return new Grilla<Docente>()
            {
                Lista = AutoMapper.Instancia.ConvertirLista(lista, new Docente())
                .Where(x => string.IsNullOrEmpty(busquedaGlobal) || x.Nombre.Contains(busquedaGlobal) || x.Apellido.Contains(busquedaGlobal))
                .Skip(paginaActual * totalPorPagina).Take(totalPorPagina).ToArray(),
                CantidadRegistros = lista.Count()
            };
=======
            throw new NotImplementedException();
>>>>>>> f556d7b46368999b6cb92bf1f91f10dfc8988210
        }

        public Institucion[] ObtenerInstituciones()
        {
            throw new NotImplementedException();
        }

        public string ObtenerNombreGrupo()
        {
<<<<<<< HEAD
            return "Cominotti Mauro, Yacovino Juan, Zoja Emanuel";
=======
            throw new NotImplementedException();
>>>>>>> f556d7b46368999b6cb92bf1f91f10dfc8988210
        }

        public Padre ObtenerPadrePorId(UsuarioLogueado usuarioLogueado, int id)
        {
            throw new NotImplementedException();
        }

        public Grilla<Padre> ObtenerPadres(UsuarioLogueado usuarioLogueado, int paginaActual, int totalPorPagina, string busquedaGlobal)
        {
<<<<<<< HEAD
            var lista = Archivo.Instancia.LeerDirectoras();  // TODO > Cambiar metodo LeerDirectoras() a LeerPadres()
            //transformar el resultado de la logica de negocios a la clase de contratos
            return new Grilla<Padre>()
            {
                Lista = AutoMapper.Instancia.ConvertirLista(lista, new Padre())
                .Where(x => string.IsNullOrEmpty(busquedaGlobal) || x.Nombre.Contains(busquedaGlobal) || x.Apellido.Contains(busquedaGlobal))
                .Skip(paginaActual * totalPorPagina).Take(totalPorPagina).ToArray(),
                CantidadRegistros = lista.Count()
            };
=======
            throw new NotImplementedException();
>>>>>>> f556d7b46368999b6cb92bf1f91f10dfc8988210
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
<<<<<<< HEAD

=======
>>>>>>> f556d7b46368999b6cb92bf1f91f10dfc8988210
    }
}
