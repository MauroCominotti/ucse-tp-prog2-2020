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
        private const string LogicaNota = @"LogicaNota.txt";
        private const string LogicaInstitucion = @"LogicaInstitucion.txt";
        private string carpeta = AppDomain.CurrentDomain.BaseDirectory; //Para definir que los archivos se guarden en la carpeta del proyecto, o sea la carpeta base(webapp)
        private string[] arrayRutas = { LogicaDirectora, LogicaUsuario, LogicaInstitucion, LogicaPadre, LogicaHijo, LogicaDocente, LogicaSala, LogicaNota };

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

        //Buscar en Archivos //////////////////////////////////////////////////////////////////////////////
        public List<T> Leer<T>()
        {
            // c# inferencia de tipo en metodo generico
            // https://sodocumentation.net/es/csharp/topic/27/genericos
            List<T> listusu = new List<T>();
            string rutaSeleccionada = arrayRutas.First(x => x.Contains(typeof(T).Name));
            ruta = Path.Combine(carpeta, rutaSeleccionada);
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
            var listadpadre = Leer<LogicaPadre>();
            var listahijo = Leer<LogicaHijo>();
            if (listadir != null)
                usuarios.AddRange(listadir.Select(x => x as LogicaUsuario));
            if (listadoc != null)
                usuarios.AddRange(listadoc.Select(x => x as LogicaUsuario));
            if (listadpadre != null)
                usuarios.AddRange(listadir.Select(x => x as LogicaUsuario));
            if (listahijo != null)
                usuarios.AddRange(listahijo.Select(x => x as LogicaUsuario));
            
            usuarios.ForEach(x => Guardar(x));
        }

        //Guardar en archivos //////////////////////////////////////////////////////////////////////////////
        public void Guardar(LogicaInstitucion institucion, bool suprimir = false)
        {
            string rutaarchivo = Path.Combine(carpeta, LogicaDocente);
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
                            institucion.Eliminado = true;
                        else
                        {
                            listinstitucion.RemoveAt(cont);
                            listinstitucion.Insert(cont, institucion);
                        }
                        ban = false;
                        break;
                    }
                    cont++;
                }
                if (ban)
                    listinstitucion.Add(institucion);
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

        public void Guardar(LogicaUsuario usu, bool suprimir = false)
        {
            string rutas = Path.Combine(carpeta, LogicaUsuario);
            List<LogicaUsuario> listusu = new List<LogicaUsuario>();
            listusu = Leer<LogicaUsuario>();
            int cont = 0; bool br = true;
            if (listusu != null)
            {
                foreach (var item in listusu)
                {
                    if (item.Id == usu.Id)
                    {
                        if (suprimir)
                            usu.Eliminado = true;
                        else
                        {
                            listusu.RemoveAt(cont);
                            listusu.Insert(cont, usu);
                        }
                        br = false;
                        break;
                    }
                    cont++;
                }
                if (br)
                    listusu.Add(usu);
            }
            else
            {
                listusu = new List<LogicaUsuario>();
                listusu.Add(usu);
            }

            using (StreamWriter escribir = new StreamWriter(ruta, false))
            {
                string Serializar = JsonConvert.SerializeObject(listusu);
                escribir.Write(Serializar);
            }
        }

        public void Guardar(LogicaDocente doc, bool suprimir = false)
        {
            string rutaarchivo = Path.Combine(carpeta, LogicaDocente);
            List<LogicaDocente> listdoc = new List<LogicaDocente>();
            listdoc = Leer<LogicaDocente>();
            int cont = 0; bool ban = true;
            if (listdoc != null)
            {
                foreach (var item in listdoc)
                {
                    if (item.Id == doc.Id)
                    {
                        if (suprimir)
                            doc.Eliminado = true;
                        else
                        {
                            listdoc.RemoveAt(cont);
                            listdoc.Insert(cont, doc);
                        }
                        ban = false;
                        break;
                    }
                    cont++;
                }
                if (ban)
                    listdoc.Add(doc);
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
            string rutaarchivo = Path.Combine(carpeta, LogicaDocente);
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
                            doc.Eliminado = true;

                        listdoc.RemoveAt(cont);
                        listdoc.Insert(cont, doc);
                        ban = false;
                        break;
                    }
                    cont++;
                }
                if (ban)
                    listdoc.Add(doc);
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
                            directivo.Eliminado = true;
                        else
                        {
                            listreg.RemoveAt(cont);
                            listreg.Insert(cont, directivo);
                        }
                        br = false;
                        break;
                    }
                    cont++;
                }
                if (br)
                    listreg.Add(directivo);
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
                            alumno.Eliminado = true;
                        else
                        {
                            listalum.RemoveAt(cont);
                            listalum.Insert(cont, alumno);
                        }
                        br = false;
                        break;
                    }
                    cont++;
                }
                if (br)
                    listalum.Add(alumno);
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
