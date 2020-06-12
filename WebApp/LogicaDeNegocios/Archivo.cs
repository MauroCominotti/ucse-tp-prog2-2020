using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace LogicaDeNegocios
{
    public sealed class Archivo
    {
        //Carpeta , archivos y ruta
        private string ruta;
        private const string LogicaDirectora = @"LogicaDirectora.txt";
        private const string LogicaUsuario = @"LogicaUsuario.txt";
        private const string LogicaPadre = @"LogicaPadre.txt";
        private const string LogicaHijo = @"LogicaHijo.txt";
        private const string LogicaDocente = @"LogicaDocente.txt";
        private const string LogicaSala = @"LogicaSala.txt";
        //private const string LogicaNota = @"LogicaNota.txt";
        private const string LogicaInstitucion = @"LogicaInstitucion.txt";
        private string carpeta = AppDomain.CurrentDomain.BaseDirectory; //Para definir que los archivos se guarden en la carpeta del proyecto, o sea la carpeta base(webapp)
        private string[] arrayRutas = { LogicaDirectora, LogicaUsuario, LogicaInstitucion, LogicaPadre, LogicaHijo, LogicaDocente, LogicaSala };

        // TODO > Crear listas que al hacer get leer de los archivos y cuando modifiquen el set se correlacione con Guardar
        //SINGLETON //////////////////////////////////////////////////////////////////////////////
        private static Archivo instancia = null;
        public static Archivo Instancia
        {
            get
            {
                if (instancia == null)
                    instancia = new Archivo();
                return instancia;
            }
        }

        public EventHandler eventoAlta;
        public EventHandler eventoBaja;
        public EventHandler eventoModificacion;
        public EventHandler eventoLectura; 

        //Buscar en Archivos //////////////////////////////////////////////////////////////////////////////
        public List<T> Leer<T>()
        {
            // c# inferencia de tipo en metodo generico
            // https://sodocumentation.net/es/csharp/topic/27/genericos
            List<T> listusu = new List<T>();
            string rutaSeleccionada = arrayRutas.First(x => x.Contains(typeof(T).Name));
            ruta = Path.Combine(carpeta, rutaSeleccionada);
            eventoLectura(rutaSeleccionada, null);
            if (!File.Exists(ruta))
            {
                var file = File.Create(ruta);
                file.Close();
                List<T> lista = new List<T>();
                return lista;
            }
            else
            {
                using (StreamReader re = new StreamReader(ruta))
                {
                    string text = re.ReadToEnd();
                    listusu = JsonConvert.DeserializeObject<List<T>>(text);
                }
                return listusu;
            }
        }


        public void ObtenerListaGeneral() // TODO > Actualiza la lista de Usuarios
        {
            List<LogicaUsuario> usuarios = new List<LogicaUsuario>();
            var listadir = Leer<LogicaDirectora>();
            var listadoc = Leer<LogicaDocente>();
            var listapadre = Leer<LogicaPadre>();
            var listahijo = Leer<LogicaHijo>();
            if (listadir != null)
                usuarios.AddRange(listadir.Select(x => x as LogicaUsuario));
            if (listadoc != null)
                usuarios.AddRange(listadoc.Select(x => x as LogicaUsuario));
            if (listapadre != null)
                usuarios.AddRange(listapadre.Select(x => x as LogicaUsuario));
            if (listahijo != null)
                usuarios.AddRange(listahijo.Select(x => x as LogicaUsuario));

            Guardar(usuarios);
        }

        //Guardar en archivos //////////////////////////////////////////////////////////////////////////////
        public void Guardar(LogicaInstitucion institucion, bool suprimir = false)
        {
            string rutaarchivo = Path.Combine(carpeta, LogicaInstitucion);
            List<LogicaInstitucion> listinstitucion = new List<LogicaInstitucion>();
            listinstitucion = Leer<LogicaInstitucion>();
            int cont = 0; bool ban = true;
            if (listinstitucion != null)
            {
                foreach (var item in listinstitucion)
                {
                    if (item.Id == institucion.Id)
                    {
                        if (suprimir)
                        {
                            item.Eliminado = true;
                            eventoBaja(this, null);
                        }
                        else
                            eventoModificacion(this, null);
                        listinstitucion.RemoveAt(cont);
                        listinstitucion.Insert(cont, item);

                        ban = false;
                        break;
                    }
                    cont++;
                }
                if (ban)
                {
                    listinstitucion.Add(institucion);
                    eventoAlta(this, null);
                }
            }
            else
            {
                listinstitucion = new List<LogicaInstitucion>();
                listinstitucion.Add(institucion);
            }

            using (StreamWriter escribir = new StreamWriter(rutaarchivo, false))
            {
                string Serializar = JsonConvert.SerializeObject(listinstitucion);
                escribir.Write(Serializar);

            }
        }

        public void Guardar(LogicaSala sala, bool suprimir = false)
        {
            string rutaarchivo = Path.Combine(carpeta, LogicaSala);
            List<LogicaSala> listsala = new List<LogicaSala>();
            listsala = Leer<LogicaSala>();
            int cont = 0; bool ban = true;
            if (listsala != null)
            {
                foreach (var item in listsala)
                {
                    if (item.Id == sala.Id)
                    {
                        if (suprimir)
                        {
                            item.Eliminado = true;
                            eventoBaja(this, null);
                        }
                        else
                            eventoModificacion(this, null);
                        listsala.RemoveAt(cont);
                        listsala.Insert(cont, item);

                        ban = false;
                        break;
                    }
                    cont++;
                }
                if (ban)
                {
                    listsala.Add(sala);
                    eventoAlta(this, null);
                }
            }
            else
            {
                listsala = new List<LogicaSala>();
                listsala.Add(sala);
            }

            using (StreamWriter escribir = new StreamWriter(rutaarchivo, false))
            {
                string Serializar = JsonConvert.SerializeObject(listsala);
                escribir.Write(Serializar);

            }
        }

        public void Guardar(List<LogicaUsuario> usu, bool suprimir = false)
        {
            string rutas = Path.Combine(carpeta, LogicaUsuario);
            //List<LogicaUsuario> listusu = new List<LogicaUsuario>();
            //listusu = Leer<LogicaUsuario>();
            //int cont = 0; bool br = true;
            //if (listusu != null)
            //{
            //    foreach (var item in listusu)
            //    {
            //        if (item.Id == usu.Id)
            //        {
            //            if (suprimir)
            //            {
            //                item.Eliminado = true;
            //                eventoBaja(this, null);
            //            }
            //            else
            //                eventoModificacion(this, null);
            //            listusu.RemoveAt(cont);
            //            listusu.Insert(cont, item);

            //            br = false;
            //            break;
            //        }
            //        cont++;
            //    }
            //    if (br)
            //    {
            //        listusu.Add(usu);
            //        eventoAlta(this, null);
            //    }
            //}
            //else
            //{
            //    listusu = new List<LogicaUsuario>();
            //    listusu.Add(usu);
            //}

            using (StreamWriter escribir = new StreamWriter(rutas, false))
            {
                string Serializar = JsonConvert.SerializeObject(usu);
                escribir.Write(Serializar);
                escribir.Close();
            }
        }

        public void Guardar(LogicaDocente doc, bool suprimir = false)
        {
            string rutaarchivo = Path.Combine(carpeta, LogicaDocente);
            List<LogicaDocente> listdoc = new List<LogicaDocente>();
            listdoc = Leer<LogicaDocente>();
            int cont = 0; bool ban = true; // TODO > Sacar bandera siempre PascalCase
            if (listdoc != null)
            {
                foreach (var item in listdoc)
                {
                    if (item.Id == doc.Id)
                    {
                        if (suprimir)
                        {
                            item.Eliminado = true;
                            eventoBaja(this, null);
                        }
                        else
                        {
                            eventoModificacion(this, null);
                            item.Nombre = doc.Nombre;
                            item.Apellido = doc.Apellido;
                            item.Email = doc.Email;
                            if (doc.Salas.Length != 0)
                            {
                                var salaDoc = Archivo.Instancia.Leer<LogicaSala>().Find(x => x.Id == doc.Salas[0].Id);

                                if (item.Salas.FirstOrDefault(x => x.Id == salaDoc.Id) == null)
                                {
                                    var salas = new LogicaSala[item.Salas.Length + 1];
                                    for (int i = 0; i < salas.Length; i++)
                                    {
                                        if (i == salas.Length - 1)
                                            salas[i] = salaDoc;
                                        else
                                            salas[i] = item.Salas[i];
                                    }
                                    item.Salas = salas;
                                }
                            }
                            else
                                item.Salas = new LogicaSala[] { };
                        }
                        listdoc.RemoveAt(cont);
                        listdoc.Insert(cont, item);

                        ban = false;
                        break;
                    }
                    cont++;
                }
                if (ban)
                {
                    listdoc.Add(doc);
                    eventoAlta(this, null);
                }
            }
            else
            {
                listdoc = new List<LogicaDocente>();
                listdoc.Add(doc);
            }

            using (StreamWriter escribir = new StreamWriter(rutaarchivo, false))
            {
                string Serializar = JsonConvert.SerializeObject(listdoc);
                escribir.Write(Serializar);
            }
            ObtenerListaGeneral();
        }

        public void Guardar(LogicaPadre doc, bool suprimir = false)
        {
            string rutaarchivo = Path.Combine(carpeta, LogicaPadre);
            List<LogicaPadre> listdoc = new List<LogicaPadre>();
            listdoc = Leer<LogicaPadre>();
            int cont = 0; bool ban = true;
            if (listdoc != null)
            {
                foreach (var item in listdoc)
                {
                    if (item.Id == doc.Id)
                    {
                        if (suprimir)
                        {
                            item.Eliminado = true;
                            eventoBaja(this, null);
                        }
                        else
                        {
                            eventoModificacion(this, null);
                            item.Nombre = doc.Nombre;
                            item.Apellido = doc.Apellido;
                            item.Email = doc.Email;
                            if (doc.Hijos.Length != 0)
                            {
                                var hijoDoc = Archivo.Instancia.Leer<LogicaHijo>().Find(x => x.Id == doc.Hijos[0].Id);

                                if (item.Hijos.FirstOrDefault(x => x.Id == hijoDoc.Id) == null)
                                {
                                    var hijos = new LogicaHijo[item.Hijos.Length + 1];
                                    for (int i = 0; i < hijos.Length; i++)
                                    {
                                        if (i == hijos.Length - 1)
                                            hijos[i] = hijoDoc;
                                        else
                                            hijos[i] = item.Hijos[i];
                                    }
                                    item.Hijos = hijos;
                                }
                            }
                            else
                                item.Hijos = new LogicaHijo[] { };
                        }
                        listdoc.RemoveAt(cont);
                        listdoc.Insert(cont, item);
                        ban = false;
                        break;
                    }
                    cont++;
                }
                if (ban)
                {
                    listdoc.Add(doc);
                    eventoAlta(this, null);
                }
            }
            else
            {
                listdoc = new List<LogicaPadre>();
                listdoc.Add(doc);
            }

            using (StreamWriter escribir = new StreamWriter(rutaarchivo, false))
            {
                string Serializar = JsonConvert.SerializeObject(listdoc);
                escribir.Write(Serializar);

            }
            ObtenerListaGeneral();
        }

        public void Guardar(LogicaDirectora directivo, bool suprimir = false)
        {
            string rutas = Path.Combine(carpeta, LogicaDirectora);
            List<LogicaDirectora> listreg = new List<LogicaDirectora>();
            listreg = Leer<LogicaDirectora>();
            int cont = 0; bool br = true;
            if (listreg != null)
            {
                foreach (var item in listreg)
                {
                    if (item.Id == directivo.Id)
                    {
                        if (suprimir)
                        {
                            item.Eliminado = true;
                            eventoBaja(this, null);
                        }
                        else
                        {
                            eventoModificacion(this, null); // TODO > Cambiar todo refactorizar props
                            item.Nombre = directivo.Nombre;
                            item.Apellido = directivo.Apellido;
                            item.Email = directivo.Email;
                            item.Cargo = directivo.Cargo;
                            item.FechaIngreso = directivo.FechaIngreso;
                        }
                        listreg.RemoveAt(cont);
                        listreg.Insert(cont, item);

                        br = false;
                        break;
                    }
                    cont++;
                }
                if (br)
                {
                    listreg.Add(directivo);
                    eventoAlta(this, null);
                }
            }
            else
            {
                listreg = new List<LogicaDirectora>();
                listreg.Add(directivo);
            }
            using (StreamWriter escribir = new StreamWriter(rutas, false))
            {
                string Serializar = JsonConvert.SerializeObject(listreg);
                escribir.Write(Serializar);

            }
            ObtenerListaGeneral();
        }

        public void Guardar(LogicaHijo alumno, bool suprimir = false)
        {
            string rutas = Path.Combine(carpeta, LogicaHijo);
            List<LogicaHijo> listalum = new List<LogicaHijo>();
            listalum = Leer<LogicaHijo>();
            int cont = 0; bool br = true;
            if (listalum != null)
            {
                foreach (var item in listalum)
                {
                    if (item.Id == alumno.Id)
                    {
                        if (suprimir)
                        {
                            item.Eliminado = true;
                            eventoBaja(this, null);
                        }
                        else
                        {
                            eventoModificacion(this, null);
                            item.Nombre = alumno.Nombre;
                            item.Apellido = alumno.Apellido;
                            item.Email = alumno.Email;
                            item.Sala = alumno.Sala;
                            item.Sala.IdInstitucion = item.IdInstitucion;
                            item.ResultadoUltimaEvaluacionAnual = alumno.ResultadoUltimaEvaluacionAnual;
                            item.FechaNacimiento = alumno.FechaNacimiento;
                        }
                        listalum.RemoveAt(cont);
                        listalum.Insert(cont, item);

                        br = false;
                        break;
                    }
                    cont++;
                }
                if (br)
                {
                    listalum.Add(alumno);
                    eventoAlta(this, null);
                }
            }
            else
            {
                listalum = new List<LogicaHijo>();
                listalum.Add(alumno);
            }

            using (StreamWriter escribir = new StreamWriter(ruta, false))
            {
                string Serializar = JsonConvert.SerializeObject(listalum);
                escribir.Write(Serializar);

            }
            ObtenerListaGeneral();
        }


    }
}
