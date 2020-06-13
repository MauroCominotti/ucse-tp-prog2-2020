using System;
using System.Collections.Generic;
using System.Linq;
using Contratos;
using LogicaDeNegocios;

namespace Servicios
{
    public class GeneralService : IServicioWeb
    {
        public GeneralService()
        {
            Archivo.Instancia.eventoAlta += AltaRealizada;
            Archivo.Instancia.eventoBaja += BajaRealizada;
            Archivo.Instancia.eventoModificacion += ModificacionRealizada;
            Archivo.Instancia.eventoLectura += LecturaRealizada;
        }

        static void AltaRealizada(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"El Alta se realizó correctamente");
        }
        static void BajaRealizada(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"La baja se realizó correctamente");
        }
        static void ModificacionRealizada(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"La modificacion se realizó correctamente");
        }
        static void LecturaRealizada(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"La lectura se realizó en el archivo: {sender}");
        }

        public Resultado AltaAlumno(Hijo hijo, UsuarioLogueado usuarioLogueado)
        {
            Resultado resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            List<LogicaHijo> Alumno = Archivo.Instancia.Leer<LogicaHijo>();
            if (Alumno != null && Alumno.Count() > 0)
                hijo.Id = Archivo.Instancia.Leer<LogicaUsuario>().Count();
            else
                resul.Errores.Add("Error 404: Alumno no encontrado en la base de datos.");
            if (resul.EsValido)
            {
                var hijoCasteado = AutoMapper.Instancia.Mapear<Hijo, LogicaHijo>(hijo);
                var usuarioLogg = Archivo.Instancia.Leer<LogicaDirectora>().Find(x => x.Email == usuarioLogueado.Email && x.Eliminado == false);
                hijoCasteado.Eliminado = false;
                hijoCasteado.Password = "123"; // TODO > contraseña segura??
                hijoCasteado.IdInstitucion = usuarioLogg.IdInstitucion;
                hijoCasteado.Institucion = usuarioLogg.Institucion;
                hijoCasteado.Sala.IdInstitucion = usuarioLogg.IdInstitucion;
                Archivo.Instancia.Guardar(hijoCasteado);
            }
            return resul;
        }

        public Resultado AltaDirectora(Directora directora, UsuarioLogueado usuarioLogueado)
        {
            if (!Empresa.RegistroUsuario(directora.Email)) //Directora inexistente
            {
                //bandera = true;
                List<LogicaUsuario> usuarios = Archivo.Instancia.Leer<LogicaUsuario>().FindAll(x => x.Eliminado == false);
                if (usuarios != null && usuarios.Count() > 0)
                    directora.Id = usuarios.Count();
                else
                    directora.Id = 0;
            }
            Resultado ress = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado); // validamos los permisos
            if (ress.EsValido)
            {
                var directoraCasteada = AutoMapper.Instancia.Mapear<Directora, LogicaDirectora>(directora);
                var DirectoraJson = Archivo.Instancia.Leer<LogicaDirectora>().Find(x => x.Eliminado == false && x.Email == usuarioLogueado.Email);
                directoraCasteada.Eliminado = false;
                directoraCasteada.Password = "123";
                directoraCasteada.Institucion = DirectoraJson.Institucion;
                directoraCasteada.IdInstitucion = DirectoraJson.IdInstitucion;
                directoraCasteada.RolSeleccionado = Roles.Directora;
                directoraCasteada.Roles = new Roles[] { Roles.Directora };

                Archivo.Instancia.Guardar(directoraCasteada);
            }
            return ress;
        }

        public Resultado AltaDocente(Docente docente, UsuarioLogueado usuarioLogueado)
        {
            // TODO > Corregir todos los casos 1) agg resul error 403 - 2) error si ya se encuentra - 3) no hubo error al leer
            Resultado resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            List<LogicaUsuario> Docente = Archivo.Instancia.Leer<LogicaUsuario>();
            if (Docente != null) // no hubo ningun error al leer, puede ser q sea una lista vacia
            {
                if (Docente.Count() == 0)
                    docente.Id = 0;
                else
                    docente.Id = Docente.Count();
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
                    docenteCasteado.Eliminado = false;
                    docenteCasteado.Password = "123";
                    docenteCasteado.RolSeleccionado = Roles.Docente;
                    docenteCasteado.Roles = new Roles[] { Roles.Docente };
                    docenteCasteado.IdInstitucion = Archivo.Instancia.Leer<LogicaDirectora>().Find(x => x.Eliminado == false && x.Email == usuarioLogueado.Email).IdInstitucion;
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
            Resultado resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            var alumnosLogica = Archivo.Instancia.Leer<LogicaHijo>().FindAll(x => x.Eliminado == false);
            var notaMap = AutoMapper.Instancia.Mapear<Nota, LogicaNota>(nota);
            if (nota.Id == 0)
                notaMap.Id = null;
            var SalaId = salas.Select(x => x.Id).ToList();
            var alumnosSalas = alumnosLogica.FindAll(alumno => SalaId.Contains(alumno.Sala.Id));
            if (usuarioLogueado.RolSeleccionado == Roles.Directora)
            {
                if (hijos.Length != 0)
                {
                    foreach (var hijo in hijos)
                    {
                        Archivo.Instancia.Guardar(notaMap);
                        var hijoMap = AutoMapper.Instancia.Mapear<Hijo, LogicaHijo>(hijo);
                        hijoMap.Notas = new LogicaNota[] { Archivo.Instancia.Leer<LogicaNota>().Last() };
                        Archivo.Instancia.Guardar(hijoMap);
                    }
                }
                else
                {
                    foreach (var alumno in alumnosSalas)
                    {
                        Archivo.Instancia.Guardar(notaMap);
                        alumno.Notas = new LogicaNota[] { Archivo.Instancia.Leer<LogicaNota>().Last() };
                        Archivo.Instancia.Guardar(alumno);
                    }
                }
            }
            if (usuarioLogueado.RolSeleccionado == Roles.Docente)
            {
                var Docente= Archivo.Instancia.Leer<LogicaDocente>().Find(x => x.Email == usuarioLogueado.Email && x.Eliminado == false);
                var SalasDocente = Docente.Salas.Select(x => x.Id).ToList();
                var alumnosSalasDocente = alumnosSalas.FindAll(x => SalasDocente.Contains(x.Sala.Id));
                foreach (var alumno in alumnosSalasDocente)
                {
                    Archivo.Instancia.Guardar(notaMap);
                    alumno.Notas = new LogicaNota[] { Archivo.Instancia.Leer<LogicaNota>().Last() };
                    Archivo.Instancia.Guardar(alumno);
                }
            }
            if (usuarioLogueado.RolSeleccionado == Roles.Padre)
            {
                var padre = Archivo.Instancia.Leer<LogicaPadre>().Find(x => x.Email == usuarioLogueado.Email && x.Eliminado == false);
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
            List<LogicaUsuario> Padre = Archivo.Instancia.Leer<LogicaUsuario>();
            if (Padre != null && Padre.Count() > 0)
                padre.Id = Padre.Count();
            else
                resul.Errores.Add("Error 404: Padre no encontrado en la base de datos.");
            if (resul.EsValido)
            {
                var UsuarioJson = Archivo.Instancia.Leer<LogicaDirectora>().Find(x => x.Eliminado == false && x.Email == usuarioLogueado.Email);
                var padreCasteado = AutoMapper.Instancia.Mapear<Padre, LogicaPadre>(padre);
                padreCasteado.Password = "123";
                padreCasteado.Eliminado = false;
                padreCasteado.RolSeleccionado = Roles.Padre;
                padreCasteado.Roles = new Roles[] { Roles.Padre };
                padreCasteado.Roles = new Roles[] { Roles.Padre };
                padreCasteado.IdInstitucion = UsuarioJson.IdInstitucion;
                Archivo.Instancia.Guardar(padreCasteado);
            }
            return resul;
        }

        public Resultado AsignarDocenteSala(Docente docente, Sala sala, UsuarioLogueado usuarioLogueado)
        {
            var resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            if (resultado.EsValido)
            {
                if (Empresa.MismaInstitucion(docente.Id, usuarioLogueado.Email))
                {
                    var docenteLogica = AutoMapper.Instancia.Mapear<Docente, LogicaDocente>(docente);
                    var salasLogica = new LogicaSala[] { AutoMapper.Instancia.Mapear<Sala, LogicaSala>(sala) };
                    docenteLogica.Salas = salasLogica;
                    Archivo.Instancia.Guardar(docenteLogica);
                }
                else
                    resultado.Errores.Add("Error 403: Directora no pertenece a la misma institucion.");
            }
            return resultado;
        }

        public Resultado AsignarHijoPadre(Hijo hijo, Padre padre, UsuarioLogueado usuarioLogueado)
        {
            //El usuario debe ser directora, y el hijo debe estar asociado a una sala de su institucion
            var resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            if (resultado.EsValido)
            {
                if (Empresa.MismaInstitucion(hijo.Id, usuarioLogueado.Email))
                {
                    var padreLogica = AutoMapper.Instancia.Mapear<Padre, LogicaPadre>(padre);
                    var hijos = new LogicaHijo[] { AutoMapper.Instancia.Mapear<Hijo, LogicaHijo>(hijo) };
                    padreLogica.Hijos = hijos;
                    Archivo.Instancia.Guardar(padreLogica);
                }
                else
                    resultado.Errores.Add("Error 403: Padre no pertenece a la misma institucion.");
            }
            return resultado;
        }

        public Resultado DesasignarDocenteSala(Docente docente, Sala sala, UsuarioLogueado usuarioLogueado)
        {
            var resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            if (resultado.EsValido)
            {
                if (Empresa.MismaInstitucion(docente.Id, usuarioLogueado.Email))
                {
                    var docenteLogica = AutoMapper.Instancia.Mapear<Docente, LogicaDocente>(docente);
                    var salasLogica = new LogicaSala[] { };
                    docenteLogica.Salas = salasLogica;
                    Archivo.Instancia.Guardar(docenteLogica);
                }
                else
                    resultado.Errores.Add("Error 403: Docente no pertenece a la misma institucion.");
            }
            return resultado;
        }

        public Resultado DesasignarHijoPadre(Hijo hijo, Padre padre, UsuarioLogueado usuarioLogueado)
        {
            var resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            if (resultado.EsValido)
            {
                if (Empresa.MismaInstitucion(hijo.Id, usuarioLogueado.Email))
                {
                    var padreLogica = AutoMapper.Instancia.Mapear<Padre, LogicaPadre>(padre);
                    var hijos = new LogicaHijo[] { };
                    padreLogica.Hijos = hijos;
                    Archivo.Instancia.Guardar(padreLogica);
                }
                else
                    resultado.Errores.Add("Error 403: Alumno no pertenece a la misma institucion.");
            }
            return resultado;
        }

        public Resultado EditarAlumno(int id, Hijo hijo, UsuarioLogueado usuarioLogueado)
        {
            var resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);

            if (resultado.EsValido)
            {
                var alumnoEncontrado = Archivo.Instancia.Leer<LogicaHijo>().FirstOrDefault(x => x.Eliminado == false && x.Id == id);
                if (alumnoEncontrado != null)
                {
                    var hijoMapeado = AutoMapper.Instancia.Mapear<Hijo, LogicaHijo>(hijo);
                    hijoMapeado.Notas = new LogicaNota[] { };
                    Archivo.Instancia.Guardar(hijoMapeado);
                }
                else
                    resultado.Errores.Add("No existe la alumno");
            }
            else
                resultado.Errores.Add("No tiene permisos");
            return resultado;
        }

        public Resultado EditarDirectora(int id, Directora directora, UsuarioLogueado usuarioLogueado)
        {
            var resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);

            if (resultado.EsValido)
            {
                var directoraEncontrada = Archivo.Instancia.Leer<LogicaDirectora>().FirstOrDefault(x => x.Eliminado == false && x.Id == id);
                if (directoraEncontrada != null)
                    Archivo.Instancia.Guardar(AutoMapper.Instancia.Mapear<Directora, LogicaDirectora>(directora));
                else
                    resultado.Errores.Add("No existe la directora");
            }
            else
                resultado.Errores.Add("No tiene permisos");
            return resultado;
        }

        public Resultado EditarDocente(int id, Docente docente, UsuarioLogueado usuarioLogueado)
        {
            var resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);

            if (resultado.EsValido)
            {
                var docenteEncontrada = Archivo.Instancia.Leer<LogicaDocente>().FirstOrDefault(x => x.Eliminado == false && x.Id == id);
                if (docenteEncontrada != null)
                    Archivo.Instancia.Guardar(AutoMapper.Instancia.Mapear<Docente, LogicaDocente>(docente));
                else
                    resultado.Errores.Add("No existe la docente");
            }
            else
                resultado.Errores.Add("No tiene permisos");
            return resultado;
        }

        public Resultado EditarPadreMadre(int id, Padre padre, UsuarioLogueado usuarioLogueado)
        {
            var resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);

            if (resultado.EsValido)
            {
                var padreEncontrado = Archivo.Instancia.Leer<LogicaPadre>().FirstOrDefault(x => x.Eliminado == false && x.Id == id);
                if (padreEncontrado != null)
                    Archivo.Instancia.Guardar(AutoMapper.Instancia.Mapear<Padre, LogicaPadre>(padre));
                else
                    resultado.Errores.Add("No existe el padre");
            }
            else
                resultado.Errores.Add("No tiene permisos");
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
                LogicaHijo alumnoSeleccionado = Alumno.Find(x => x.Id == id && x.Eliminado == false);
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
                LogicaDirectora directoraSeleccionado = Directora.Find(x => x.Id == id && x.Eliminado == false);
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
                LogicaDocente docenteSeleccionado = Docente.Find(x => x.Id == id && x.Eliminado == false);
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
                LogicaPadre padreSeleccionado = Padre.Find(x => x.Id == id && x.Eliminado == false);
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
                //var LogicaPadre = Archivo.Instancia.Leer<LogicaPadre>().Find(x => x.Email == usuarioLogueado.Email && x.Eliminado == false && x.Nombre == usuarioLogueado.Nombre);
                var NotaLogica = Archivo.Instancia.Leer<LogicaNota>().Find(x => x.Id == nota.Id);
                var AlumnoLogica = Archivo.Instancia.Leer<LogicaHijo>().Find(x => x.Notas.ToList().Exists(y => y.Id == NotaLogica.Id) && x.Eliminado == false);
                NotaLogica.Leida = true; // TODO > no se actualiza la tabla en realidad
                Archivo.Instancia.Guardar(NotaLogica);
                AlumnoLogica.Notas.ToList().Find(x => x.Id == NotaLogica.Id).Leida = true;
                Archivo.Instancia.Guardar(AlumnoLogica);
            }
            else
                resultado.Errores.Add("No tiene permisos");
            return resultado;
        }

        public Hijo ObtenerAlumnoPorId(UsuarioLogueado usuarioLogueado, int id)
        {
            LogicaHijo alumno = Archivo.Instancia.Leer<LogicaHijo>().Where(x => x.Id == id && x.Eliminado == false).FirstOrDefault();
            var alumnoo = AutoMapper.Instancia.Mapear<LogicaHijo, Hijo>(alumno);
            return alumnoo;
        }

        public Grilla<Hijo> ObtenerAlumnos(UsuarioLogueado usuarioLogueado, int paginaActual, int totalPorPagina, string busquedaGlobal)
        {
            Resultado resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            if (resul.EsValido)
            {
                var id = Empresa.IDInstitucionUsuarioLogueado(usuarioLogueado.Email);
                List<LogicaHijo> lista = Archivo.Instancia.Leer<LogicaHijo>().FindAll(x => x.Eliminado == false && x.IdInstitucion == id)
                    .Where(x => string.IsNullOrEmpty(busquedaGlobal) || x.Nombre.Contains(busquedaGlobal) || x.Apellido.Contains(busquedaGlobal))
                    .ToList();
                var listaADevolver = AutoMapper.Instancia.ConvertirLista<LogicaHijo, Hijo>(lista.FindAll(x => x.Eliminado == false))
                    .Skip(paginaActual * totalPorPagina).Take(totalPorPagina).ToArray();
                //transformar el resultado de la logica de negocios a la clase de contratos
                return new Grilla<Hijo>()
                {
                    Lista = listaADevolver,
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
            var LogicaUsuario = Archivo.Instancia.Leer<LogicaUsuario>().Find(x => x.Email == usuarioLogueado.Email && x.Eliminado == false && x.Nombre == usuarioLogueado.Nombre);
            if (usuarioLogueado.RolSeleccionado == Roles.Directora)
            {
                var alumno = Archivo.Instancia.Leer<LogicaHijo>().Find(alm => alm.Id == idPersona && alm.Eliminado == false);
                return AutoMapper.Instancia.ConvertirLista<LogicaNota, Nota>(alumno.Notas.ToList()).ToArray();
            }

            if (LogicaUsuario.RolSeleccionado == Roles.Docente)
            {
                var LogicaDocente = Archivo.Instancia.Leer<LogicaDocente>().Find(x => x.Email == usuarioLogueado.Email && x.Eliminado == false && x.Nombre == usuarioLogueado.Nombre);
                var logicaSalasId = LogicaDocente.Salas.Select(x => x.Id).ToList();
                var AlumnosDelDocente = AutoMapper.Instancia.ConvertirLista<LogicaHijo, Hijo>(
                       Archivo.Instancia.Leer<LogicaHijo>().FindAll(al => al.IdInstitucion == LogicaDocente.IdInstitucion && al.Eliminado == false && logicaSalasId.Contains(al.Sala.Id))) // retorno los alumnos que estan en la institucion
                       .ToArray();
                return AlumnosDelDocente.Select(x => x.Notas).SelectMany(x => x).ToArray();
            }
            if (LogicaUsuario.RolSeleccionado == Roles.Padre)
            {
                var LogicaPadre = Archivo.Instancia.Leer<LogicaPadre>().Find(x => x.Email == usuarioLogueado.Email && x.Eliminado == false && x.Nombre == usuarioLogueado.Nombre);
                var LogicaHijoId = LogicaPadre.Hijos.Select(x => x.Id).ToList();
                var HijosDelPadre = AutoMapper.Instancia.ConvertirLista<LogicaHijo, Hijo>(
                       Archivo.Instancia.Leer<LogicaHijo>().FindAll(hijo => hijo.IdInstitucion == LogicaPadre.IdInstitucion && hijo.Eliminado == false && LogicaHijoId.Contains(hijo.Id))) // retorno los alumnos que estan en la institucion
                       .ToArray();
                var asd = HijosDelPadre.Select(x => x.Notas).SelectMany(x => x).ToArray();
                return asd;
            }
            return new Nota[] { };
            //if (usuarioLogueado.RolSeleccionado == Roles.Docente)
            //{
            //    var docente = Archivo.Instancia.Leer<LogicaDocente>().Find(x => x.Id == idPersona && x.Eliminado == false);
            //    var salasDocente = docente.Salas.ToList();
            //    var alumnosDocente = Archivo.Instancia.Leer<LogicaHijo>().FindAll(x => x.Eliminado == false).
            //        FindAll(alm => salasDocente.Exists(sala => sala.Id == alm.Sala.Id)); // TODO > ver si funciona

            //    foreach (var alumno in alumnosDocente)
            //    {
            //        var notasALM = alumno.Notas.ToList();
            //        var notasMapeadas = AutoMapper.Instancia.ConvertirLista<LogicaNota, Nota>(notasALM);
            //        notas.AddRange(notasMapeadas);
            //    }

            //}
            //if (usuarioLogueado.RolSeleccionado == Roles.Padre)
            //{
            //    var padre = Archivo.Instancia.Leer<LogicaPadre>().Find(x => x.Id == idPersona && x.Eliminado == false);
            //    foreach (var hijo in padre.Hijos)
            //    {
            //        var notasALM = hijo.Notas.ToList();
            //        var notasMapeadas = AutoMapper.Instancia.ConvertirLista<LogicaNota, Nota>(notasALM);
            //        notas.AddRange(notasMapeadas);
            //    }
            //}

            //return notas.ToArray();
        }

        public Directora ObtenerDirectoraPorId(UsuarioLogueado usuarioLogueado, int id)
        {

            LogicaDirectora directora = Archivo.Instancia.Leer<LogicaDirectora>().Where(x => x.Id == id && x.Eliminado == false).FirstOrDefault();
            var directoraa = AutoMapper.Instancia.Mapear<LogicaDirectora, Directora>(directora);
            return directoraa;
        }

        public Grilla<Directora> ObtenerDirectoras(UsuarioLogueado usuarioLogueado, int paginaActual, int totalPorPagina, string busquedaGlobal)
        {
            Resultado resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            if (resul.EsValido)
            {
                var id = Empresa.IDInstitucionUsuarioLogueado(usuarioLogueado.Email);
                List<LogicaDirectora> lista = Archivo.Instancia.Leer<LogicaDirectora>().FindAll(x => x.Eliminado == false && x.IdInstitucion == id)
                    .Where(x => string.IsNullOrEmpty(busquedaGlobal) || x.Nombre.Contains(busquedaGlobal) || x.Apellido.Contains(busquedaGlobal))
                    .ToList();
                var listaADevolver = AutoMapper.Instancia.ConvertirLista<LogicaDirectora, Directora>(lista)
                    .Skip(paginaActual * totalPorPagina).Take(totalPorPagina).ToArray();
                //transformar el resultado de la logica de negocios a la clase de contratos
                return new Grilla<Directora>()
                {
                    Lista = listaADevolver,
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

            LogicaDocente docente = Archivo.Instancia.Leer<LogicaDocente>().Where(x => x.Id == id && x.Eliminado == false).FirstOrDefault();
            var docentee = AutoMapper.Instancia.Mapear<LogicaDocente, Docente>(docente);
            return docentee;

        }

        public Grilla<Docente> ObtenerDocentes(UsuarioLogueado usuarioLogueado, int paginaActual, int totalPorPagina, string busquedaGlobal)
        {
            Resultado resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            if (resul.EsValido)
            {
                var id = Empresa.IDInstitucionUsuarioLogueado(usuarioLogueado.Email);
                List<LogicaDocente> lista = Archivo.Instancia.Leer<LogicaDocente>().FindAll(x => x.Eliminado == false && x.IdInstitucion == id)
                    .Where(x => string.IsNullOrEmpty(busquedaGlobal) || x.Nombre.Contains(busquedaGlobal) || x.Apellido.Contains(busquedaGlobal))
                    .ToList();
                var listaADevolver = AutoMapper.Instancia.ConvertirLista<LogicaDocente, Docente>(lista)
                    .Skip(paginaActual * totalPorPagina).Take(totalPorPagina).ToArray();
                //transformar el resultado de la logica de negocios a la clase de contratos
                return new Grilla<Docente>()
                {
                    Lista = listaADevolver,
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
            var listaDirectoras = Archivo.Instancia.Leer<LogicaDirectora>().FindAll(x => x.Eliminado == false);
            var listaInstituciones = listaDirectoras.Select(x => x.Institucion);
            return AutoMapper.Instancia.ConvertirLista<LogicaInstitucion, Institucion>(listaInstituciones.Distinct().ToList()).ToArray();
        }

        public string ObtenerNombreGrupo()
        {
            //// las instituciones no se pueden agregar, no hay un metodo AltaInstitucion
            //List<LogicaInstitucion> _instituciones = new List<LogicaInstitucion>() // institucion no tiene id=0 por Empresa.IDInstitucionUsuarioLogueado
            //{
            //new LogicaInstitucion(){ Id = 1, Ciudad = "Rafaela", Direccion = "Ituzaingo 403", Nombre = "Misericordia", Provincia = "Santa Fe", Telefono = "03492565890", Eliminado = false},
            //new LogicaInstitucion(){ Id = 2, Ciudad = "Rafaela", Direccion = "Colon 403", Nombre = "San Jose", Provincia = "Santa Fe", Telefono = "03492565890", Eliminado = false},
            //new LogicaInstitucion(){ Id = 3, Ciudad = "Rafaela", Direccion = "Saavedra 403", Nombre = "Normal", Provincia = "Santa Fe", Telefono = "03492565890", Eliminado = false},
            //};

            //List<LogicaDirectora> _directoras = new List<LogicaDirectora>()
            //{
            //new LogicaDirectora(){ Id = 0, Nombre = "A 0", Apellido ="B", Email = "C", Roles = new Roles[] { Roles.Directora }, RolSeleccionado = Roles.Directora, FechaIngreso = DateTime.Now, Eliminado = false, IdInstitucion = 1, Password = "123", Cargo = "D", Institucion = _instituciones[0]},
            //new LogicaDirectora(){ Id = 1, Nombre = "A 1", Apellido ="B", Email = "C", Roles = new Roles[] { Roles.Directora }, RolSeleccionado = Roles.Directora, FechaIngreso = DateTime.Now, Eliminado = false, IdInstitucion = 1, Password = "123", Cargo = "D", Institucion = _instituciones[0]},new LogicaDirectora(){ Id = 2, Nombre = "A 2", Apellido ="B", Email = "C", Roles = new Roles[] { Roles.Directora }, RolSeleccionado = Roles.Directora, FechaIngreso = DateTime.Now, Eliminado = false, IdInstitucion = 2, Password = "123", Cargo = "D", Institucion = _instituciones[0]},
            //new LogicaDirectora(){ Id = 3, Nombre = "A 3", Apellido ="B", Email = "C", Roles = new Roles[] { Roles.Directora }, RolSeleccionado = Roles.Directora, FechaIngreso = DateTime.Now, Eliminado = false, IdInstitucion = 1, Password = "123", Cargo = "D", Institucion = _instituciones[0]},new LogicaDirectora(){ Id = 4, Nombre = "A 4", Apellido ="B", Email = "C", Roles = new Roles[] { Roles.Directora }, RolSeleccionado = Roles.Directora, FechaIngreso = DateTime.Now, Eliminado = false, IdInstitucion = 2, Password = "123", Cargo = "D", Institucion = _instituciones[0]},
            //new LogicaDirectora(){ Id = 5, Nombre = "A 5", Apellido ="B", Email = "C", Roles = new Roles[] { Roles.Directora }, RolSeleccionado = Roles.Directora, FechaIngreso = DateTime.Now, Eliminado = false, IdInstitucion = 1, Password = "123", Cargo = "D", Institucion = _instituciones[0]},new LogicaDirectora(){ Id = 6, Nombre = "A 6", Apellido ="B", Email = "C", Roles = new Roles[] { Roles.Directora }, RolSeleccionado = Roles.Directora, FechaIngreso = DateTime.Now, Eliminado = false, IdInstitucion = 2, Password = "123", Cargo = "D", Institucion = _instituciones[0]},
            //new LogicaDirectora(){ Id = 7, Nombre = "A 7", Apellido ="B", Email = "C", Roles = new Roles[] { Roles.Directora }, RolSeleccionado = Roles.Directora, FechaIngreso = DateTime.Now, Eliminado = false, IdInstitucion = 1, Password = "123", Cargo = "D", Institucion = _instituciones[0]},new LogicaDirectora(){ Id = 8, Nombre = "A 8", Apellido ="B", Email = "C", Roles = new Roles[] { Roles.Directora }, RolSeleccionado = Roles.Directora, FechaIngreso = DateTime.Now, Eliminado = false, IdInstitucion = 2, Password = "123", Cargo = "D", Institucion = _instituciones[0]},
            //new LogicaDirectora(){ Id = 9, Nombre = "A 9", Apellido ="B", Email = "C", Roles = new Roles[] { Roles.Directora }, RolSeleccionado = Roles.Directora, FechaIngreso = DateTime.Now, Eliminado = false, IdInstitucion = 1, Password = "123", Cargo = "D", Institucion = _instituciones[0]},new LogicaDirectora(){ Id = 10, Nombre = "A 10", Apellido ="B", Email = "C", Roles = new Roles[] { Roles.Directora }, RolSeleccionado = Roles.Directora, FechaIngreso = DateTime.Now, Eliminado = false, IdInstitucion = 2, Password = "123", Cargo = "D", Institucion = _instituciones[0]},
            //new LogicaDirectora(){ Id = 11, Nombre = "A 11", Apellido ="B", Email = "C", Roles = new Roles[] { Roles.Directora }, RolSeleccionado = Roles.Directora, FechaIngreso = DateTime.Now, Eliminado = false, IdInstitucion = 1, Password = "123", Cargo = "D", Institucion = _instituciones[1]},new LogicaDirectora(){ Id = 12, Nombre = "A 12", Apellido ="B", Email = "C", Roles = new Roles[] { Roles.Directora }, RolSeleccionado = Roles.Directora, FechaIngreso = DateTime.Now, Eliminado = false, IdInstitucion = 2, Password = "123", Cargo = "D", Institucion = _instituciones[2]},
            //};

            //List<LogicaSala> _salas = new List<LogicaSala>()
            //{
            //    new LogicaSala(){ Id = 0, Nombre = "Historia", IdInstitucion = 1, Eliminado = false },
            //    new LogicaSala(){ Id = 1, Nombre = "Matematica", IdInstitucion = 1, Eliminado = false },
            //    new LogicaSala(){ Id = 2, Nombre = "Lengua", IdInstitucion = 1, Eliminado = false },
            //    new LogicaSala(){ Id = 3, Nombre = "Programacion", IdInstitucion = 1, Eliminado = false },
            //};

            //List<LogicaDocente> _docentes = new List<LogicaDocente>()
            //{
            //new LogicaDocente(){ Id = 13, Nombre = "D 1", Apellido ="DA 1", Eliminado = false, IdInstitucion = 1, Email = "DE 1",  Password = "123", Roles = new Roles[] { Roles.Docente }, RolSeleccionado = Roles.Docente, Salas = new LogicaSala[] { _salas[0] }},new LogicaDocente(){ Id = 14, Nombre = "D 2", Apellido ="DA 2", Eliminado = false, IdInstitucion = 1, Email = "DE 2", Password = "123", Roles = new Roles[] { Roles.Docente }, RolSeleccionado = Roles.Docente, Salas = new LogicaSala[] { _salas[0] } },
            //new LogicaDocente(){ Id = 15, Nombre = "D 3", Apellido ="DA 3", Eliminado = false, IdInstitucion = 1, Email = "DE 3",  Password = "123", Roles = new Roles[] { Roles.Docente }, RolSeleccionado = Roles.Docente, Salas = new LogicaSala[] { _salas[0] }},new LogicaDocente(){ Id = 16, Nombre = "D 4", Apellido ="DA 4", Eliminado = false, IdInstitucion = 1, Email = "DE 4",  Password = "123", Roles = new Roles[] { Roles.Docente }, RolSeleccionado = Roles.Docente, Salas = new LogicaSala[] { _salas[0] }},
            //new LogicaDocente(){ Id = 17, Nombre = "D 5", Apellido ="DA 5", Eliminado = false, IdInstitucion = 1, Email = "DE 5",  Password = "123", Roles = new Roles[] { Roles.Docente }, RolSeleccionado = Roles.Docente, Salas = new LogicaSala[] { _salas[0] }},new LogicaDocente(){ Id = 18, Nombre = "D 6", Apellido ="DA 6", Eliminado = false, IdInstitucion = 1, Email = "DE 6",  Password = "123", Roles = new Roles[] { Roles.Docente }, RolSeleccionado = Roles.Docente, Salas = new LogicaSala[] { _salas[0] }},
            //new LogicaDocente(){ Id = 19, Nombre = "D 7", Apellido ="DA 7", Eliminado = false, IdInstitucion = 1, Email = "DE 7",  Password = "123", Roles = new Roles[] { Roles.Docente }, RolSeleccionado = Roles.Docente, Salas = new LogicaSala[] { _salas[1] }},new LogicaDocente(){ Id = 20, Nombre = "D 8", Apellido ="DA 8", Eliminado = false, IdInstitucion = 1, Email = "DE 8",  Password = "123", Roles = new Roles[] { Roles.Docente }, RolSeleccionado = Roles.Docente, Salas = new LogicaSala[] { _salas[1] }},
            //new LogicaDocente(){ Id = 21, Nombre = "D 9", Apellido ="DA 9", Eliminado = false, IdInstitucion = 2, Email = "DE 9",  Password = "123", Roles = new Roles[] { Roles.Docente }, RolSeleccionado = Roles.Docente, Salas = new LogicaSala[] { _salas[1] }},new LogicaDocente(){ Id = 22, Nombre = "D 10", Apellido ="DA 10", Eliminado = false, IdInstitucion = 2, Email = "DE 10",  Password = "123", Roles = new Roles[] { Roles.Docente }, RolSeleccionado = Roles.Docente, Salas = new LogicaSala[] { _salas[1] }},
            //new LogicaDocente(){ Id = 23, Nombre = "D 11", Apellido ="DA 11", Eliminado = false, IdInstitucion = 2, Email = "DE 11",  Password = "123", Roles = new Roles[] { Roles.Docente }, RolSeleccionado = Roles.Docente, Salas = new LogicaSala[] { _salas[2] }},new LogicaDocente(){ Id = 24, Nombre = "D 12", Apellido ="DA 12", Eliminado = false, IdInstitucion = 2, Email = "DE 12",  Password = "123", Roles = new Roles[] { Roles.Docente }, RolSeleccionado = Roles.Docente, Salas = new LogicaSala[] { _salas[2] }},
            //new LogicaDocente(){ Id = 25, Nombre = "D 13", Apellido ="DA 13", Eliminado = false, IdInstitucion = 2, Email = "DE 13",  Password = "123", Roles = new Roles[] { Roles.Docente }, RolSeleccionado = Roles.Docente, Salas = new LogicaSala[] { _salas[2] }},new LogicaDocente(){ Id = 26, Nombre = "D 14", Apellido ="DA 14", Eliminado = false, IdInstitucion = 2, Email = "DE 14",  Password = "123", Roles = new Roles[] { Roles.Docente }, RolSeleccionado = Roles.Docente, Salas = new LogicaSala[] { _salas[2] }},
            //new LogicaDocente(){ Id = 27, Nombre = "D 15", Apellido ="DA 15", Eliminado = false, IdInstitucion = 2, Email = "DE 15",  Password = "123", Roles = new Roles[] { Roles.Docente }, RolSeleccionado = Roles.Docente, Salas = new LogicaSala[] { _salas[2] }},new LogicaDocente(){ Id = 28, Nombre = "D 16", Apellido ="DA 16", Eliminado = false, IdInstitucion = 2, Email = "DE 16",  Password = "123", Roles = new Roles[] { Roles.Docente }, RolSeleccionado = Roles.Docente, Salas = new LogicaSala[] { _salas[2] }},
            //};

            //List<LogicaNota> _notas1 = new List<LogicaNota>()
            //{
            //new LogicaNota(){ Id = 0, Leida = false, Titulo= "LogicaNota 0", Descripcion = "Descripcion de la nota 0", Comentarios = new LogicaComentario[]{ } },
            //new LogicaNota(){ Id = 1, Leida = false, Titulo= "LogicaNota 1", Descripcion = "Descripcion de la nota 1", Comentarios = new LogicaComentario[]{ } },
            //new LogicaNota(){ Id = 2, Leida = false, Titulo= "LogicaNota 2", Descripcion = "Descripcion de la nota 2", Comentarios = new LogicaComentario[]{
            //    new LogicaComentario() { Fecha = DateTime.Now.AddDays(-2), Mensaje = "LogicaComentario 1" , Usuario = new LogicaUsuario(){ Nombre = "Usuario", Apellido="Cualquiera" } },
            //    new LogicaComentario() { Fecha = DateTime.Now.AddDays(-1), Mensaje = "LogicaComentario 2" , Usuario = new LogicaUsuario(){ Nombre = "Usuario", Apellido="Cualquiera 2" } },
            //    } }
            //};

            //List<LogicaNota> _notas2 = new List<LogicaNota>()
            //{
            //    new LogicaNota(){ Id = 3, Leida = true, Titulo= "LogicaNota 3", Descripcion = "Descripcion de la nota 3", Comentarios = new LogicaComentario[]{ } },
            //};

            //List<LogicaNota> _notas3 = new List<LogicaNota>()
            //{
            //    new LogicaNota(){ Id = 4, Leida = false, Titulo= "LogicaNota 4", Descripcion = "Descripcion de la nota 4", Comentarios = new LogicaComentario[]{ } },
            //};

            //List<LogicaNota> _notas4 = new List<LogicaNota>()
            //{
            //    new LogicaNota(){ Id = 5, Leida = true, Titulo= "LogicaNota 5", Descripcion = "Descripcion de la nota 5", Comentarios = new LogicaComentario[]{ } },
            //};

            //List<LogicaHijo> _alumnos = new List<LogicaHijo>()
            //{
            //new LogicaHijo(){ Id = 33, Nombre = "AL 1", Apellido="AP 1", IdInstitucion = 1, Eliminado = false, Email="APE 1", FechaNacimiento = new DateTime(1990,5,4), ResultadoUltimaEvaluacionAnual = 10, Sala = _salas[0], Notas = _notas1.ToArray(), Institucion = _instituciones[0], Password = "123", Roles = null, RolSeleccionado = null},
            //new LogicaHijo(){ Id = 34, Nombre = "AL 2", Apellido="AP 2", IdInstitucion = 1, Eliminado = false, Email="APE 2", FechaNacimiento = new DateTime(1991,3,20), ResultadoUltimaEvaluacionAnual = 6, Sala = _salas[0], Notas = _notas2.ToArray(), Institucion = _instituciones[0], Password = "123", Roles = null, RolSeleccionado = null},
            //new LogicaHijo(){ Id = 35, Nombre = "AL 3", Apellido="AP 3", IdInstitucion = 1, Eliminado = false, Email="APE 3", FechaNacimiento = new DateTime(1992,12,14), ResultadoUltimaEvaluacionAnual = 5, Sala = _salas[0], Notas = _notas3.ToArray(), Institucion = _instituciones[0], Password = "123", Roles = null, RolSeleccionado = null},
            //new LogicaHijo(){ Id = 36, Nombre = "AL 4", Apellido="AP 4", IdInstitucion = 1, Eliminado = false, Email="APE 4", FechaNacimiento = new DateTime(1989,11,29), ResultadoUltimaEvaluacionAnual = 3, Sala = _salas[0], Notas = _notas4.ToArray(), Institucion = _instituciones[0], Password = "123", Roles = null, RolSeleccionado = null},
            //new LogicaHijo(){ Id = 37, Nombre = "AL 5", Apellido="AP 5", IdInstitucion = 2, Eliminado = false, Email="APE 1", FechaNacimiento = new DateTime(1990,5,4), ResultadoUltimaEvaluacionAnual = 10, Sala = _salas[1], Notas = _notas1.ToArray(), Institucion = _instituciones[1], Password = "123", Roles = null, RolSeleccionado = null},
            //new LogicaHijo(){ Id = 38, Nombre = "AL 6", Apellido="AP 6", IdInstitucion = 2, Eliminado = false, Email="APE 2", FechaNacimiento = new DateTime(1991,3,20), ResultadoUltimaEvaluacionAnual = 6, Sala = _salas[1], Notas = _notas2.ToArray(), Institucion = _instituciones[1], Password = "123", Roles = null, RolSeleccionado = null},
            //new LogicaHijo(){ Id = 39, Nombre = "AL 7", Apellido="AP 7", IdInstitucion = 3, Eliminado = false, Email="APE 1", FechaNacimiento = new DateTime(1990,5,4), ResultadoUltimaEvaluacionAnual = 10, Sala = _salas[2], Notas = _notas1.ToArray(), Institucion = _instituciones[2], Password = "123", Roles = null, RolSeleccionado = null},
            //new LogicaHijo(){ Id = 40, Nombre = "AL 8", Apellido="AP 8", IdInstitucion = 3, Eliminado = false, Email="APE 2", FechaNacimiento = new DateTime(1991,3,20), ResultadoUltimaEvaluacionAnual = 6, Sala = _salas[2], Notas = _notas2.ToArray(), Institucion = _instituciones[2], Password = "123", Roles = null, RolSeleccionado = null},
            //};

            //List<LogicaPadre> _padres = new List<LogicaPadre>()
            //{
            //new LogicaPadre(){ Id = 29, Nombre = "P 1", Apellido = "PA 1", Eliminado = false, Hijos = new LogicaHijo[] { _alumnos[0] }, Password = "123", Roles = new Roles[] { Roles.Padre }, RolSeleccionado = Roles.Padre, IdInstitucion = 1, Email = "PE 1"},
            //new LogicaPadre(){ Id = 30, Nombre = "P 2", Apellido = "PA 2", Eliminado = false, Hijos = new LogicaHijo[] { _alumnos[1] }, IdInstitucion = 1, Email = "PE 2", Password = "123", Roles = new Roles[] { Roles.Padre }, RolSeleccionado = Roles.Padre },
            //new LogicaPadre(){ Id = 31, Nombre = "P 3", Apellido = "PA 3", Eliminado = false, Hijos = new LogicaHijo[] { _alumnos[2] }, Password = "123", Roles = new Roles[] { Roles.Padre }, RolSeleccionado = Roles.Padre, IdInstitucion = 1, Email = "PE 3"},
            //new LogicaPadre(){ Id = 32, Nombre = "P 4", Apellido = "PA 4", Eliminado = false, Hijos = new LogicaHijo[] { _alumnos[3] }, IdInstitucion = 2, Email = "PE 4"},
            //new LogicaPadre(){ Id = 41, Nombre = "P 5", Apellido = "PA 5", Eliminado = false, Hijos = new LogicaHijo[] { _alumnos[4] }, IdInstitucion = 2, Email = "PE 4"},
            //};

            //_directoras.ForEach(x => Archivo.Instancia.Guardar(AutoMapper.Instancia.Mapear<Directora, LogicaDirectora>(x)));
            //_docentes.ForEach(x => Archivo.Instancia.Guardar(AutoMapper.Instancia.Mapear<Docente, LogicaDocente>(x)));
            //_padres.ForEach(x => Archivo.Instancia.Guardar(AutoMapper.Instancia.Mapear<Padre, LogicaPadre>(x)));
            ////_notas1.ForEach(x => Archivo.Instancia.Guardar(AutoMapper.Instancia.Mapear<Nota, LogicaNota>(x)));
            //_alumnos.ForEach(x => Archivo.Instancia.Guardar(AutoMapper.Instancia.Mapear<Hijo, LogicaHijo>(x)));
            ////_salas.ForEach(x => Archivo.Instancia.Guardar(AutoMapper.Instancia.Mapear<Sala, LogicaSala>(x)));

            //_directoras.ForEach(x => Archivo.Instancia.Guardar(x));
            //_salas.ForEach(x => Archivo.Instancia.Guardar(x));
            //_docentes.ForEach(x => Archivo.Instancia.Guardar(x));
            //_padres.ForEach(x => Archivo.Instancia.Guardar(x));
            //_instituciones.ForEach(x => Archivo.Instancia.Guardar(x));
            //_notas1.ForEach(x => Archivo.Instancia.Guardar(x));
            //_notas2.ForEach(x => Archivo.Instancia.Guardar(x));
            //_notas3.ForEach(x => Archivo.Instancia.Guardar(x));
            //_notas4.ForEach(x => Archivo.Instancia.Guardar(x));
            //_alumnos.ForEach(x => Archivo.Instancia.Guardar(x));


            return "Cominotti Mauro, Yacovino Juan, Zoja Emanuel";
        }

        public Padre ObtenerPadrePorId(UsuarioLogueado usuarioLogueado, int id)
        {
            LogicaPadre padre = Archivo.Instancia.Leer<LogicaPadre>().Where(x => x.Id == id && x.Eliminado == false).FirstOrDefault();
            var padree = AutoMapper.Instancia.Mapear<LogicaPadre, Padre>(padre);
            return padree;
        }

        public Grilla<Padre> ObtenerPadres(UsuarioLogueado usuarioLogueado, int paginaActual, int totalPorPagina, string busquedaGlobal)
        {
            Resultado resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            if (resul.EsValido)
            {
                var id = Empresa.IDInstitucionUsuarioLogueado(usuarioLogueado.Email);
                List<LogicaPadre> lista = Archivo.Instancia.Leer<LogicaPadre>().FindAll(x => x.Eliminado == false && x.IdInstitucion == id)
                    .Where(x => string.IsNullOrEmpty(busquedaGlobal) || x.Nombre.Contains(busquedaGlobal) || x.Apellido.Contains(busquedaGlobal))
                    .ToList();
                var listaADevolver = AutoMapper.Instancia.ConvertirLista<LogicaPadre, Padre>(lista)
                    .Skip(paginaActual * totalPorPagina).Take(totalPorPagina).ToArray();
                //transformar el resultado de la logica de negocios a la clase de contratos
                return new Grilla<Padre>()
                {
                    Lista = listaADevolver,
                    CantidadLogicaUsuario = lista.Count()
                };
            }
            else
            {
                return new Grilla<Padre>();
            }
        }

        public Hijo[] ObtenerPersonas(UsuarioLogueado usuarioLogueado) // TODO > Funciones ObtenerSalasPorInstitucion, ObtenerPersonas
        {
            Resultado resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            var LogicaUsuario = Archivo.Instancia.Leer<LogicaUsuario>().Find(x => x.Email == usuarioLogueado.Email && x.Eliminado == false && x.Nombre == usuarioLogueado.Nombre);
            if (resul.EsValido)
                return AutoMapper.Instancia.ConvertirLista<LogicaHijo, Hijo>(
                    Archivo.Instancia.Leer<LogicaHijo>().FindAll(x => x.IdInstitucion == LogicaUsuario.IdInstitucion && x.Eliminado == false)) // retorno los alumnos que estan en la institucion
                    .ToArray();
            if (LogicaUsuario.RolSeleccionado == Roles.Docente)
            {
                var LogicaDocente = Archivo.Instancia.Leer<LogicaDocente>().Find(x => x.Email == usuarioLogueado.Email && x.Eliminado == false && x.Nombre == usuarioLogueado.Nombre);
                var logicaSalasId = LogicaDocente.Salas.Select(x => x.Id).ToList();
                return AutoMapper.Instancia.ConvertirLista<LogicaHijo, Hijo>(
                       Archivo.Instancia.Leer<LogicaHijo>().FindAll(al => al.IdInstitucion == LogicaDocente.IdInstitucion && al.Eliminado == false && logicaSalasId.Contains(al.Sala.Id))) // retorno los alumnos que estan en la institucion
                       .ToArray();
            }
            if (LogicaUsuario.RolSeleccionado == Roles.Padre)
            {
                var LogicaPadre = Archivo.Instancia.Leer<LogicaPadre>().Find(x => x.Email == usuarioLogueado.Email && x.Eliminado == false && x.Nombre == usuarioLogueado.Nombre);
                var LogicaHijoId = LogicaPadre.Hijos.Select(x => x.Id).ToList();
                return AutoMapper.Instancia.ConvertirLista<LogicaHijo, Hijo>(
                       Archivo.Instancia.Leer<LogicaHijo>().FindAll(hijo => hijo.IdInstitucion == LogicaPadre.IdInstitucion && hijo.Eliminado == false && LogicaHijoId.Contains(hijo.Id))) // retorno los alumnos que estan en la institucion
                       .ToArray();
            }
            return new Hijo[] { };
        }

        public Sala[] ObtenerSalasPorInstitucion(UsuarioLogueado usuarioLogueado)
        {
            if (usuarioLogueado.RolSeleccionado == Roles.Directora)
            {
                var user = Archivo.Instancia.Leer<LogicaDirectora>().Find(y => y.Eliminado == false && y.Email == usuarioLogueado.Email);
                List<LogicaSala> salas = Archivo.Instancia.Leer<LogicaSala>().FindAll(x =>
                x.Eliminado == false &&
                x.IdInstitucion == user.IdInstitucion);
                return salas == null ?
                    new Sala[0] :
                    AutoMapper.Instancia.ConvertirLista<LogicaSala, Sala>(salas).ToArray(); // TODO > Listas dentro de listas??
            }
            else
            {
                if (usuarioLogueado.RolSeleccionado == Roles.Docente)
                {
                    LogicaDocente docent = Archivo.Instancia.Leer<LogicaDocente>().Where(
                        x => x.Nombre == usuarioLogueado.Nombre &&
                        x.Apellido == usuarioLogueado.Apellido &&
                        x.Email == usuarioLogueado.Email &&
                        x.Eliminado == false)
                        .FirstOrDefault();
                    List<LogicaSala> listasalas = docent.Salas.ToList();
                    return AutoMapper.Instancia.ConvertirLista<LogicaSala, Sala>(listasalas).ToArray();
                }
                else
                    return null;
            }
        }

        public UsuarioLogueado ObtenerUsuario(string email, string clave)
        {
            return AutoMapper.Instancia.Mapear<LogicaUsuario, UsuarioLogueado>(Archivo.Instancia.Leer<LogicaUsuario>().Find(x =>
                                                                                x.Email == email &&
                                                                                x.Password == clave &&
                                                                                x.Eliminado == false));
        }

        public Resultado ResponderNota(Nota nota, Comentario nuevoComentario, UsuarioLogueado usuarioLogueado)
        {
            Resultado resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            var notaLogica = Archivo.Instancia.Leer<LogicaNota>().Find(x => x.Id == nota.Id);
            var alumno = Archivo.Instancia.Leer<LogicaHijo>().Find(x => x.Notas.Contains(notaLogica) && x.Eliminado == false);
            var nuevoComentarioLogica = AutoMapper.Instancia.Mapear<Comentario, LogicaComentario>(nuevoComentario);
            if (resul.EsValido)
            {
                if (Empresa.MismaInstitucion(alumno.Id, usuarioLogueado.Email))
                {
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
                    var docente = Archivo.Instancia.Leer<LogicaDocente>().Find(x => x.Email == usuarioLogueado.Email && x.Eliminado == false);
                    if (Empresa.MismaInstitucion(alumno.Id, usuarioLogueado.Email) && docente.Salas.Contains(alumno.Sala)) // si van a la misma institucion y a la misma sala
                    {
                        alumno.Notas.ToList().Find(x => x == notaLogica).Comentarios = new LogicaComentario[] { nuevoComentarioLogica };
                        Archivo.Instancia.Guardar(alumno);
                    }
                    else
                        resul.Errores.Add("Error 403: Directora no pertenece a la misma institucion.");
                }

                if ((int)usuarioLogueado.RolSeleccionado == 0)
                {
                    var padre = Archivo.Instancia.Leer<LogicaPadre>().Find(x => x.Email == usuarioLogueado.Email && x.Eliminado == false);
                    if (Empresa.MismaInstitucion(alumno.Id, usuarioLogueado.Email) && padre.Hijos.ToList().Contains(alumno)) // si es su hijo
                    {
                        alumno.Notas.ToList().Find(x => x == notaLogica).Comentarios = new LogicaComentario[] { nuevoComentarioLogica };
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
