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
            Resultado resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            List<LogicaHijo> Alumno = Archivo.Instancia.Leer<LogicaHijo>();
            if (Alumno != null || Alumno.Count() > 0)
                hijo.Id = Alumno.LastOrDefault().Id + 1;
            else
                resul.Errores.Add("Error 404: Alumno no encontrado en la base de datos.");
            if (resul.EsValido)
            {
                var hijoCasteado = AutoMapper.Instancia.Mapear<Hijo, LogicaHijo>(hijo);
                Archivo.Instancia.Guardar(hijoCasteado);
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
                LogicaUsuario regis = Archivo.Instancia.Leer<LogicaUsuario>().Where(x => x.Email == directora.Email).FirstOrDefault();
                regis.Roles.ToList().Add(usuarioLogueado.RolSeleccionado);
                Archivo.Instancia.Guardar(regis);
            }
            Resultado ress = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado); // validamos los permisos
            if (ress.EsValido)
            {
                var directoraCasteada = AutoMapper.Instancia.Mapear<Directora, LogicaDirectora>(directora);
                Archivo.Instancia.Guardar(directoraCasteada);
                if (bandera)
                {
                    List<Roles> roles = new List<Roles>();
                    roles.Add(Roles.Directora);
                    LogicaUsuario nuevoregistro = new LogicaUsuario()
                    {
                        Email = directora.Email,
                        Id = directora.Id,
                        Password = new Random().Next(100000, 999999).ToString(),
                        Roles = roles.ToArray(),
                    };
                    Archivo.Instancia.Guardar(nuevoregistro);
                }
            }
            return ress;
        }

        public Resultado AltaDocente(Docente docente, UsuarioLogueado usuarioLogueado)
        {
            // TODO > Corregir todos los casos 1) agg resul error 403 - 2) error si ya se encuentra - 3) no hubo error al leer
            Resultado resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
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
                if (Docente.Find(x => x.Id == docente.Id) == null)
                    resul.Errores.Add("Error 404: Docente ya se encuentra en la base de datos.");
                else
                {
                    var docenteCasteado = AutoMapper.Instancia.Mapear<Docente, LogicaDocente>(docente);
                    Archivo.Instancia.Guardar(docenteCasteado);
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
                        Archivo.Instancia.Guardar(hijoMap);
                    }
                }
                if (salas != null)
                {
                    foreach (var alumno in alumnosSalas)
                    {
                        alumno.Notas.ToList().Add(notaMap);
                        Archivo.Instancia.Guardar(alumno);
                    }
                }
            }
            if (usuarioLogueado.RolSeleccionado == Roles.Docente)
            {
                foreach (var alumno in alumnosSalas)
                {
                    alumno.Notas.ToList().Add(notaMap);
                    Archivo.Instancia.Guardar(alumno);
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
                    Archivo.Instancia.Guardar(padre);
                }
            }
            return resultado;
        }

        public Resultado AltaPadreMadre(Padre padre, UsuarioLogueado usuarioLogueado)
        {
            Resultado resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            List<LogicaPadre> Padre = Archivo.Instancia.Leer<LogicaPadre>();
            if (Padre != null || Padre.Count() > 0)
                padre.Id = Padre.LastOrDefault().Id + 1;
            else
                resul.Errores.Add("Error 404: Padre no encontrado en la base de datos.");
            if (resul.EsValido)
            {
                var padreCasteado = AutoMapper.Instancia.Mapear<Padre, LogicaPadre>(padre);
                Archivo.Instancia.Guardar(padreCasteado);
            }
            return resul;
        }

        public Resultado AsignarDocenteSala(Docente docente, Sala sala, UsuarioLogueado usuarioLogueado)
        {
            Resultado nuevardo = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            if (nuevardo.EsValido)
            {
                if (docente.Salas == null)
                {
                    docente.Salas = new Sala[] { sala };
                    var docenteCasteado = AutoMapper.Instancia.Mapear<Docente, LogicaDocente>(docente);
                    Archivo.Instancia.Guardar(docenteCasteado);
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
                        Archivo.Instancia.Guardar(docenteCasteado);
                    }
                }
            }
            return nuevardo;
        }

        public Resultado AsignarHijoPadre(Hijo hijo, Padre padre, UsuarioLogueado usuarioLogueado)
        {
            //El usuario debe ser directora, y el hijo debe estar asociado a una sala de su institucion
            var resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            if (resultado.EsValido)
            {
                if (Empresa.MismaInstitucion(hijo.Id, usuarioLogueado.Email))
                {
                    padre.Hijos.ToList().Add(hijo);
                    Archivo.Instancia.Guardar(AutoMapper.Instancia.Mapear<Padre, LogicaPadre>(padre));
                }
                else
                    resultado.Errores.Add("Error 403: Directora no pertenece a la misma institucion.");
            }
            return resultado;
        }

        public Resultado DesasignarDocenteSala(Docente docente, Sala sala, UsuarioLogueado usuarioLogueado)
        {
            Resultado resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            if (resultado.EsValido)
            {
                if (Empresa.MismaInstitucion(docente.Id, usuarioLogueado.Email)) // TODO > Refactorizar el resto del codigo
                {
                    docente.Salas.ToList().Remove(sala);
                    Archivo.Instancia.Guardar(AutoMapper.Instancia.Mapear<Docente, LogicaDocente>(docente));
                }
                else
                    resultado.Errores.Add("Error 403: Directora no pertenece a la misma institucion.");
            }
            return resultado;
        }

        public Resultado DesasignarHijoPadre(Hijo hijo, Padre padre, UsuarioLogueado usuarioLogueado)
        {
            var resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            if (resultado.EsValido)
            {
                var padreLogica = Archivo.Instancia.Leer<LogicaPadre>().Find(x => x.Id == padre.Id);
                padreLogica.Hijos.ToList().RemoveAll(hij => hij.Id == hijo.Id);
                Archivo.Instancia.Guardar(padreLogica);
            }
            else
            {
                resultado.Errores.Add("Error 403: El usuario no tiene los permisos suficientes.");
            }

            return resultado;
        }

        public Resultado EditarAlumno(int id, Hijo hijo, UsuarioLogueado usuarioLogueado)
        {
            Resultado resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
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
                            Archivo.Instancia.Guardar(item);
                        }
                    }
                    var hijoo = AutoMapper.Instancia.Mapear<Hijo, LogicaHijo>(hijo);
                    Archivo.Instancia.Guardar(hijoo);
                }
                else
                {
                    var hijo2 = AutoMapper.Instancia.Mapear<Hijo, LogicaHijo>(hijo);
                    Archivo.Instancia.Guardar(hijo2);
                }
            }
            return resultado;
        }

        public Resultado EditarDirectora(int id, Directora directora, UsuarioLogueado usuarioLogueado)
        {
            var resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);

            if (resultado.EsValido)
            {
                var directoraMapeada = AutoMapper.Instancia.Mapear<Directora, LogicaDirectora>(directora);
                var directoras = Archivo.Instancia.Leer<LogicaDirectora>();
                var directoraEncontrada = directoras.Find(dir => dir.Id == id);
                if (directoraEncontrada != null)
                {
                    directoraEncontrada.Eliminado = true;
                    Archivo.Instancia.Guardar(directoraEncontrada);
                    Archivo.Instancia.Guardar(directoraMapeada);
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
            var resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);

            if (resultado.EsValido)
            {
                var docenteMapeada = AutoMapper.Instancia.Mapear<Docente, LogicaDocente>(docente);
                var docentes = Archivo.Instancia.Leer<LogicaDirectora>();
                var docenteEncontrada = docentes.Find(x => x.Id == id);
                if (docenteEncontrada != null)
                {
                    docenteEncontrada.Eliminado = true;
                    Archivo.Instancia.Guardar(docenteEncontrada);
                    Archivo.Instancia.Guardar(docenteMapeada);
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
            var resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);

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
            Resultado resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
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
            Resultado resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
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
            Resultado resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
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
            Resultado resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
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
                nota.Leida = true; // TODO > no se actualiza la tabla en realidad
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
            Resultado resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            if (resul.EsValido)
            {
                List<LogicaHijo> lista = Archivo.Instancia.Leer<LogicaHijo>();
                //transformar el resultado de la logica de negocios a la clase de contratos
                return new Grilla<Hijo>()
                {
                    Lista = AutoMapper.Instancia.ConvertirLista<LogicaHijo, Hijo>(lista)
                    .Where(x => string.IsNullOrEmpty(busquedaGlobal) || x.Nombre.Contains(busquedaGlobal) || x.Apellido.Contains(busquedaGlobal))
                    .Skip(paginaActual * totalPorPagina).Take(totalPorPagina).ToArray(),
                    CantidadLogicaUsuario = lista.Count()
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
            Resultado resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            if (resul.EsValido)
            {
                List<LogicaDirectora> lista = Archivo.Instancia.Leer<LogicaDirectora>();
                //transformar el resultado de la logica de negocios a la clase de contratos
                return new Grilla<Directora>()
                {
                    Lista = AutoMapper.Instancia.ConvertirLista<LogicaDirectora, Directora>(lista)
                    .Where(x => string.IsNullOrEmpty(busquedaGlobal) || x.Nombre.Contains(busquedaGlobal) || x.Apellido.Contains(busquedaGlobal))
                    .Skip(paginaActual * totalPorPagina).Take(totalPorPagina).ToArray(),
                    CantidadLogicaUsuario = lista.Count()
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
            Resultado resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            if (resul.EsValido)
            {
                List<LogicaDocente> lista = Archivo.Instancia.Leer<LogicaDocente>();
                //transformar el resultado de la logica de negocios a la clase de contratos
                return new Grilla<Docente>()
                {
                    Lista = AutoMapper.Instancia.ConvertirLista<LogicaDocente, Docente>(lista)
                    .Where(x => string.IsNullOrEmpty(busquedaGlobal) || x.Nombre.Contains(busquedaGlobal) || x.Apellido.Contains(busquedaGlobal))
                    .Skip(paginaActual * totalPorPagina).Take(totalPorPagina).ToArray(),
                    CantidadLogicaUsuario = lista.Count()
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
            List<LogicaDirectora> _directoras = new List<LogicaDirectora>()
            {
            new LogicaDirectora(){ Id = 1, Nombre = "A 1", Apellido ="B", Email = "C", Eliminado = false, IdInstitucion = 1, Password = "123", Cargo = "D"},new LogicaDirectora(){ Id = 2, Nombre = "A 2", Apellido ="B", Email = "C", Eliminado = false, IdInstitucion = 2, Password = "123", Cargo = "D"},
            new LogicaDirectora(){ Id = 3, Nombre = "A 3", Apellido ="B", Email = "C", Eliminado = false, IdInstitucion = 1, Password = "123", Cargo = "D"},new LogicaDirectora(){ Id = 4, Nombre = "A 4", Apellido ="B", Email = "C", Eliminado = false, IdInstitucion = 2, Password = "123", Cargo = "D"},
            new LogicaDirectora(){ Id = 5, Nombre = "A 5", Apellido ="B", Email = "C", Eliminado = false, IdInstitucion = 1, Password = "123", Cargo = "D"},new LogicaDirectora(){ Id = 6, Nombre = "A 6", Apellido ="B", Email = "C", Eliminado = false, IdInstitucion = 2, Password = "123", Cargo = "D"},
            new LogicaDirectora(){ Id = 7, Nombre = "A 7", Apellido ="B", Email = "C", Eliminado = false, IdInstitucion = 1, Password = "123", Cargo = "D"},new LogicaDirectora(){ Id = 8, Nombre = "A 8", Apellido ="B", Email = "C", Eliminado = false, IdInstitucion = 2, Password = "123", Cargo = "D"},
            new LogicaDirectora(){ Id = 9, Nombre = "A 9", Apellido ="B", Email = "C", Eliminado = false, IdInstitucion = 1, Password = "123", Cargo = "D"},new LogicaDirectora(){ Id = 10, Nombre = "A 10", Apellido ="B", Email = "C", Eliminado = false, IdInstitucion = 2, Password = "123", Cargo = "D"},
            new LogicaDirectora(){ Id = 11, Nombre = "A 11", Apellido ="B", Email = "C", Eliminado = false, IdInstitucion = 1, Password = "123", Cargo = "D"},new LogicaDirectora(){ Id = 12, Nombre = "A 12", Apellido ="B", Email = "C", Eliminado = false, IdInstitucion = 2, Password = "123", Cargo = "D"},
            };

            List<LogicaInstitucion> _instituciones = new List<LogicaInstitucion>()
            {
            new LogicaInstitucion(){ Id = 1, Ciudad = "Rafaela", Direccion = "Cosettini 403", Nombre = "Misericordia", Provincia = "Santa Fe", Telefono = "03492565890", Eliminado = false},
            new LogicaInstitucion(){ Id = 2, Ciudad = "Rafaela", Direccion = "Cosettini 403", Nombre = "Misericordia", Provincia = "Santa Fe", Telefono = "03492565890", Eliminado = false},
            };

            List<LogicaDocente> _docentes = new List<LogicaDocente>()
            {
            new LogicaDocente(){ Id = 1, Nombre = "D 1", Apellido ="DA 1", Eliminado = false, IdInstitucion = 1, Email = "DE 1"},new LogicaDocente(){ Id = 2, Nombre = "D 2", Apellido ="DA 2", Eliminado = false, IdInstitucion = 1, Email = "DE 2"},
            new LogicaDocente(){ Id = 3, Nombre = "D 3", Apellido ="DA 3", Eliminado = false, IdInstitucion = 1, Email = "DE 3"},new LogicaDocente(){ Id = 4, Nombre = "D 4", Apellido ="DA 4", Eliminado = false, IdInstitucion = 1, Email = "DE 4"},
            new LogicaDocente(){ Id = 5, Nombre = "D 5", Apellido ="DA 5", Eliminado = false, IdInstitucion = 1, Email = "DE 5"},new LogicaDocente(){ Id = 6, Nombre = "D 6", Apellido ="DA 6", Eliminado = false, IdInstitucion = 1, Email = "DE 6"},
            new LogicaDocente(){ Id = 7, Nombre = "D 7", Apellido ="DA 7", Eliminado = false, IdInstitucion = 1, Email = "DE 7"},new LogicaDocente(){ Id = 8, Nombre = "D 8", Apellido ="DA 8", Eliminado = false, IdInstitucion = 1, Email = "DE 8"},
            new LogicaDocente(){ Id = 9, Nombre = "D 9", Apellido ="DA 9", Eliminado = false, IdInstitucion = 2, Email = "DE 9"},new LogicaDocente(){ Id = 10, Nombre = "D 10", Apellido ="DA 10", Eliminado = false, IdInstitucion = 2, Email = "DE 10"},
            new LogicaDocente(){ Id = 11, Nombre = "D 11", Apellido ="DA 11", Eliminado = false, IdInstitucion = 2, Email = "DE 11"},new LogicaDocente(){ Id = 12, Nombre = "D 12", Apellido ="DA 12", Eliminado = false, IdInstitucion = 2, Email = "DE 12"},
            new LogicaDocente(){ Id = 13, Nombre = "D 13", Apellido ="DA 13", Eliminado = false, IdInstitucion = 2, Email = "DE 13"},new LogicaDocente(){ Id = 14, Nombre = "D 14", Apellido ="DA 14", Eliminado = false, IdInstitucion = 2, Email = "DE 14"},
            new LogicaDocente(){ Id = 15, Nombre = "D 15", Apellido ="DA 15", Eliminado = false, IdInstitucion = 2, Email = "DE 15"},new LogicaDocente(){ Id = 16, Nombre = "D 16", Apellido ="DA 16", Eliminado = false, IdInstitucion = 2, Email = "DE 16"},
            };

            List<LogicaNota> _notas1 = new List<LogicaNota>()
            {
            new LogicaNota(){ Id = 1, Leida = false, Titulo= "LogicaNota 1", Descripcion = "Descripcion de la nota 1", Comentarios = new LogicaComentario[]{ } },
            new LogicaNota(){ Id = 2, Leida = false, Titulo= "LogicaNota 2", Descripcion = "Descripcion de la nota 2", Comentarios = new LogicaComentario[]{
                new LogicaComentario() { Fecha = DateTime.Now.AddDays(-2), Mensaje = "LogicaComentario 1" , Usuario = new LogicaUsuario(){ Nombre = "Usuario", Apellido="Cualquiera" } },
                new LogicaComentario() { Fecha = DateTime.Now.AddDays(-1), Mensaje = "LogicaComentario 2" , Usuario = new LogicaUsuario(){ Nombre = "Usuario", Apellido="Cualquiera 2" } },
                } }
            };

            List<LogicaNota> _notas2 = new List<LogicaNota>()
            {
                new LogicaNota(){ Id = 3, Leida = true, Titulo= "LogicaNota 3", Descripcion = "Descripcion de la nota 3", Comentarios = new LogicaComentario[]{ } },
            };

            List<LogicaNota> _notas3 = new List<LogicaNota>()
            {
                new LogicaNota(){ Id = 4, Leida = false, Titulo= "LogicaNota 4", Descripcion = "Descripcion de la nota 4", Comentarios = new LogicaComentario[]{ } },
            };

            List<LogicaNota> _notas4 = new List<LogicaNota>()
            {
                new LogicaNota(){ Id = 5, Leida = true, Titulo= "LogicaNota 5", Descripcion = "Descripcion de la nota 5", Comentarios = new LogicaComentario[]{ } },
            };

            List<LogicaHijo> _alumnos = new List<LogicaHijo>()
            {
            new LogicaHijo(){ Id = 1, Nombre = "AL 1", Apellido="AP 1", IdInstitucion = 1, Eliminado = false, Email="APE 1", FechaNacimiento = new DateTime(1990,5,4), ResultadoUltimaEvaluacionAnual = 10, Sala = new LogicaSala(){ Id = 1 }, Notas = _notas1.ToArray() },
            new LogicaHijo(){ Id = 2, Nombre = "AL 2", Apellido="AP 2", IdInstitucion = 1, Eliminado = false, Email="APE 2", FechaNacimiento = new DateTime(1991,3,20), ResultadoUltimaEvaluacionAnual = 6, Sala = new LogicaSala(){ Id = 2 }, Notas = _notas2.ToArray()},
            new LogicaHijo(){ Id = 3, Nombre = "AL 3", Apellido="AP 3", IdInstitucion = 2, Eliminado = false, Email="APE 3", FechaNacimiento = new DateTime(1992,12,14), ResultadoUltimaEvaluacionAnual = 5, Sala = new LogicaSala(){ Id = 2 }, Notas = _notas3.ToArray()},
            new LogicaHijo(){ Id = 4, Nombre = "AL 4", Apellido="AP 4", IdInstitucion = 2, Eliminado = false, Email="APE 4", FechaNacimiento = new DateTime(1989,11,29), ResultadoUltimaEvaluacionAnual = 3, Sala = new LogicaSala(){ Id = 3 }, Notas = _notas4.ToArray()},
            };

            List<Sala> _salas = new List<Sala>()
            {
                new Sala(){ Id = 1, Nombre = "Sala 1" },
                new Sala(){ Id = 2, Nombre = "Sala 2" },
                new Sala(){ Id = 3, Nombre = "Sala 3" },
            };

            List<LogicaPadre> _padres = new List<LogicaPadre>()
            {
            new LogicaPadre(){ Id = 1, Nombre = "P 1", Apellido = "PA 1", Eliminado = false, Hijos = new LogicaHijo[] { new LogicaHijo(){ Id = 1, Nombre = "AL 1", Apellido="AP 1", IdInstitucion = 1, Eliminado = false, Email="APE 1", FechaNacimiento = new DateTime(1990,5,4), ResultadoUltimaEvaluacionAnual = 10, Sala = new LogicaSala(){ Id = 1 }, Notas = _notas1.ToArray() } }, Password = "123", Roles = new Roles[] { Roles.Padre }, RolSeleccionado = Roles.Padre, IdInstitucion = 1, Email = "PE 1"},
            new LogicaPadre(){ Id = 2, Nombre = "P 2", Apellido = "PA 2", Eliminado = false, IdInstitucion = 1, Email = "PE 2"},
            new LogicaPadre(){ Id = 3, Nombre = "P 3", Apellido = "PA 3", Eliminado = false, Hijos = new LogicaHijo[] { new LogicaHijo(){ Id = 1, Nombre = "AL 1", Apellido="AP 1", IdInstitucion = 1, Eliminado = false, Email="APE 1", FechaNacimiento = new DateTime(1990,5,4), ResultadoUltimaEvaluacionAnual = 10, Sala = new LogicaSala(){ Id = 1 }, Notas = _notas1.ToArray() } }, Password = "123", Roles = new Roles[] { Roles.Padre }, RolSeleccionado = Roles.Padre, IdInstitucion = 1, Email = "PE 3"},
            new LogicaPadre(){ Id = 4, Nombre = "P 4", Apellido = "PA 4", Eliminado = false, IdInstitucion = 1, Email = "PE 4"},
            };

            //_directoras.ForEach(x => Archivo.Instancia.Guardar(AutoMapper.Instancia.Mapear<Directora, LogicaDirectora>(x)));
            //_docentes.ForEach(x => Archivo.Instancia.Guardar(AutoMapper.Instancia.Mapear<Docente, LogicaDocente>(x)));
            //_padres.ForEach(x => Archivo.Instancia.Guardar(AutoMapper.Instancia.Mapear<Padre, LogicaPadre>(x)));
            ////_notas1.ForEach(x => Archivo.Instancia.Guardar(AutoMapper.Instancia.Mapear<Nota, LogicaNota>(x)));
            //_alumnos.ForEach(x => Archivo.Instancia.Guardar(AutoMapper.Instancia.Mapear<Hijo, LogicaHijo>(x)));
            ////_salas.ForEach(x => Archivo.Instancia.Guardar(AutoMapper.Instancia.Mapear<Sala, LogicaSala>(x)));

            _directoras.ForEach(x => Archivo.Instancia.Guardar(x));
            _docentes.ForEach(x => Archivo.Instancia.Guardar(x));
            _padres.ForEach(x => Archivo.Instancia.Guardar(x));
            _instituciones.ForEach(x => Archivo.Instancia.Guardar(x));
            //_notas1.ForEach(x => Archivo.Instancia.Guardar());
            _alumnos.ForEach(x => Archivo.Instancia.Guardar(x));


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
            Resultado resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            if (resul.EsValido)
            {
                List<LogicaPadre> lista = Archivo.Instancia.Leer<LogicaPadre>();
                //transformar el resultado de la logica de negocios a la clase de contratos
                return new Grilla<Padre>()
                {
                    Lista = AutoMapper.Instancia.ConvertirLista<LogicaPadre, Padre>(lista)
                    .Where(x => string.IsNullOrEmpty(busquedaGlobal) || x.Nombre.Contains(busquedaGlobal) || x.Apellido.Contains(busquedaGlobal))
                    .Skip(paginaActual * totalPorPagina).Take(totalPorPagina).ToArray(),
                    CantidadLogicaUsuario = lista.Count()
                };
            }
            else
            {
                return new Grilla<Padre>();
            }
        }

        public Hijo[] ObtenerPersonas(UsuarioLogueado usuarioLogueado)
        {
            Resultado resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            var institucionUsuario = Archivo.Instancia.Leer<LogicaUsuario>().First(x => x.Email == usuarioLogueado.Email).IdInstitucion; // encuentro la institucion del usuario
            if (resul.EsValido)
                return AutoMapper.Instancia.ConvertirLista<LogicaHijo, Hijo>(
                    Archivo.Instancia.Leer<LogicaHijo>().FindAll(x => x.IdInstitucion == institucionUsuario)) // retorno los alumnos que estan en la institucion
                    .ToArray();
            else
                return new Hijo[] { };
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
                    var salaas = AutoMapper.Instancia.Mapear<List<LogicaSala>, List<Sala>>(salas); // TODO > Listas dentro de listas??
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
            return AutoMapper.Instancia.Mapear<LogicaUsuario, UsuarioLogueado>(Archivo.Instancia.Leer<LogicaUsuario>().Find(x =>
                                                                                x.Email == email &&
                                                                                x.Password == clave));
        }

        public Resultado ResponderNota(Nota nota, Comentario nuevoComentario, UsuarioLogueado usuarioLogueado)
        {
            Resultado resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            var notaLogica = AutoMapper.Instancia.Mapear<Nota, LogicaNota>(nota);
            var alumno = Archivo.Instancia.Leer<LogicaHijo>().Find(x => x.Notas.Contains(notaLogica));
            if (resul.EsValido)
            {
                if (Empresa.MismaInstitucion(alumno.Id, usuarioLogueado.Email))
                {
                    var nuevoComentarioLogica = AutoMapper.Instancia.Mapear<Comentario, LogicaComentario>(nuevoComentario);
                    alumno.Notas.ToList().Find(x => x == notaLogica).Comentarios.ToList().Add(nuevoComentarioLogica);
                    Archivo.Instancia.Guardar(alumno);
                }
                else
                    resul.Errores.Add("Error 403: Directora no pertenece a la misma institucion.");
            }
            else
            {
                if ((int)usuarioLogueado.RolSeleccionado == 2)
                {
                    var docente = Archivo.Instancia.Leer<LogicaDocente>().Find(x => x.Email == usuarioLogueado.Email);
                    if (Empresa.MismaInstitucion(alumno.Id, usuarioLogueado.Email) && docente.Salas.Contains(alumno.Sala)) // si van a la misma institucion y a la misma sala
                    {
                        var nuevoComentarioLogica = AutoMapper.Instancia.Mapear<Comentario, LogicaComentario>(nuevoComentario);
                        alumno.Notas.ToList().Find(x => x == notaLogica).Comentarios.ToList().Add(nuevoComentarioLogica);
                        Archivo.Instancia.Guardar(alumno);
                    }
                    else
                        resul.Errores.Add("Error 403: Directora no pertenece a la misma institucion.");
                }

                if ((int)usuarioLogueado.RolSeleccionado == 0)
                {
                    var padre = Archivo.Instancia.Leer<LogicaPadre>().Find(x => x.Email == usuarioLogueado.Email);
                    if (Empresa.MismaInstitucion(alumno.Id, usuarioLogueado.Email) && padre.Hijos.ToList().Contains(alumno)) // si es su hijo
                    {
                        var nuevoComentarioLogica = AutoMapper.Instancia.Mapear<Comentario, LogicaComentario>(nuevoComentario);
                        alumno.Notas.ToList().Find(x => x == notaLogica).Comentarios.ToList().Add(nuevoComentarioLogica);
                        Archivo.Instancia.Guardar(alumno);
                    }
                    else
                        resul.Errores.Add("Error 403: Directora no pertenece a la misma institucion.");
                }
            }
            return resul;
        }
    }
}
