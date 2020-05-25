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
        private const string LogicaDirectora = @"directoras.txt"; // TODO > YA HECHO - cambiar nombre de prop para que coincidan con las clases
        private const string LogicaUsuario = @"usuarios.txt";
        private const string LogicaPadre = @"padres.txt";
        private const string LogicaHijo = @"hijos.txt";
        private const string LogicaDocente = @"docentes.txt";
        private const string LogicaSala = @"salas.txt";
        private const string Registros = @"registros.txt";
        private const string LogicaNota = @"notas.txt";
        private string carpeta = AppDomain.CurrentDomain.BaseDirectory;//Para definir que los archivos se guarden en la carpeta del proyecto, o sea la carpeta base(webapp)
        private string[] arrayRutas = { LogicaDirectora, LogicaUsuario, LogicaPadre, LogicaHijo, LogicaDocente, LogicaSala, Registros, LogicaNota }; 

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

        public static List<T> VerificarExistencia <T>(string r, T obj)
        {
            File.Create(r);
            List<T> lista = new List<T>();
            return lista;
        }

        //Buscar en Archivos //////////////////////////////////////////////////////////////////////////////
        public List<T> Leer<T>()
        {
            // c# inferencia de tipo en metodo generico
            // https://sodocumentation.net/es/csharp/topic/27/genericos
            List<T> listusu = new List<T>();
            string rutaSeleccionada = arrayRutas.First(x => x == typeof(T).Name);
            ruta = Path.Combine(carpeta, rutaSeleccionada);
            if (!File.Exists(ruta))
            {
                File.Create(ruta);
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
        //Guardar en archivos //////////////////////////////////////////////////////////////////////////////
        public void Guardar(Registros reg, bool suprimir)
        {
            string rutas = Path.Combine(carpeta, Registros);
            List<Registros> listreg = new List<Registros>();
            listreg = Leer<Registros>();
            if (listreg != null)
            {
                int cont = 0; bool br = true;
                foreach (var item in listreg)
                {
                    if (item.Id == reg.Id)
                    {
                        if (suprimir)
                            listreg.RemoveAt(cont); // TODO > LA ELIMINACION TENDRIA Q SER UNA ELMINACION LOGICA!!!
                        else
                        {
                            listreg.RemoveAt(cont);
                            listreg.Insert(cont, reg);
                        }
                        br = false;
                        break;
                    }
                    cont++;
                }
                if (br)
                    listreg.Add(reg);
            }
            else
            {
                listreg = new List<Registros>();
                listreg.Add(reg);
            }
            using (StreamWriter escribir = new StreamWriter(ruta, false))
            {
                string Serializar = JsonConvert.SerializeObject(listreg);
                escribir.Write(Serializar);
            }
        }


        public void Guardar(LogicaDirectora directivo, bool suprimir)
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
                            listreg.RemoveAt(cont);
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
            using (StreamWriter escritor = new StreamWriter(rutas, false))
            {
                string Serializar = JsonConvert.SerializeObject(listreg);
                escritor.Write(Serializar);
            }
        }

        public void Guardar(LogicaUsuario usu, bool suprimir)
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
                            listusu.RemoveAt(cont);
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

            using (StreamWriter escritor = new StreamWriter(ruta, false))
            {
                string Serializar = JsonConvert.SerializeObject(listusu);
                escritor.Write(Serializar);
            }
        }

        public void Guardar(LogicaHijo alumno, bool suprimir)
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
                            listalum.RemoveAt(cont);
                        }
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

            using (StreamWriter escritor = new StreamWriter(ruta, false))
            {
                string Serializar = JsonConvert.SerializeObject(listalum);
                escritor.Write(Serializar);
            }
        }


    }
}
