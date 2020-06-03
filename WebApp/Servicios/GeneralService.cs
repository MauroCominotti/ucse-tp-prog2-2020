using System;
using System.Collections.Generic;
using System.Linq;
using Contratos;
using LogicaDeNegocios;

namespace Servicios
{
    public class GeneralService : IServicioWeb
    {
        public Resultado AltaAlumno(Hijo hijo, UsuarioLogueado usuarioLogueado)
        {
            Resultado resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado); // TODO > Cambie resul de lugar porque sino se sobreescribe al ponerlo al final
            List<LogicaHijo> Alumno = Archivo.Instancia.Leer<LogicaHijo>();
            if (Alumno != null || Alumno.Count() > 0)
                hijo.Id = Alumno.LastOrDefault().Id + 1;
            else
                resul.Errores.Add("Error 404: Alumno no encontrado en la base de datos.");
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
            // TODO > Corregir todos los casos 1) agg resul error 403 - 2) error si ya se encuentra - 3) no hubo error al leer
            Resultado resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado);
            List<LogicaDocente> Docente = Archivo.Instancia.Leer<LogicaDocente>();
            if (Docente != null) // no hubo ningun error al leer, puede ser q sea una lista vacia
            {
                if (Docente.Count() == 0)
                    docente.Id = 1;
                else
                    docente.Id = Docente.LastOrDefault().Id + 1;
            }
            else
                resul.Errores.Add("Error 404: Docente no encontrado en la base de datos.");
            if (resul.EsValido)
            {
                if (Docente.Find(x=> x.Id == docente.Id) == null)
                    resul.Errores.Add("Error 404: Docente ya se encuentra en la base de datos.");
                else
                {
                    var docenteCasteado = AutoMapper.Instancia.Mapear<Docente, LogicaDocente>(docente);
                    Archivo.Instancia.Guardar(docenteCasteado, false);
                }
            }
            else
                resul.Errores.Add("Error 403: Docente no tiene los permisos suficientes.");
            return resul;
        }

        public Resultado AltaNota(Nota nota, Sala[] salas, Hijo[] hijos, UsuarioLogueado usuarioLogueado)
        {
            // si yo tengo algun elemento dentro del array de hijos creo una nota a cada uno de esos hijos.
            // si viene vacio tengo q usar el array de salas y tengo q buscar en cada sala todos los alumnos 
            // q esten ahi adentro y mandarle las notas a todos los alumnos q esten ahi
            // 1 nota por alumno
            Resultado resultado = new Resultado();
            var alumnosLogica = Archivo.Instancia.Leer<LogicaHijo>();
            var notaMap = AutoMapper.Instancia.Mapear<Nota, LogicaNota>(nota);
            var salasLista = salas.ToList();
            var alumnosSalas = alumnosLogica.FindAll(alm => salasLista.Exists(sala => sala.Id == alm.Sala.Id));


            if (usuarioLogueado.RolSeleccionado == Roles.Directora)
            {
                if (hijos != null)
                {
                    foreach (var hijo in hijos)
                    {
                        hijo.Notas.ToList().Add(nota);
                        var hijoMap = AutoMapper.Instancia.Mapear<Hijo, LogicaHijo>(hijo);
                        Archivo.Instancia.Guardar(hijoMap, false);
                    }
                }
                if (salas != null)
                {
                    foreach (var alumno in alumnosSalas)
                    {
                        alumno.Notas.ToList().Add(notaMap);
                        Archivo.Instancia.Guardar(alumno, false);
                    }
                }
            }
            if (usuarioLogueado.RolSeleccionado == Roles.Docente)
            {
                foreach (var alumno in alumnosSalas)
                {
                    alumno.Notas.ToList().Add(notaMap);
                    Archivo.Instancia.Guardar(alumno, false);
                }
                
            }
            if (usuarioLogueado.RolSeleccionado == Roles.Padre)
            {
                var padre = Archivo.Instancia.Leer<LogicaPadre>().Find(x => x.Email == usuarioLogueado.Email);
                var hijosLogica = AutoMapper.Instancia.ConvertirLista<Hijo, LogicaHijo>(hijos.ToList());
                var hijosPadreLogica = padre.Hijos.ToList();
                foreach (var hijo in hijosPadreLogica)
                {
                    var siexiste = hijosLogica.Exists(x => x == hijo);
                    hijo.Notas.ToList().Add(notaMap);
                    Archivo.Instancia.Guardar(padre, false);
                }
                


            }


            return resultado;
        }

        public Resultado AltaPadreMadre(Padre padre, UsuarioLogueado usuarioLogueado)
        {
            Resultado resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado);
            List<LogicaPadre> Padre = Archivo.Instancia.Leer<LogicaPadre>();
            if (Padre != null || Padre.Count() > 0)
                padre.Id = Padre.LastOrDefault().Id + 1;
            else
                resul.Errores.Add("Error 404: Padre no encontrado en la base de datos.");
            if (resul.EsValido)
            {
                var padreCasteado = AutoMapper.Instancia.Mapear<Padre, LogicaPadre>(padre);
                Archivo.Instancia.Guardar(padreCasteado, false);
            }
            return resul;
        }

        public Resultado AsignarDocenteSala(Docente docente, Sala sala, UsuarioLogueado usuarioLogueado)
        {
            Resultado nuevardo = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado);
            if (nuevardo.EsValido)
            {
                if (docente.Salas == null)
                {
                    docente.Salas = new Sala[] { sala };
                    var docenteCasteado = AutoMapper.Instancia.Mapear<Docente, LogicaDocente>(docente);
                    Archivo.Instancia.Guardar(docenteCasteado, false);
                }
                else
                {
                    List<Sala> listsalas = docente.Salas.ToList();
                    Sala salaasig = listsalas.Where(x => x.Id == sala.Id).FirstOrDefault();
                    if (salaasig == null)
                    {
                        listsalas.Add(sala);
                        docente.Salas = listsalas.ToArray();
                        var docenteCasteado = AutoMapper.Instancia.Mapear<Docente, LogicaDocente>(docente);
                        Archivo.Instancia.Guardar(docenteCasteado, false);
                    }
                }
            }
            return nuevardo;
        }

        public Resultado AsignarHijoPadre(Hijo hijo, Padre padre, UsuarioLogueado usuarioLogueado)
        {
            //El usuario debe ser directora, y el hijo debe estar asociado a una sala de su institucion
            var resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado);
            if (resultado.EsValido)
            {

            }
            return resultado;
        }

        public Resultado DesasignarDocenteSala(Docente docente, Sala sala, UsuarioLogueado usuarioLogueado)
        {
            return null; 
        }

        public Resultado DesasignarHijoPadre(Hijo hijo, Padre padre, UsuarioLogueado usuarioLogueado)
        {
            throw new NotImplementedException();
        }

        public Resultado EditarAlumno(int id, Hijo hijo, UsuarioLogueado usuarioLogueado)
        {
            Resultado resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado);
            if (resultado.EsValido)
            {
                List<LogicaPadre> padres = Archivo.Instancia.Leer<LogicaPadre>();
                if (padres != null)
                {
                    foreach (var item in padres)
                    {
                        LogicaHijo hije = item.Hijos.Where(x => x.Id == id).FirstOrDefault();
                        if (hije != null)
                        {
                            List<LogicaHijo> hijos = item.Hijos.ToList();
                            hijos.Remove(hije);
                            var hijo1 = AutoMapper.Instancia.Mapear<Hijo, LogicaHijo>(hijo);
                            hijos.Add(hijo1);
                            item.Hijos = hijos.ToArray();
                            Archivo.Instancia.Guardar(item, false);
                        }
                    }
                    var hijoo = AutoMapper.Instancia.Mapear<Hijo, LogicaHijo>(hijo);
                    Archivo.Instancia.Guardar(hijoo, false);
                }
                else
                {
                    var hijo2 = AutoMapper.Instancia.Mapear<Hijo, LogicaHijo>(hijo);
                    Archivo.Instancia.Guardar(hijo2, false);
                }
            }
            return resultado;
        }

        public Resultado EditarDirectora(int id, Directora directora, UsuarioLogueado usuarioLogueado)
        {
            var resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado);

            if (resultado.EsValido)
            {
                var directoraMapeada = AutoMapper.Instancia.Mapear<Directora, LogicaDirectora>(directora); 
                var directoras = Archivo.Instancia.Leer<LogicaDirectora>();
                var directoraEncontrada = directoras.Find(dir => dir.Id == id);
                if (directoraEncontrada != null)
                {
                    directoraEncontrada.Eliminado = true;
                    Archivo.Instancia.Guardar(directoraEncontrada, false);
                    Archivo.Instancia.Guardar(directoraMapeada, false);
                }
                else
                {
                    resultado.Errores.Add("No existe la directora");
                }
            }
            else
            {
                resultado.Errores.Add("No tiene permisos");
            }

            return resultado;
        }

        public Resultado EditarDocente(int id, Docente docente, UsuarioLogueado usuarioLogueado)
        {
            var resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado);

            if (resultado.EsValido)
            {
                var docenteMapeada = AutoMapper.Instancia.Mapear<Docente, LogicaDocente>(docente);
                var docentes = Archivo.Instancia.Leer<LogicaDirectora>();
                var docenteEncontrada = docentes.Find(x => x.Id == id);
                if (docenteEncontrada != null)
                {
                    docenteEncontrada.Eliminado = true;
                    Archivo.Instancia.Guardar(docenteEncontrada, false);
                    Archivo.Instancia.Guardar(docenteMapeada, false);
                }
                else
                {
                    resultado.Errores.Add("no existe directora");
                }
            }
            else
            {
                resultado.Errores.Add("no tenes permisos");
            }

            return resultado;
        }

        public Resultado EditarPadreMadre(int id, Padre padre, UsuarioLogueado usuarioLogueado)
        {
            var resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado);

            if (resultado.EsValido)
            {
                var padreMapeado = AutoMapper.Instancia.Mapear<Padre, LogicaPadre>(padre);
                var padres = Archivo.Instancia.Leer<LogicaPadre>();
                var padreEncontrado = padres.Find(x => x.Id == id);
                if (padreEncontrado != null)
                {
                    padreEncontrado.Eliminado = true;
                    Archivo.Instancia.Guardar(padreEncontrado, true);
                    Archivo.Instancia.Guardar(padreMapeado, true);
                }
                else
                {
                    resultado.Errores.Add("no existe padre");
                }
            }
            else
            {
                resultado.Errores.Add("no tenes permisos");
            }

            return resultado;
        }

        public Resultado EliminarAlumno(int id, Hijo hijo, UsuarioLogueado usuarioLogueado)
        {
            Resultado resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado);
            List<LogicaHijo> Alumno = Archivo.Instancia.Leer<LogicaHijo>();
            if (Alumno == null || Alumno.Count() == 0)
                resul.Errores.Add("Error 404: Alumno no encontrado en la base de datos.");
            else
            {
                LogicaHijo alumnoSeleccionado = Alumno.Find(x => x.Id == id);
                if (alumnoSeleccionado == null)
                    resul.Errores.Add("Error 404: Alumno no encontrado en la base de datos.");
            }
            if (resul.EsValido)
            {
                var hijoCasteado = AutoMapper.Instancia.Mapear<Hijo, LogicaHijo>(hijo);
                Archivo.Instancia.Guardar(hijoCasteado, true);
            }
            return resul;
        }

        public Resultado EliminarDirectora(int id, Directora directora, UsuarioLogueado usuarioLogueado)
        {
            Resultado resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado);
            List<LogicaDirectora> Directora = Archivo.Instancia.Leer<LogicaDirectora>();
            if (Directora == null || Directora.Count() == 0)
                resul.Errores.Add("Error 404: Directora no encontrado en la base de datos.");
            else
            {
                LogicaDirectora directoraSeleccionado = Directora.Find(x => x.Id == id);
                if (directoraSeleccionado == null)
                    resul.Errores.Add("Error 404: Directora no encontrado en la base de datos.");
            }
            if (resul.EsValido)
            {
                var directoraCasteado = AutoMapper.Instancia.Mapear<Directora, LogicaDirectora>(directora);
                Archivo.Instancia.Guardar(directoraCasteado, true);
            }
            return resul;
        }

        public Resultado EliminarDocente(int id, Docente docente, UsuarioLogueado usuarioLogueado)
        {
            Resultado resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado);
            List<LogicaDocente> Docente = Archivo.Instancia.Leer<LogicaDocente>();
            if (Docente == null || Docente.Count() == 0)
                resul.Errores.Add("Error 404: Docente no encontrado en la base de datos.");
            else
            {
                LogicaDocente docenteSeleccionado = Docente.Find(x => x.Id == id);
                if (docenteSeleccionado == null)
                    resul.Errores.Add("Error 404: Docente no encontrado en la base de datos.");
            }
            if (resul.EsValido)
            {
                var docenteCasteado = AutoMapper.Instancia.Mapear<Docente, LogicaDocente>(docente);
                Archivo.Instancia.Guardar(docenteCasteado, true);
            }
            return resul;
        }

        public Resultado EliminarPadreMadre(int id, Padre padre, UsuarioLogueado usuarioLogueado)
        {
            Resultado resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado);
            List<LogicaPadre> Padre = Archivo.Instancia.Leer<LogicaPadre>();
            if (Padre == null || Padre.Count() == 0)
                resul.Errores.Add("Error 404: Padre no encontrado en la base de datos.");
            else
            {
                LogicaPadre padreSeleccionado = Padre.Find(x => x.Id == id);
                if (padreSeleccionado == null)
                    resul.Errores.Add("Error 404: Padre no encontrado en la base de datos.");
            }
            if (resul.EsValido)
            {
                var padreCasteado = AutoMapper.Instancia.Mapear<Padre, LogicaPadre>(padre);
                Archivo.Instancia.Guardar(padreCasteado, true);
            }
            return resul;
        }

        public Resultado MarcarNotaComoLeida(Nota nota, UsuarioLogueado usuarioLogueado)
        {
            var resultado = new Resultado();
            if (usuarioLogueado.RolSeleccionado == Roles.Padre)
            {
                nota.Leida = true;
            }
            else
            {
                resultado.Errores.Add("No tiene permisos");
            }
            return resultado;
        }

        public Hijo ObtenerAlumnoPorId(UsuarioLogueado usuarioLogueado, int id)
        {
            LogicaHijo alumno = Archivo.Instancia.Leer<LogicaHijo>().Where(x => x.Id == id).FirstOrDefault();
            var alumnoo = AutoMapper.Instancia.Mapear<LogicaHijo, Hijo>(alumno);
            return alumnoo;
        }

        public Grilla<Hijo> ObtenerAlumnos(UsuarioLogueado usuarioLogueado, int paginaActual, int totalPorPagina, string busquedaGlobal)
        {
            Resultado resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado);
            if (resul.EsValido)
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
            else
            {
                return new Grilla<Hijo>();
            }
        }

        public Nota[] ObtenerCuadernoComunicaciones(int idPersona, UsuarioLogueado usuarioLogueado)
        {
            List<Nota> notas = new List<Nota>();
            if (usuarioLogueado.RolSeleccionado == Roles.Directora)
            {
                var directora = Archivo.Instancia.Leer<LogicaDirectora>().Find(x => x.Id == idPersona);
                var alumnos = Archivo.Instancia.Leer<LogicaHijo>().FindAll(alm => alm.Institucion == directora.Institucion);
                foreach (var alumno in alumnos)
                {
                    var notasALM = alumno.Notas.ToList();
                    var notasMapeadas = AutoMapper.Instancia.ConvertirLista<LogicaNota, Nota>(notasALM);
                    notas.AddRange(notasMapeadas);
                }
                
            }
            if (usuarioLogueado.RolSeleccionado == Roles.Docente)
            {
                var docente = Archivo.Instancia.Leer<LogicaDocente>().Find(x => x.Id == idPersona);
                var salasDocente = docente.Salas.ToList();
                var alumnosDocente = Archivo.Instancia.Leer<LogicaHijo>().
                    FindAll(alm => salasDocente.Exists(sala => sala.Id == alm.Sala.Id)); //ver si funciona
                
                foreach (var alumno in alumnosDocente)
                {
                    var notasALM = alumno.Notas.ToList();
                    var notasMapeadas = AutoMapper.Instancia.ConvertirLista<LogicaNota, Nota>(notasALM);
                    notas.AddRange(notasMapeadas);
                }
                
            }
            if (usuarioLogueado.RolSeleccionado == Roles.Padre)
            {
                var padre = Archivo.Instancia.Leer<LogicaPadre>().Find(x => x.Id == idPersona);
                foreach (var hijo in padre.Hijos)
                {
                    var notasALM = hijo.Notas.ToList();
                    var notasMapeadas = AutoMapper.Instancia.ConvertirLista<LogicaNota, Nota>(notasALM);
                    notas.AddRange(notasMapeadas);
                }
            }

            return notas.ToArray();
        }

        public Directora ObtenerDirectoraPorId(UsuarioLogueado usuarioLogueado, int id)
        {

            LogicaDirectora directora = Archivo.Instancia.Leer<LogicaDirectora>().Where(x => x.Id == id).FirstOrDefault();
            var directoraa = AutoMapper.Instancia.Mapear<LogicaDirectora, Directora>(directora);
            return directoraa;
        }

        public Grilla<Directora> ObtenerDirectoras(UsuarioLogueado usuarioLogueado, int paginaActual, int totalPorPagina, string busquedaGlobal)
        {
            Resultado resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado);
            if (resul.EsValido)
            {
                List<LogicaDirectora> lista = Archivo.Instancia.Leer<LogicaDirectora>();
                //transformar el resultado de la logica de negocios a la clase de contratos
                return new Grilla<Directora>()
                {
                    Lista = AutoMapper.Instancia.ConvertirLista<LogicaDirectora, Directora>(lista)
                    .Where(x => string.IsNullOrEmpty(busquedaGlobal) || x.Nombre.Contains(busquedaGlobal) || x.Apellido.Contains(busquedaGlobal))
                    .Skip(paginaActual * totalPorPagina).Take(totalPorPagina).ToArray(),
                    CantidadRegistros = lista.Count()
                };
            }
            else
            {
                return new Grilla<Directora>();
            }
        }

        public Docente ObtenerDocentePorId(UsuarioLogueado usuarioLogueado, int id)
        {

            LogicaDocente docente = Archivo.Instancia.Leer<LogicaDocente>().Where(x => x.Id == id).FirstOrDefault();
            var docentee = AutoMapper.Instancia.Mapear<LogicaDocente, Docente>(docente);
            return docentee;

        }

        public Grilla<Docente> ObtenerDocentes(UsuarioLogueado usuarioLogueado, int paginaActual, int totalPorPagina, string busquedaGlobal)
        {
            Resultado resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado);
            if (resul.EsValido)
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
            else
            {
                return new Grilla<Docente>();
            }
        }

        public Institucion[] ObtenerInstituciones()
        {
            // https://docs.microsoft.com/en-us/dotnet/api/system.linq.enumerable.distinct?view=netcore-3.1
            // ver la clase institucion para ver los criterios de igualdad
            var listaDirectoras = Archivo.Instancia.Leer<LogicaDirectora>();
            var listaInstituciones = listaDirectoras.Select(x => x.Institucion);
            return AutoMapper.Instancia.ConvertirLista<LogicaInstitucion, Institucion>(listaInstituciones.Distinct().ToList()).ToArray();
        }

        public string ObtenerNombreGrupo()
        {
            return "Cominotti Mauro, Yacovino Juan, Zoja Emanuel";
        }

        public Padre ObtenerPadrePorId(UsuarioLogueado usuarioLogueado, int id)
        {
            LogicaPadre padre = Archivo.Instancia.Leer<LogicaPadre>().Where(x => x.Id == id).FirstOrDefault();
            var padree = AutoMapper.Instancia.Mapear<LogicaPadre, Padre>(padre);
            return padree;
        }

        public Grilla<Padre> ObtenerPadres(UsuarioLogueado usuarioLogueado, int paginaActual, int totalPorPagina, string busquedaGlobal)
        {
            Resultado resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado);
            if (resul.EsValido)
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
            else
            {
                return new Grilla<Padre>();
            }
        }

        public Hijo[] ObtenerPersonas(UsuarioLogueado usuarioLogueado)
        {
            throw new NotImplementedException();
        }

        public Sala[] ObtenerSalasPorInstitucion(UsuarioLogueado usuarioLogueado)
        {
            if (usuarioLogueado.RolSeleccionado == Roles.Directora)
            {
                List<LogicaSala> salas = Archivo.Instancia.Leer<LogicaSala>();
                if (salas == null)
                    return new Sala[0];
                else
                {
                    var salaas = AutoMapper.Instancia.Mapear< List<LogicaSala>, List<Sala> >(salas);
                    return salaas.ToArray();
                }
            }
            else
            {
                if (usuarioLogueado.RolSeleccionado == Roles.Docente)
                {
                    LogicaDocente docent = Archivo.Instancia.Leer<LogicaDocente>().Where(x => x.Nombre == usuarioLogueado.Nombre && x.Apellido == usuarioLogueado.Apellido &&
                        x.Email == usuarioLogueado.Email).FirstOrDefault();
                    List<LogicaSala> listasalas = docent.Salas.ToList();
                    var listasalass = AutoMapper.Instancia.Mapear<List<LogicaSala>, List<Sala>>(listasalas);
                    return listasalass.ToArray();
                }
                else
                    return null;
            }
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
