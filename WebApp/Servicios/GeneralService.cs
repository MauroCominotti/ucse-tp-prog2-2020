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
            Resultado Resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            List<LogicaUsuario> Usuarios = Archivo.Instancia.Leer<LogicaUsuario>().FindAll(x => x.Eliminado == false);
            if (Usuarios != null) // no hubo ningun error al leer, puede ser q sea una Lista vacia
            {
                if (Usuarios.Count() == 0)
                    hijo.Id = 1;
                else
                    hijo.Id = Usuarios.Count() + 1;
            }
            else
                Resultado.Errores.Add("Error 404: Alumno no encontrado en la base de datos.");
            if (Resultado.EsValido)
            {
                if (Usuarios.Find(x => x.Id == hijo.Id && x.Email == hijo.Email) != null)
                    Resultado.Errores.Add("Error 404: Alumno ya se encuentra en la base de datos.");
                else
                {
                    var HijoCasteado = AutoMapper.Instancia.Mapear<Hijo, LogicaHijo>(hijo);
                    var UsuarioLogg = Archivo.Instancia.Leer<LogicaDirectora>().Find(x => x.Email == usuarioLogueado.Email && x.Eliminado == false);
                    HijoCasteado.Eliminado = false;
                    HijoCasteado.Password = "123";
                    HijoCasteado.IdInstitucion = UsuarioLogg.IdInstitucion;
                    HijoCasteado.Institucion = UsuarioLogg.Institucion;
                    HijoCasteado.Sala.IdInstitucion = UsuarioLogg.IdInstitucion;
                    Archivo.Instancia.Guardar(HijoCasteado);
                }
            }
            else
                Resultado.Errores.Add("Error 403: Alumno no tiene los permisos suficientes.");
            return Resultado;
        }

        public Resultado AltaDirectora(Directora directora, UsuarioLogueado usuarioLogueado)
        {
            Resultado Resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            List<LogicaUsuario> Usuarios = Archivo.Instancia.Leer<LogicaUsuario>().FindAll(x => x.Eliminado == false);
            if (Usuarios != null)
            {
                if (Usuarios.Count() == 0)
                    directora.Id = 1;
                else
                    directora.Id = Usuarios.Count() + 1;
            }
            else
                Resultado.Errores.Add("Error 404: Directora no encontrada en la base de datos.");
            if (Resultado.EsValido)
            {
                if (Usuarios.Find(x => x.Id == directora.Id && x.Email == directora.Email) != null)
                    Resultado.Errores.Add("Error 404: Directora ya se encuentra en la base de datos.");
                else
                {
                    var DirectoraCasteada = AutoMapper.Instancia.Mapear<Directora, LogicaDirectora>(directora);
                    var DirectoraLogueada = Archivo.Instancia.Leer<LogicaDirectora>().Find(x => x.Eliminado == false && x.Email == usuarioLogueado.Email);
                    DirectoraCasteada.Eliminado = false;
                    DirectoraCasteada.Password = "123";
                    DirectoraCasteada.Institucion = DirectoraLogueada.Institucion;
                    DirectoraCasteada.IdInstitucion = DirectoraLogueada.IdInstitucion;
                    DirectoraCasteada.RolSeleccionado = Roles.Directora;
                    DirectoraCasteada.Roles = new Roles[] { Roles.Directora };

                    Archivo.Instancia.Guardar(DirectoraCasteada);
                }
            }
            else
                Resultado.Errores.Add("Error 403: Directora no tiene los permisos suficientes.");
            return Resultado;
        }

        public Resultado AltaDocente(Docente docente, UsuarioLogueado usuarioLogueado)
        {
            Resultado Resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            List<LogicaUsuario> Usuarios = Archivo.Instancia.Leer<LogicaUsuario>().FindAll(x => x.Eliminado == false);
            if (Usuarios != null)
            {
                if (Usuarios.Count() == 0)
                    docente.Id = 1;
                else
                    docente.Id = Usuarios.Count() + 1;
            }
            else
                Resultado.Errores.Add("Error 404: Docente no encontrado en la base de datos.");
            if (Resultado.EsValido)
            {
                if (Usuarios.Find(x => x.Id == docente.Id && x.Email == docente.Email) != null)
                    Resultado.Errores.Add("Error 404: Docente ya se encuentra en la base de datos.");
                else
                {
                    var DocenteCasteado = AutoMapper.Instancia.Mapear<Docente, LogicaDocente>(docente);
                    DocenteCasteado.Eliminado = false;
                    DocenteCasteado.Password = "123";
                    DocenteCasteado.RolSeleccionado = Roles.Docente;
                    DocenteCasteado.Roles = new Roles[] { Roles.Docente };
                    DocenteCasteado.IdInstitucion = Archivo.Instancia.Leer<LogicaDirectora>().Find(x => x.Eliminado == false && x.Email == usuarioLogueado.Email).IdInstitucion;
                    Archivo.Instancia.Guardar(DocenteCasteado);
                }
            }
            else
                Resultado.Errores.Add("Error 403: Docente no tiene los permisos suficientes.");
            return Resultado;
        }

        public Resultado AltaNota(Nota nota, Sala[] salas, Hijo[] hijos, UsuarioLogueado usuarioLogueado)
        {
            Resultado Resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            var LogicaUsuario = Archivo.Instancia.Leer<LogicaUsuario>().Find(x => x.Email == usuarioLogueado.Email && x.Eliminado == false && x.Nombre == usuarioLogueado.Nombre);
            var AlumnosLogica = Archivo.Instancia.Leer<LogicaHijo>().FindAll(x => x.Eliminado == false);
            var NotaMap = AutoMapper.Instancia.Mapear<Nota, LogicaNota>(nota);
            var SalaId = salas.Select(x => x.Id).ToList();
            var AlumnosSalas = AlumnosLogica.FindAll(alumno => SalaId.Contains(alumno.Sala.Id));
            if (hijos.Length != 0 && (LogicaUsuario.RolSeleccionado == Roles.Directora || LogicaUsuario.RolSeleccionado == Roles.Docente || LogicaUsuario.RolSeleccionado == Roles.Padre))
            {
                try
                {
                    foreach (var Hijo in hijos)
                    {
                        NotaMap.Id = null;
                        Archivo.Instancia.Guardar(NotaMap);
                        var HijoMap = AlumnosLogica.Find(x => x.Id == Hijo.Id);
                        HijoMap.Notas.Add(Archivo.Instancia.Leer<LogicaNota>().Last());
                        Archivo.Instancia.Guardar(HijoMap);
                        var Padres = Archivo.Instancia.Leer<LogicaPadre>().FindAll(x => x.Hijos.Exists(y => y.Id == HijoMap.Id) && x.Eliminado == false);
                        foreach (var p in Padres)
                        {
                            p.Hijos.RemoveAll(x => x.Id == HijoMap.Id);
                            p.Hijos.Add(HijoMap);
                            Archivo.Instancia.Guardar(p);
                        }
                    }
                }
                catch (Exception)
                {
                    Resultado.Errores.Add("Error 404: Alta Nota no se pudo realizar correctamente.");
                }
                return Resultado;
            }
            else
            {
                try
                {
                    if (LogicaUsuario.RolSeleccionado == Roles.Directora || LogicaUsuario.RolSeleccionado == Roles.Docente)
                    {
                        foreach (var Alumno in AlumnosSalas)
                        {
                            NotaMap.Id = null;
                            Archivo.Instancia.Guardar(NotaMap);
                            Alumno.Notas.Add(Archivo.Instancia.Leer<LogicaNota>().Last());
                            Archivo.Instancia.Guardar(Alumno);
                            var Padres = Archivo.Instancia.Leer<LogicaPadre>().FindAll(x => x.Hijos.Exists(y => y.Id == Alumno.Id) && x.Eliminado == false);
                            foreach (var p in Padres)
                            {
                                p.Hijos.RemoveAll(x => x.Id == Alumno.Id);
                                p.Hijos.Add(Alumno);
                                Archivo.Instancia.Guardar(p);
                            }
                        }
                    }
                    if (LogicaUsuario.RolSeleccionado == Roles.Padre) // Agregada funcionalidad para el padre
                    {
                        var Padre = Archivo.Instancia.Leer<LogicaPadre>().Find(x => x.Email == usuarioLogueado.Email && x.Eliminado == false);
                        var HijosId = Padre.Hijos.Select(x => x.Id).ToList();
                        var HijosLogica = Archivo.Instancia.Leer<LogicaHijo>().FindAll(x => HijosId.Contains(x.Id) && x.Eliminado == false);
                        foreach (var Hijo in HijosLogica)
                        {
                            NotaMap.Id = null;
                            Archivo.Instancia.Guardar(NotaMap);
                            Hijo.Notas.Add(Archivo.Instancia.Leer<LogicaNota>().Last());
                            Archivo.Instancia.Guardar(Hijo);
                            var Padres = Archivo.Instancia.Leer<LogicaPadre>().FindAll(x => x.Hijos.Exists(y => y.Id == Hijo.Id) && x.Eliminado == false);
                            foreach (var p in Padres)
                            {
                                p.Hijos.RemoveAll(x => x.Id == Hijo.Id);
                                p.Hijos.Add(Hijo);
                                Archivo.Instancia.Guardar(p);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    Resultado.Errores.Add("Error 404: Alta Nota no se pudo realizar correctamente.");
                }
            }
            return Resultado;
        }

        public Resultado AltaPadreMadre(Padre padre, UsuarioLogueado usuarioLogueado)
        {
            Resultado Resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            List<LogicaUsuario> Usuarios = Archivo.Instancia.Leer<LogicaUsuario>();
            if (Usuarios != null) // no hubo ningun error al leer, puede ser q sea una Lista vacia
            {
                if (Usuarios.Count() == 0)
                    padre.Id = 1;
                else
                    padre.Id = Usuarios.Count() + 1;
            }
            else
                Resultado.Errores.Add("Error 404: Padre no encontrado en la base de datos.");
            if (Resultado.EsValido)
            {
                if (Usuarios.Find(x => x.Id == padre.Id && x.Email == padre.Email) != null)
                    Resultado.Errores.Add("Error 404: Padre ya se encuentra en la base de datos.");
                else
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
            }
            else
                Resultado.Errores.Add("Error 403: Padre no tiene los permisos suficientes.");
            return Resultado;
        }

        public Resultado AsignarDocenteSala(Docente docente, Sala sala, UsuarioLogueado usuarioLogueado)
        {
            var Resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            if (Resultado.EsValido)
            {
                if (Empresa.MismaInstitucion(docente.Id, usuarioLogueado.Email))
                {
                    var docenteLogica = Archivo.Instancia.Leer<LogicaDocente>().Find(x => x.Id == docente.Id && x.Eliminado == false);
                    if (!docenteLogica.Salas.Exists(x => x.Id == sala.Id))
                    {
                        docenteLogica.Salas.Add(Archivo.Instancia.Leer<LogicaSala>().Find(x => x.Id == sala.Id && x.Eliminado == false));
                        Archivo.Instancia.Guardar(docenteLogica);
                    }
                }
                else
                    Resultado.Errores.Add("Error 403: Directora no pertenece a la misma institucion.");
            }
            return Resultado;
        }

        public Resultado AsignarHijoPadre(Hijo hijo, Padre padre, UsuarioLogueado usuarioLogueado)
        {
            //El usuario debe ser directora, y el hijo debe estar asociado a una sala de su institucion
            var Resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            if (Resultado.EsValido)
            {
                if (Empresa.MismaInstitucion(hijo.Id, usuarioLogueado.Email))
                {
                    var padreLogica = Archivo.Instancia.Leer<LogicaPadre>().Find(x => x.Id == padre.Id && x.Eliminado == false);
                    padreLogica.Hijos.RemoveAll(y => y.Id == Archivo.Instancia.Leer<LogicaHijo>().Find(x => x.Id == hijo.Id && x.Eliminado == false).Id);
                    padreLogica.Hijos.Add(Archivo.Instancia.Leer<LogicaHijo>().Find(x => x.Id == hijo.Id && x.Eliminado == false));
                    Archivo.Instancia.Guardar(padreLogica);
                }
                else
                    Resultado.Errores.Add("Error 403: Padre no pertenece a la misma institucion.");
            }
            return Resultado;
        }

        public Resultado DesasignarDocenteSala(Docente docente, Sala sala, UsuarioLogueado usuarioLogueado)
        {
            var Resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            if (Resultado.EsValido)
            {
                if (Empresa.MismaInstitucion(docente.Id, usuarioLogueado.Email))
                {
                    var docenteLogica = Archivo.Instancia.Leer<LogicaDocente>().Find(x => x.Id == docente.Id && x.Eliminado == false);
                    docenteLogica.Salas.RemoveAll(x => x.Id == sala.Id);
                    Archivo.Instancia.Guardar(docenteLogica);
                }
                else
                    Resultado.Errores.Add("Error 403: Docente no pertenece a la misma institucion.");
            }
            return Resultado;
        }

        public Resultado DesasignarHijoPadre(Hijo hijo, Padre padre, UsuarioLogueado usuarioLogueado)
        {
            var Resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            if (Resultado.EsValido)
            {
                if (Empresa.MismaInstitucion(hijo.Id, usuarioLogueado.Email))
                {
                    var padreLogica = Archivo.Instancia.Leer<LogicaPadre>().Find(x => x.Id == padre.Id && x.Eliminado == false);
                    padreLogica.Hijos.RemoveAll(y => y.Id == hijo.Id);
                    Archivo.Instancia.Guardar(padreLogica);
                }
                else
                    Resultado.Errores.Add("Error 403: Alumno no pertenece a la misma institucion.");
            }
            return Resultado;
        }

        public Resultado EditarAlumno(int id, Hijo hijo, UsuarioLogueado usuarioLogueado)
        {
            var Resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);

            if (Resultado.EsValido)
            {
                var AlumnoEncontrado = Archivo.Instancia.Leer<LogicaHijo>().FirstOrDefault(x => x.Eliminado == false && x.Id == id);
                if (AlumnoEncontrado != null)
                {
                    var HijoMapeado = AutoMapper.Instancia.Mapear<Hijo, LogicaHijo>(hijo);
                    HijoMapeado.CopiarParametrosFaltantes(AlumnoEncontrado);
                    Archivo.Instancia.Guardar(HijoMapeado);
                }
                else
                    Resultado.Errores.Add("Error 404: Alumno no encontrado en la base de datos");
            }
            else
                Resultado.Errores.Add("Error 403: Permisos Insuficientes");
            return Resultado;
        }

        public Resultado EditarDirectora(int id, Directora directora, UsuarioLogueado usuarioLogueado)
        {
            var Resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);

            if (Resultado.EsValido)
            {
                var DirectoraEncontrada = Archivo.Instancia.Leer<LogicaDirectora>().FirstOrDefault(x => x.Eliminado == false && x.Id == id);
                if (DirectoraEncontrada != null)
                {
                    var DirectoraMapeada = AutoMapper.Instancia.Mapear<Directora, LogicaDirectora>(directora);
                    DirectoraMapeada.CopiarParametrosFaltantes(DirectoraEncontrada);
                    Archivo.Instancia.Guardar(DirectoraMapeada);
                }
                else
                    Resultado.Errores.Add("Error 404: Directora no encontrada en la base de datos");
            }
            else
                Resultado.Errores.Add("Error 403: Permisos Insuficientes");
            return Resultado;
        }

        public Resultado EditarDocente(int id, Docente docente, UsuarioLogueado usuarioLogueado)
        {
            var Resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);

            if (Resultado.EsValido)
            {
                var DocenteEncontrada = Archivo.Instancia.Leer<LogicaDocente>().FirstOrDefault(x => x.Eliminado == false && x.Id == id);
                if (DocenteEncontrada != null)
                {
                    var DocenteMapeada = AutoMapper.Instancia.Mapear<Docente, LogicaDocente>(docente);
                    DocenteMapeada.CopiarParametrosFaltantes(DocenteEncontrada);
                    Archivo.Instancia.Guardar(DocenteMapeada);
                }
                else
                    Resultado.Errores.Add("Error 404: Docente no encontrada en la base de datos");
            }
            else
                Resultado.Errores.Add("Error 403: Permisos Insuficientes");
            return Resultado;
        }

        public Resultado EditarPadreMadre(int id, Padre padre, UsuarioLogueado usuarioLogueado)
        {
            var Resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);

            if (Resultado.EsValido)
            {
                var PadreEncontrado = Archivo.Instancia.Leer<LogicaPadre>().FirstOrDefault(x => x.Eliminado == false && x.Id == id);
                if (PadreEncontrado != null)
                {
                    var PadreMadreMapeada = AutoMapper.Instancia.Mapear<Padre, LogicaPadre>(padre);
                    PadreMadreMapeada.CopiarParametrosFaltantes(PadreEncontrado);
                    Archivo.Instancia.Guardar(PadreMadreMapeada);
                }
                else
                    Resultado.Errores.Add("Error 404: Padre no encontrado en la base de datos");
            }
            else
                Resultado.Errores.Add("Error 403: Permisos Insuficientes");
            return Resultado;
        }

        public Resultado EliminarAlumno(int id, Hijo hijo, UsuarioLogueado usuarioLogueado)
        {
            Resultado Resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            List<LogicaHijo> Alumno = Archivo.Instancia.Leer<LogicaHijo>();
            if (Alumno == null || Alumno.Count() == 0)
                Resultado.Errores.Add("Error 404: Alumno no encontrado en la base de datos.");
            else
            {
                LogicaHijo AlumnoSeleccionado = Alumno.Find(x => x.Id == id && x.Eliminado == false);
                if (AlumnoSeleccionado == null)
                    Resultado.Errores.Add("Error 404: Alumno no encontrado en la base de datos.");
            }
            if (Resultado.EsValido)
            {
                var HijoCasteado = AutoMapper.Instancia.Mapear<Hijo, LogicaHijo>(hijo);
                Archivo.Instancia.Guardar(HijoCasteado, true);
            }
            return Resultado;
        }

        public Resultado EliminarDirectora(int id, Directora directora, UsuarioLogueado usuarioLogueado)
        {
            Resultado Resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            List<LogicaDirectora> Directora = Archivo.Instancia.Leer<LogicaDirectora>();
            if (Directora == null || Directora.Count() == 0)
                Resultado.Errores.Add("Error 404: Directora no encontrado en la base de datos.");
            else
            {
                LogicaDirectora DirectoraSeleccionado = Directora.Find(x => x.Id == id && x.Eliminado == false);
                if (DirectoraSeleccionado == null)
                    Resultado.Errores.Add("Error 404: Directora no encontrado en la base de datos.");
            }
            if (Resultado.EsValido)
            {
                var DirectoraCasteada = AutoMapper.Instancia.Mapear<Directora, LogicaDirectora>(directora);
                Archivo.Instancia.Guardar(DirectoraCasteada, true);
            }
            return Resultado;
        }

        public Resultado EliminarDocente(int id, Docente docente, UsuarioLogueado usuarioLogueado)
        {
            Resultado Resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            List<LogicaDocente> Docente = Archivo.Instancia.Leer<LogicaDocente>();
            if (Docente == null || Docente.Count() == 0)
                Resultado.Errores.Add("Error 404: Docente no encontrado en la base de datos.");
            else
            {
                LogicaDocente DocenteSeleccionado = Docente.Find(x => x.Id == id && x.Eliminado == false);
                if (DocenteSeleccionado == null)
                    Resultado.Errores.Add("Error 404: Docente no encontrado en la base de datos.");
            }
            if (Resultado.EsValido)
            {
                var DocenteCasteado = AutoMapper.Instancia.Mapear<Docente, LogicaDocente>(docente);
                Archivo.Instancia.Guardar(DocenteCasteado, true);
            }
            return Resultado;
        }

        public Resultado EliminarPadreMadre(int id, Padre padre, UsuarioLogueado usuarioLogueado)
        {
            Resultado Resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            List<LogicaPadre> Padre = Archivo.Instancia.Leer<LogicaPadre>();
            if (Padre == null || Padre.Count() == 0)
                Resultado.Errores.Add("Error 404: Padre no encontrado en la base de datos.");
            else
            {
                LogicaPadre PadreSeleccionado = Padre.Find(x => x.Id == id && x.Eliminado == false);
                if (PadreSeleccionado == null)
                    Resultado.Errores.Add("Error 404: Padre no encontrado en la base de datos.");
            }
            if (Resultado.EsValido)
            {
                var PadreCasteado = AutoMapper.Instancia.Mapear<Padre, LogicaPadre>(padre);
                Archivo.Instancia.Guardar(PadreCasteado, true);
            }
            return Resultado;
        }

        public Resultado MarcarNotaComoLeida(Nota nota, UsuarioLogueado usuarioLogueado)
        {
            var Resultado = new Resultado();
            var User = Archivo.Instancia.Leer<LogicaUsuario>().Find(x => x.Email == usuarioLogueado.Email && x.Eliminado == false && x.Nombre == usuarioLogueado.Nombre);
            if (User.RolSeleccionado == Roles.Padre)
            {
                var LogicaPadre = Archivo.Instancia.Leer<LogicaPadre>().Find(x => x.Email == usuarioLogueado.Email && x.Eliminado == false && x.Nombre == usuarioLogueado.Nombre);
                var NotaLogica = Archivo.Instancia.Leer<LogicaNota>().Find(x => x.Id == nota.Id);
                var AlumnoLogica = Archivo.Instancia.Leer<LogicaHijo>().Find(x => LogicaPadre.Hijos.Exists(z => z.Id == x.Id) && x.Notas.Exists(y => y.Id == NotaLogica.Id) && x.Eliminado == false);
                NotaLogica.Leida = true;
                Archivo.Instancia.Guardar(NotaLogica);
                AlumnoLogica.Notas.Find(x => x.Id == NotaLogica.Id).Leida = true;
                Archivo.Instancia.Guardar(AlumnoLogica);
                LogicaPadre.Hijos.RemoveAll(x => x.Id == AlumnoLogica.Id);
                LogicaPadre.Hijos.Add(AlumnoLogica);
                Archivo.Instancia.Guardar(LogicaPadre);
            }
            else
                Resultado.Errores.Add("Error 403: Permisos Insuficientes");
            return Resultado;
        }

        public Hijo ObtenerAlumnoPorId(UsuarioLogueado usuarioLogueado, int id)
        {
            LogicaHijo Alumno = Archivo.Instancia.Leer<LogicaHijo>().Where(x => x.Id == id && x.Eliminado == false).FirstOrDefault();
            return AutoMapper.Instancia.Mapear<LogicaHijo, Hijo>(Alumno);
        }

        public Grilla<Hijo> ObtenerAlumnos(UsuarioLogueado usuarioLogueado, int paginaActual, int totalPorPagina, string busquedaGlobal)
        {
            Resultado Resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            if (Resultado.EsValido)
            {
                var Id = Empresa.IDInstitucionUsuarioLogueado(usuarioLogueado.Email);
                List<LogicaHijo> Lista = Archivo.Instancia.Leer<LogicaHijo>()
                    .Where(x => x.Id != Id && x.Eliminado == false && x.IdInstitucion == Id && (string.IsNullOrEmpty(busquedaGlobal) || x.Nombre.Contains(busquedaGlobal) || x.Apellido.Contains(busquedaGlobal)))
                    .ToList();
                var ListaADevolver = AutoMapper.Instancia.ConvertirLista<LogicaHijo, Hijo>(Lista.FindAll(x => x.Eliminado == false))
                    .Skip(paginaActual * totalPorPagina).Take(totalPorPagina).ToArray();
                if (ListaADevolver == null)
                    ListaADevolver = new Hijo[] { };
                return new Grilla<Hijo>()
                {
                    Lista = ListaADevolver,
                    CantidadLogicaUsuario = Lista.Count()
                };
            }
            else
            {
                return new Grilla<Hijo>()
                {
                    Lista = new Hijo[] { },
                    CantidadLogicaUsuario = 0
                };
            }
        }

        public Nota[] ObtenerCuadernoComunicaciones(int idPersona, UsuarioLogueado usuarioLogueado)
        {
            var LogicaUsuario = Archivo.Instancia.Leer<LogicaUsuario>().Find(x => x.Email == usuarioLogueado.Email && x.Eliminado == false && x.Nombre == usuarioLogueado.Nombre);
            if (LogicaUsuario.RolSeleccionado == Roles.Directora)
            {
                var Alumno = Archivo.Instancia.Leer<LogicaHijo>().Find(alm => alm.Id == idPersona && alm.Eliminado == false);
                return AutoMapper.Instancia.ConvertirLista<LogicaNota, Nota>(Alumno.Notas).ToArray();
            }

            if (LogicaUsuario.RolSeleccionado == Roles.Docente)
            {
                var LogicaDocente = Archivo.Instancia.Leer<LogicaDocente>().Find(x => x.Email == usuarioLogueado.Email && x.Eliminado == false && x.Nombre == usuarioLogueado.Nombre);
                var LogicaSalasId = LogicaDocente.Salas.Select(x => x.Id).ToList();
                var AlumnoDelDocente = Archivo.Instancia.Leer<LogicaHijo>().Find(al => al.Id == idPersona && al.IdInstitucion == LogicaDocente.IdInstitucion && al.Eliminado == false && LogicaSalasId.Contains(al.Sala.Id)); // retorno los alumnos que estan en la institucion
                return AutoMapper.Instancia.ConvertirLista<LogicaNota, Nota>(AlumnoDelDocente.Notas).ToArray();
            }
            if (LogicaUsuario.RolSeleccionado == Roles.Padre)
            {
                var LogicaPadre = Archivo.Instancia.Leer<LogicaPadre>().Find(x => x.Email == usuarioLogueado.Email && x.Eliminado == false && x.Nombre == usuarioLogueado.Nombre);
                return AutoMapper.Instancia.ConvertirLista<LogicaNota, Nota>(LogicaPadre.Hijos.Find(x => x.Id == idPersona).Notas).ToArray();
            }
            if (LogicaUsuario.RolSeleccionado == null) // Agregada funcionalidad para que un alumno pueda ver su cuaderno
            {
                var Alumno = Archivo.Instancia.Leer<LogicaHijo>().Find(x => x.Email == usuarioLogueado.Email && x.Eliminado == false && x.Nombre == usuarioLogueado.Nombre);
                return AutoMapper.Instancia.ConvertirLista<LogicaNota, Nota>(Alumno.Notas).ToArray();
            }
            return new Nota[] { };
        }

        public Directora ObtenerDirectoraPorId(UsuarioLogueado usuarioLogueado, int id)
        {
            LogicaDirectora Directora = Archivo.Instancia.Leer<LogicaDirectora>().Where(x => x.Id == id && x.Eliminado == false).FirstOrDefault();
            return AutoMapper.Instancia.Mapear<LogicaDirectora, Directora>(Directora);
        }

        public Grilla<Directora> ObtenerDirectoras(UsuarioLogueado usuarioLogueado, int paginaActual, int totalPorPagina, string busquedaGlobal)
        {
            Resultado Resul = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            if (Resul.EsValido)
            {
                var Id = Empresa.IDInstitucionUsuarioLogueado(usuarioLogueado.Email);
                List<LogicaDirectora> Lista = Archivo.Instancia.Leer<LogicaDirectora>()
                    .Where(x => x.Id != Id && x.Eliminado == false && x.IdInstitucion == Id && (string.IsNullOrEmpty(busquedaGlobal) || x.Nombre.Contains(busquedaGlobal) || x.Apellido.Contains(busquedaGlobal)))
                    .ToList();
                var ListaADevolver = AutoMapper.Instancia.ConvertirLista<LogicaDirectora, Directora>(Lista)
                    .Skip(paginaActual * totalPorPagina).Take(totalPorPagina).ToArray();
                if (ListaADevolver == null)
                    ListaADevolver = new Directora[] { };
                return new Grilla<Directora>()
                {
                    Lista = ListaADevolver,
                    CantidadLogicaUsuario = Lista.Count()
                };
            }
            else
            {
                return new Grilla<Directora>()
                {
                    Lista = new Directora[] { },
                    CantidadLogicaUsuario = 0
                }; ;
            }
        }

        public Docente ObtenerDocentePorId(UsuarioLogueado usuarioLogueado, int id)
        {
            LogicaDocente Docente = Archivo.Instancia.Leer<LogicaDocente>().Where(x => x.Id == id && x.Eliminado == false).FirstOrDefault();
            return AutoMapper.Instancia.Mapear<LogicaDocente, Docente>(Docente);
        }

        public Grilla<Docente> ObtenerDocentes(UsuarioLogueado usuarioLogueado, int paginaActual, int totalPorPagina, string busquedaGlobal)
        {
            Resultado Resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            if (Resultado.EsValido)
            {
                var Id = Empresa.IDInstitucionUsuarioLogueado(usuarioLogueado.Email);
                List<LogicaDocente> Lista = Archivo.Instancia.Leer<LogicaDocente>()
                    .Where(x => x.Id != Id && x.Eliminado == false && x.IdInstitucion == Id && (string.IsNullOrEmpty(busquedaGlobal) || x.Nombre.Contains(busquedaGlobal) || x.Apellido.Contains(busquedaGlobal)))
                    .ToList();
                var ListaADevolver = AutoMapper.Instancia.ConvertirLista<LogicaDocente, Docente>(Lista)
                    .Skip(paginaActual * totalPorPagina).Take(totalPorPagina).ToArray();
                if (ListaADevolver == null)
                    ListaADevolver = new Docente[] { };
                return new Grilla<Docente>()
                {
                    Lista = ListaADevolver,
                    CantidadLogicaUsuario = Lista.Count()
                };
            }
            else
            {
                return new Grilla<Docente>()
                {
                    Lista = new Docente[] { },
                    CantidadLogicaUsuario = 0
                };
            }
        }

        public Institucion[] ObtenerInstituciones()
        {
            var ListaInstituciones = Archivo.Instancia.Leer<LogicaInstitucion>().FindAll(x => x.Eliminado == false);
            return AutoMapper.Instancia.ConvertirLista<LogicaInstitucion, Institucion>(ListaInstituciones).ToArray();
        }

        public void InicializarTablas()
        {
            // las instituciones no se pueden agregar, no hay un metodo AltaInstitucion
            List<LogicaInstitucion> _instituciones = new List<LogicaInstitucion>() // institucion no tiene id=0 por Empresa.IDInstitucionUsuarioLogueado
            {
            new LogicaInstitucion(){ Id = 1, Ciudad = "Rafaela", Direccion = "Ituzaingo 403", Nombre = "Misericordia", Provincia = "Santa Fe", Telefono = "03492565890", Eliminado = false},
            new LogicaInstitucion(){ Id = 2, Ciudad = "Rafaela", Direccion = "Colon 403", Nombre = "San Jose", Provincia = "Santa Fe", Telefono = "03492565890", Eliminado = false},
            new LogicaInstitucion(){ Id = 3, Ciudad = "Rafaela", Direccion = "Saavedra 403", Nombre = "Normal", Provincia = "Santa Fe", Telefono = "03492565890", Eliminado = false},
            };

            List<LogicaDirectora> _directoras = new List<LogicaDirectora>()
            {
            new LogicaDirectora(){ Id = 1, Nombre = "A 1", Apellido ="B1", Email = "C1", Roles = new Roles[] { Roles.Directora }, RolSeleccionado = Roles.Directora, FechaIngreso = DateTime.Now, Eliminado = false, IdInstitucion = 1, Password = "123", Cargo = "D", Institucion = _instituciones[0]},new LogicaDirectora(){ Id = 2, Nombre = "A 2", Apellido ="B2", Email = "C2", Roles = new Roles[] { Roles.Directora }, RolSeleccionado = Roles.Directora, FechaIngreso = DateTime.Now, Eliminado = false, IdInstitucion = 2, Password = "123", Cargo = "D", Institucion = _instituciones[0]},
            new LogicaDirectora(){ Id = 3, Nombre = "A 3", Apellido ="B3", Email = "C3", Roles = new Roles[] { Roles.Directora }, RolSeleccionado = Roles.Directora, FechaIngreso = DateTime.Now, Eliminado = false, IdInstitucion = 1, Password = "123", Cargo = "D", Institucion = _instituciones[0]},new LogicaDirectora(){ Id = 4, Nombre = "A 4", Apellido ="B4", Email = "C4", Roles = new Roles[] { Roles.Directora }, RolSeleccionado = Roles.Directora, FechaIngreso = DateTime.Now, Eliminado = false, IdInstitucion = 2, Password = "123", Cargo = "D", Institucion = _instituciones[0]},
            new LogicaDirectora(){ Id = 5, Nombre = "A 5", Apellido ="B5", Email = "C5", Roles = new Roles[] { Roles.Directora }, RolSeleccionado = Roles.Directora, FechaIngreso = DateTime.Now, Eliminado = false, IdInstitucion = 1, Password = "123", Cargo = "D", Institucion = _instituciones[0]},new LogicaDirectora(){ Id = 6, Nombre = "A 6", Apellido ="B6", Email = "C6", Roles = new Roles[] { Roles.Directora }, RolSeleccionado = Roles.Directora, FechaIngreso = DateTime.Now, Eliminado = false, IdInstitucion = 2, Password = "123", Cargo = "D", Institucion = _instituciones[0]},
            new LogicaDirectora(){ Id = 7, Nombre = "A 7", Apellido ="B7", Email = "C7", Roles = new Roles[] { Roles.Directora }, RolSeleccionado = Roles.Directora, FechaIngreso = DateTime.Now, Eliminado = false, IdInstitucion = 1, Password = "123", Cargo = "D", Institucion = _instituciones[0]},new LogicaDirectora(){ Id = 8, Nombre = "A 8", Apellido ="B8", Email = "8", Roles = new Roles[] { Roles.Directora }, RolSeleccionado = Roles.Directora, FechaIngreso = DateTime.Now, Eliminado = false, IdInstitucion = 2, Password = "123", Cargo = "D", Institucion = _instituciones[0]},
            new LogicaDirectora(){ Id = 9, Nombre = "A 9", Apellido ="B9", Email = "C9", Roles = new Roles[] { Roles.Directora }, RolSeleccionado = Roles.Directora, FechaIngreso = DateTime.Now, Eliminado = false, IdInstitucion = 1, Password = "123", Cargo = "D", Institucion = _instituciones[0]},new LogicaDirectora(){ Id = 10, Nombre = "A 10", Apellido ="B10", Email = "C10", Roles = new Roles[] { Roles.Directora }, RolSeleccionado = Roles.Directora, FechaIngreso = DateTime.Now, Eliminado = false, IdInstitucion = 2, Password = "123", Cargo = "D", Institucion = _instituciones[0]},
            new LogicaDirectora(){ Id = 11, Nombre = "A 11", Apellido ="B11", Email = "C11", Roles = new Roles[] { Roles.Directora }, RolSeleccionado = Roles.Directora, FechaIngreso = DateTime.Now, Eliminado = false, IdInstitucion = 1, Password = "123", Cargo = "D", Institucion = _instituciones[1]},new LogicaDirectora(){ Id = 12, Nombre = "A 12", Apellido ="B12", Email = "C12", Roles = new Roles[] { Roles.Directora }, RolSeleccionado = Roles.Directora, FechaIngreso = DateTime.Now, Eliminado = false, IdInstitucion = 2, Password = "123", Cargo = "D", Institucion = _instituciones[2]},
            };

            List<LogicaSala> _salas = new List<LogicaSala>()
            {
                new LogicaSala(){ Id = 1, Nombre = "Matematica", IdInstitucion = 1, Eliminado = false },
                new LogicaSala(){ Id = 2, Nombre = "Lengua", IdInstitucion = 1, Eliminado = false },
                new LogicaSala(){ Id = 3, Nombre = "Programacion", IdInstitucion = 1, Eliminado = false },
                new LogicaSala(){ Id = 4, Nombre = "Historia", IdInstitucion = 1, Eliminado = false },
            };

            List<LogicaDocente> _docentes = new List<LogicaDocente>()
            {
            new LogicaDocente(){ Id = 13, Nombre = "D 1", Apellido ="DA 1", Eliminado = false, IdInstitucion = 1, Email = "DE 1",  Password = "123", Roles = new Roles[] { Roles.Docente }, RolSeleccionado = Roles.Docente, Salas = new List<LogicaSala> { _salas[0] }},new LogicaDocente(){ Id = 14, Nombre = "D 2", Apellido ="DA 2", Eliminado = false, IdInstitucion = 1, Email = "DE 2", Password = "123", Roles = new Roles[] { Roles.Docente }, RolSeleccionado = Roles.Docente, Salas = new List<LogicaSala> { _salas[0] } },
            new LogicaDocente(){ Id = 15, Nombre = "D 3", Apellido ="DA 3", Eliminado = false, IdInstitucion = 1, Email = "DE 3",  Password = "123", Roles = new Roles[] { Roles.Docente }, RolSeleccionado = Roles.Docente, Salas = new List<LogicaSala> { _salas[1] }},new LogicaDocente(){ Id = 16, Nombre = "D 4", Apellido ="DA 4", Eliminado = false, IdInstitucion = 1, Email = "DE 4",  Password = "123", Roles = new Roles[] { Roles.Docente }, RolSeleccionado = Roles.Docente, Salas = new List<LogicaSala> { _salas[0] }},
            new LogicaDocente(){ Id = 17, Nombre = "D 5", Apellido ="DA 5", Eliminado = false, IdInstitucion = 1, Email = "DE 5",  Password = "123", Roles = new Roles[] { Roles.Docente }, RolSeleccionado = Roles.Docente, Salas = new List<LogicaSala> { _salas[2] }},new LogicaDocente(){ Id = 18, Nombre = "D 6", Apellido ="DA 6", Eliminado = false, IdInstitucion = 1, Email = "DE 6",  Password = "123", Roles = new Roles[] { Roles.Docente }, RolSeleccionado = Roles.Docente, Salas = new List<LogicaSala> { _salas[0] }},
            new LogicaDocente(){ Id = 19, Nombre = "D 7", Apellido ="DA 7", Eliminado = false, IdInstitucion = 1, Email = "DE 7",  Password = "123", Roles = new Roles[] { Roles.Docente }, RolSeleccionado = Roles.Docente, Salas = new List<LogicaSala> { _salas[3] }},new LogicaDocente(){ Id = 20, Nombre = "D 8", Apellido ="DA 8", Eliminado = false, IdInstitucion = 1, Email = "DE 8",  Password = "123", Roles = new Roles[] { Roles.Docente }, RolSeleccionado = Roles.Docente, Salas = new List<LogicaSala> { _salas[1] }},
            new LogicaDocente(){ Id = 21, Nombre = "D 9", Apellido ="DA 9", Eliminado = false, IdInstitucion = 2, Email = "DE 9",  Password = "123", Roles = new Roles[] { Roles.Docente }, RolSeleccionado = Roles.Docente, Salas = new List<LogicaSala> { _salas[1] }},new LogicaDocente(){ Id = 22, Nombre = "D 10", Apellido ="DA 10", Eliminado = false, IdInstitucion = 2, Email = "DE 10",  Password = "123", Roles = new Roles[] { Roles.Docente }, RolSeleccionado = Roles.Docente, Salas = new List<LogicaSala> { _salas[1] }},
            new LogicaDocente(){ Id = 23, Nombre = "D 11", Apellido ="DA 11", Eliminado = false, IdInstitucion = 2, Email = "DE 11",  Password = "123", Roles = new Roles[] { Roles.Docente }, RolSeleccionado = Roles.Docente, Salas = new List<LogicaSala> { _salas[2] }},new LogicaDocente(){ Id = 24, Nombre = "D 12", Apellido ="DA 12", Eliminado = false, IdInstitucion = 2, Email = "DE 12",  Password = "123", Roles = new Roles[] { Roles.Docente }, RolSeleccionado = Roles.Docente, Salas = new List<LogicaSala> { _salas[2] }},
            new LogicaDocente(){ Id = 25, Nombre = "D 13", Apellido ="DA 13", Eliminado = false, IdInstitucion = 2, Email = "DE 13",  Password = "123", Roles = new Roles[] { Roles.Docente }, RolSeleccionado = Roles.Docente, Salas = new List<LogicaSala> { _salas[2] }},new LogicaDocente(){ Id = 26, Nombre = "D 14", Apellido ="DA 14", Eliminado = false, IdInstitucion = 2, Email = "DE 14",  Password = "123", Roles = new Roles[] { Roles.Docente }, RolSeleccionado = Roles.Docente, Salas = new List<LogicaSala> { _salas[2] }},
            new LogicaDocente(){ Id = 27, Nombre = "D 15", Apellido ="DA 15", Eliminado = false, IdInstitucion = 2, Email = "DE 15",  Password = "123", Roles = new Roles[] { Roles.Docente }, RolSeleccionado = Roles.Docente, Salas = new List<LogicaSala> { _salas[2] }},new LogicaDocente(){ Id = 28, Nombre = "D 16", Apellido ="DA 16", Eliminado = false, IdInstitucion = 2, Email = "DE 16",  Password = "123", Roles = new Roles[] { Roles.Docente }, RolSeleccionado = Roles.Docente, Salas = new List<LogicaSala> { _salas[2] }},
            };

            List<LogicaNota> _notas1 = new List<LogicaNota>()
            {
            new LogicaNota(){ Id = 1, Leida = false, Titulo= "LogicaNota 1", Descripcion = "Descripcion de la nota 1", Comentarios = new List<LogicaComentario>{ } },
            new LogicaNota(){ Id = 2, Leida = false, Titulo= "LogicaNota 2", Descripcion = "Descripcion de la nota 2", Comentarios = new List<LogicaComentario>{
                new LogicaComentario() { Fecha = DateTime.Now.AddDays(-2), Mensaje = "LogicaComentario 1" , Usuario = new LogicaUsuario(){ Nombre = "Usuario", Apellido="Cualquiera" } },
                new LogicaComentario() { Fecha = DateTime.Now.AddDays(-1), Mensaje = "LogicaComentario 2" , Usuario = new LogicaUsuario(){ Nombre = "Usuario", Apellido="Cualquiera 2" } },
                } }
            };

            List<LogicaNota> _notas2 = new List<LogicaNota>()
            {
                new LogicaNota(){ Id = 3, Leida = true, Titulo= "LogicaNota 3", Descripcion = "Descripcion de la nota 3", Comentarios = new List<LogicaComentario>{ } },
            };

            List<LogicaNota> _notas3 = new List<LogicaNota>()
            {
                new LogicaNota(){ Id = 4, Leida = false, Titulo= "LogicaNota 4", Descripcion = "Descripcion de la nota 4", Comentarios = new List<LogicaComentario>{ } },
            };

            List<LogicaNota> _notas4 = new List<LogicaNota>()
            {
                new LogicaNota(){ Id = 5, Leida = true, Titulo= "LogicaNota 5", Descripcion = "Descripcion de la nota 5", Comentarios = new List<LogicaComentario>{ } },
            };

            List<LogicaHijo> _alumnos = new List<LogicaHijo>()
            {
            new LogicaHijo(){ Id = 33, Nombre = "AL 1", Apellido="AP 1", IdInstitucion = 1, Eliminado = false, Email="APE 1", FechaNacimiento = new DateTime(1990,5,4), ResultadoUltimaEvaluacionAnual = 10, Sala = _salas[0], Notas = _notas1, Institucion = _instituciones[0], Password = "123", Roles = null, RolSeleccionado = null},
            new LogicaHijo(){ Id = 34, Nombre = "AL 2", Apellido="AP 2", IdInstitucion = 1, Eliminado = false, Email="APE 2", FechaNacimiento = new DateTime(1991,3,20), ResultadoUltimaEvaluacionAnual = 6, Sala = _salas[0], Notas = _notas2, Institucion = _instituciones[0], Password = "123", Roles = null, RolSeleccionado = null},
            new LogicaHijo(){ Id = 35, Nombre = "AL 3", Apellido="AP 3", IdInstitucion = 1, Eliminado = false, Email="APE 3", FechaNacimiento = new DateTime(1992,12,14), ResultadoUltimaEvaluacionAnual = 5, Sala = _salas[0], Notas = _notas3, Institucion = _instituciones[0], Password = "123", Roles = null, RolSeleccionado = null},
            new LogicaHijo(){ Id = 36, Nombre = "AL 4", Apellido="AP 4", IdInstitucion = 1, Eliminado = false, Email="APE 4", FechaNacimiento = new DateTime(1989,11,29), ResultadoUltimaEvaluacionAnual = 3, Sala = _salas[0], Notas = _notas4, Institucion = _instituciones[0], Password = "123", Roles = null, RolSeleccionado = null},
            new LogicaHijo(){ Id = 37, Nombre = "AL 5", Apellido="AP 5", IdInstitucion = 2, Eliminado = false, Email="APE 1", FechaNacimiento = new DateTime(1990,5,4), ResultadoUltimaEvaluacionAnual = 10, Sala = _salas[1], Notas = _notas1, Institucion = _instituciones[1], Password = "123", Roles = null, RolSeleccionado = null},
            new LogicaHijo(){ Id = 38, Nombre = "AL 6", Apellido="AP 6", IdInstitucion = 2, Eliminado = false, Email="APE 2", FechaNacimiento = new DateTime(1991,3,20), ResultadoUltimaEvaluacionAnual = 6, Sala = _salas[1], Notas = _notas2, Institucion = _instituciones[1], Password = "123", Roles = null, RolSeleccionado = null},
            new LogicaHijo(){ Id = 39, Nombre = "AL 7", Apellido="AP 7", IdInstitucion = 3, Eliminado = false, Email="APE 1", FechaNacimiento = new DateTime(1990,5,4), ResultadoUltimaEvaluacionAnual = 10, Sala = _salas[2], Notas = _notas1, Institucion = _instituciones[2], Password = "123", Roles = null, RolSeleccionado = null},
            new LogicaHijo(){ Id = 40, Nombre = "AL 8", Apellido="AP 8", IdInstitucion = 3, Eliminado = false, Email="APE 2", FechaNacimiento = new DateTime(1991,3,20), ResultadoUltimaEvaluacionAnual = 6, Sala = _salas[2], Notas = _notas2, Institucion = _instituciones[2], Password = "123", Roles = null, RolSeleccionado = null},
            };

            List<LogicaPadre> _padres = new List<LogicaPadre>()
            {
            new LogicaPadre(){ Id = 29, Nombre = "P 1", Apellido = "PA 1", Eliminado = false, Hijos = new List<LogicaHijo> { _alumnos[0] }, Password = "123", Roles = new Roles[] { Roles.Padre }, RolSeleccionado = Roles.Padre, IdInstitucion = 1, Email = "PE 1"},
            new LogicaPadre(){ Id = 30, Nombre = "P 2", Apellido = "PA 2", Eliminado = false, Hijos = new List<LogicaHijo> { _alumnos[1] }, IdInstitucion = 1, Email = "PE 2", Password = "123", Roles = new Roles[] { Roles.Padre }, RolSeleccionado = Roles.Padre },
            new LogicaPadre(){ Id = 31, Nombre = "P 3", Apellido = "PA 3", Eliminado = false, Hijos = new List<LogicaHijo> { _alumnos[2] }, Password = "123", Roles = new Roles[] { Roles.Padre }, RolSeleccionado = Roles.Padre, IdInstitucion = 1, Email = "PE 3"},
            new LogicaPadre(){ Id = 32, Nombre = "P 4", Apellido = "PA 4", Eliminado = false, Hijos = new List<LogicaHijo> { _alumnos[3] }, IdInstitucion = 2, Email = "PE 4"},
            new LogicaPadre(){ Id = 41, Nombre = "P 5", Apellido = "PA 5", Eliminado = false, Hijos = new List<LogicaHijo> { _alumnos[4] }, IdInstitucion = 2, Email = "PE 4"},
            };

            _directoras.ForEach(x => Archivo.Instancia.Guardar(x));
            _salas.ForEach(x => Archivo.Instancia.Guardar(x));
            _docentes.ForEach(x => Archivo.Instancia.Guardar(x));
            _padres.ForEach(x => Archivo.Instancia.Guardar(x));
            _instituciones.ForEach(x => Archivo.Instancia.Guardar(x));
            _notas1.ForEach(x => Archivo.Instancia.Guardar(x));
            _notas2.ForEach(x => Archivo.Instancia.Guardar(x));
            _notas3.ForEach(x => Archivo.Instancia.Guardar(x));
            _notas4.ForEach(x => Archivo.Instancia.Guardar(x));
            _alumnos.ForEach(x => Archivo.Instancia.Guardar(x));
        }

        public string ObtenerNombreGrupo()
        {
            /// INICIALIZAR TABLAS
            /// Para inicializar las tablas en archivo .txt:
            /// 1. Descomente la funcion InicializarTablas(), ejecute la aplicación UNA sola vez y NO interactue, 
            /// solo cierrela una vez que termina de cargar, luego comente nuevamente la funcion.
            /// 2. O bien ejecute el test unitario "ProbarCreacionDeTablas_DeberiaCrearTablasEnCarpetaPrincipal" UNA sola vez
            /// para no sobreescribir las tablas.
            /// Si ya hay tablas creadas en WebApp/WebApp/ borrelas para poder probarlo. Las tablas a crear son archivos .txt

            //InicializarTablas();

            return "Cominotti Mauro, Yacovino Juan, Zoja Emanuel";
        }

        public Padre ObtenerPadrePorId(UsuarioLogueado usuarioLogueado, int id)
        {
            LogicaPadre Padre = Archivo.Instancia.Leer<LogicaPadre>().Where(x => x.Id == id && x.Eliminado == false).FirstOrDefault();
            return AutoMapper.Instancia.Mapear<LogicaPadre, Padre>(Padre);
        }

        public Grilla<Padre> ObtenerPadres(UsuarioLogueado usuarioLogueado, int paginaActual, int totalPorPagina, string busquedaGlobal)
        {
            Resultado Resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            if (Resultado.EsValido)
            {
                var Id = Empresa.IDInstitucionUsuarioLogueado(usuarioLogueado.Email);
                List<LogicaPadre> Lista = Archivo.Instancia.Leer<LogicaPadre>()
                    .Where(x => x.Id != Id && x.Eliminado == false && x.IdInstitucion == Id && (string.IsNullOrEmpty(busquedaGlobal) || x.Nombre.Contains(busquedaGlobal) || x.Apellido.Contains(busquedaGlobal)))
                    .ToList();
                var ListaADevolver = AutoMapper.Instancia.ConvertirLista<LogicaPadre, Padre>(Lista)
                    .Skip(paginaActual * totalPorPagina).Take(totalPorPagina).ToArray();
                if (ListaADevolver == null)
                    ListaADevolver = new Padre[] { };
                return new Grilla<Padre>()
                {
                    Lista = ListaADevolver,
                    CantidadLogicaUsuario = Lista.Count()
                };
            }
            else
            {
                return new Grilla<Padre>()
                {
                    Lista = new Padre[] { },
                    CantidadLogicaUsuario = 0
                };
            };
        }

        public Hijo[] ObtenerPersonas(UsuarioLogueado usuarioLogueado)
        {
            Resultado Resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            var LogicaUsuario = Archivo.Instancia.Leer<LogicaUsuario>().Find(x => x.Email == usuarioLogueado.Email && x.Eliminado == false && x.Nombre == usuarioLogueado.Nombre);
            if (Resultado.EsValido)
                return AutoMapper.Instancia.ConvertirLista<LogicaHijo, Hijo>(
                    Archivo.Instancia.Leer<LogicaHijo>().FindAll(x => x.IdInstitucion == LogicaUsuario.IdInstitucion && x.Eliminado == false)) // retorno los alumnos que estan en la institucion
                    .ToArray();
            if (LogicaUsuario.RolSeleccionado == Roles.Docente)
            {
                var LogicaDocente = Archivo.Instancia.Leer<LogicaDocente>().Find(x => x.Email == usuarioLogueado.Email && x.Eliminado == false && x.Nombre == usuarioLogueado.Nombre);
                var logicaSalasId = LogicaDocente.Salas.Select(x => x.Id).ToList();
                return AutoMapper.Instancia.ConvertirLista<LogicaHijo, Hijo>(
                       Archivo.Instancia.Leer<LogicaHijo>().FindAll(al => al.IdInstitucion == LogicaDocente.IdInstitucion && al.Eliminado == false && logicaSalasId.Contains(al.Sala.Id)))
                       .ToArray();
            }
            if (LogicaUsuario.RolSeleccionado == Roles.Padre)
            {
                var LogicaPadre = Archivo.Instancia.Leer<LogicaPadre>().Find(x => x.Email == usuarioLogueado.Email && x.Eliminado == false && x.Nombre == usuarioLogueado.Nombre);
                var LogicaHijoId = LogicaPadre.Hijos.Select(x => x.Id).ToList();
                return AutoMapper.Instancia.ConvertirLista<LogicaHijo, Hijo>(
                       Archivo.Instancia.Leer<LogicaHijo>().FindAll(hijo => hijo.IdInstitucion == LogicaPadre.IdInstitucion && hijo.Eliminado == false && LogicaHijoId.Contains(hijo.Id)))
                       .ToArray();
            }
            if (LogicaUsuario.RolSeleccionado == null)
            {
                var alumno = Archivo.Instancia.Leer<LogicaHijo>().Find(x => x.Email == usuarioLogueado.Email && x.Eliminado == false && x.Nombre == usuarioLogueado.Nombre);
                return new Hijo[] { AutoMapper.Instancia.Mapear<LogicaHijo, Hijo>(alumno) };
            }

            return new Hijo[] { };
        }

        public Sala[] ObtenerSalasPorInstitucion(UsuarioLogueado usuarioLogueado)
        {
            if (usuarioLogueado.RolSeleccionado == Roles.Directora)
            {
                var User = Archivo.Instancia.Leer<LogicaDirectora>().Find(y => y.Eliminado == false && y.Email == usuarioLogueado.Email);
                List<LogicaSala> Salas = Archivo.Instancia.Leer<LogicaSala>().FindAll(x =>
                x.Eliminado == false &&
                x.IdInstitucion == User.IdInstitucion);
                return Salas == null ?
                    new Sala[0] :
                    AutoMapper.Instancia.ConvertirLista<LogicaSala, Sala>(Salas).ToArray();
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
                    List<LogicaSala> ListaSalas = docent.Salas.ToList();
                    return AutoMapper.Instancia.ConvertirLista<LogicaSala, Sala>(ListaSalas).ToArray();
                }
                else
                    return null;
            }
        }

        public UsuarioLogueado ObtenerUsuario(string email, string clave)
        {
            return AutoMapper.Instancia.Mapear<LogicaUsuario, UsuarioLogueado>(Archivo.Instancia.Leer<LogicaUsuario>().FirstOrDefault(x =>
                                                                                x.Email == email &&
                                                                                x.Password == clave &&
                                                                                x.Eliminado == false));
        }

        public Resultado ResponderNota(Nota nota, Comentario nuevoComentario, UsuarioLogueado usuarioLogueado)
        {
            Resultado Resultado = Empresa.PermisosDirectora(usuarioLogueado.RolSeleccionado, usuarioLogueado);
            if (nuevoComentario.Mensaje == "")
            {
                Resultado.Errores.Add("No hay ningun comentario para agregar");
                return Resultado;
            }
            var User = Archivo.Instancia.Leer<LogicaUsuario>().Find(x => x.Email == usuarioLogueado.Email && x.Eliminado == false);
            var NotaLogica = Archivo.Instancia.Leer<LogicaNota>().Find(x => x.Id == nota.Id);
            var Alumno = Archivo.Instancia.Leer<LogicaHijo>().Find(x => x.Notas.Select(y => y.Id).Contains(NotaLogica.Id) && x.Eliminado == false);
            var NuevoComentarioLogica = AutoMapper.Instancia.Mapear<Comentario, LogicaComentario>(nuevoComentario);
            NuevoComentarioLogica.Usuario = User;
            if (Resultado.EsValido)
            {
                if (Empresa.MismaInstitucion(Alumno.Id, usuarioLogueado.Email))
                {
                    try
                    {
                        Alumno.Notas.Find(x => x.Id == NotaLogica.Id).Comentarios.Add(NuevoComentarioLogica);
                        NotaLogica.Comentarios.Add(NuevoComentarioLogica);
                        Archivo.Instancia.Guardar(NotaLogica);
                        Archivo.Instancia.Guardar(Alumno);
                        var padres = Archivo.Instancia.Leer<LogicaPadre>().FindAll(x => x.Hijos.Exists(y => y.Id == Alumno.Id) && x.Eliminado == false);
                        foreach (var p in padres)
                        {
                            p.Hijos.RemoveAll(x => x.Id == Alumno.Id);
                            p.Hijos.Add(Alumno);
                            Archivo.Instancia.Guardar(p);
                        }
                    }
                    catch (Exception)
                    {
                        Resultado.Errores.Add("Error al dar de alta la nota");
                        return Resultado;
                    }
                }
                else
                    Resultado.Errores.Add("Error 403: Directora no pertenece a la misma institucion.");
            }
            else
            {
                if (User.RolSeleccionado == Roles.Docente)
                {
                    var Docente = Archivo.Instancia.Leer<LogicaDocente>().Find(x => x.Email == usuarioLogueado.Email && x.Eliminado == false);
                    var SalasDocente = Docente.Salas.Select(x => x.Id);
                    if (Empresa.MismaInstitucion(Alumno.Id, usuarioLogueado.Email) && SalasDocente.Contains(Alumno.Sala.Id)) // si van a la misma institucion y a la misma sala
                    {
                        try
                        {
                            Alumno.Notas.Find(x => x.Id == NotaLogica.Id).Comentarios.Add(NuevoComentarioLogica);
                            NotaLogica.Comentarios.Add(NuevoComentarioLogica);
                            Archivo.Instancia.Guardar(NotaLogica);
                            Archivo.Instancia.Guardar(Alumno);
                            var Padres = Archivo.Instancia.Leer<LogicaPadre>().FindAll(x => x.Hijos.Exists(y => y.Id == Alumno.Id) && x.Eliminado == false);
                            foreach (var p in Padres)
                            {
                                p.Hijos.RemoveAll(x => x.Id == Alumno.Id);
                                p.Hijos.Add(Alumno);
                                Archivo.Instancia.Guardar(p);
                            }
                        }
                        catch (Exception)
                        {
                            Resultado.Errores.Add("Error al dar de alta la nota");
                            return Resultado;
                        }
                    }
                    else
                        Resultado.Errores.Add("Error 403: Docente no pertenece a la misma institucion.");
                }

                if (User.RolSeleccionado == Roles.Padre)
                {
                    var Padre = Archivo.Instancia.Leer<LogicaPadre>().Find(x => x.Email == usuarioLogueado.Email && x.Eliminado == false);
                    var HijosPadreId = Padre.Hijos.Select(x => x.Id).ToList();
                    if (Empresa.MismaInstitucion(Alumno.Id, usuarioLogueado.Email) && HijosPadreId.Contains(Alumno.Id)) // si es su hijo
                    {
                        try
                        {
                            Alumno.Notas.Find(x => x.Id == NotaLogica.Id).Comentarios.Add(NuevoComentarioLogica);
                            NotaLogica.Comentarios.Add(NuevoComentarioLogica);
                            Archivo.Instancia.Guardar(NotaLogica);
                            Archivo.Instancia.Guardar(Alumno);
                            var Padres = Archivo.Instancia.Leer<LogicaPadre>().FindAll(x => x.Hijos.Exists(y => y.Id == Alumno.Id) && x.Eliminado == false);
                            foreach (var p in Padres)
                            {
                                p.Hijos.RemoveAll(x => x.Id == Alumno.Id);
                                p.Hijos.Add(Alumno);
                                Archivo.Instancia.Guardar(p);
                            }
                        }
                        catch (Exception)
                        {
                            Resultado.Errores.Add("Error al dar de alta la nota");
                            return Resultado;
                        }
                    }
                    else
                        Resultado.Errores.Add("Error 403: Padre no pertenece a la misma institucion.");
                }
            }
            return Resultado;
        }
    }
}
