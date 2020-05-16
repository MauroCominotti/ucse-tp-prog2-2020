using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contratos;
using Newtonsoft.Json;

namespace Lógica_de_Negocios
{
   public class Archivos
    {
        
        //Carpeta , archivos y ruta
        private string ruta;

        private const string directoras = @"directoras.txt";

        private const string usuarios = @"usuarios.txt";

        private const string padres = @"padres.txt";

        private const string hijos = @"hijos.txt";

        private const string docentes = @"docentes.txt";

        private const string salas = @"salas.txt";

        private const string registros = @"registros.txt";

        private const string notas = @"notas.txt";
        private string carpeta = AppDomain.CurrentDomain.BaseDirectory;//Para definir que los archivos se guarden en la carpeta del proyecto, o sea la carpeta base(webapp)

        //SINGLETON---------------------------------------------
        private static Archivos instancia = null;
        public static Archivos Instancia
        {
            get
            {
                if (instancia == null)
                    instancia = new Archivos();
                return instancia;
            }
        }


        //Buscar en Archivos----------------------------------------------------------
        public List<Usuario> AdquirirUsuario()
        {
            List<Usuario> listusu = new List<Usuario>();
            ruta = Path.Combine(carpeta, usuarios);
            using (StreamReader re = new StreamReader(ruta))
            {
                string text = re.ReadToEnd();
                listusu = JsonConvert.DeserializeObject<List<Usuario>>(text);
            }
            return listusu;
        }

        public List<Registros> AdquirirRegistros()
        {
            List<Registros> reg = new List<Registros>();
            ruta = Path.Combine(carpeta, registros);
            using (StreamReader re = new StreamReader(ruta))
            {
                string text = re.ReadToEnd();
                reg = JsonConvert.DeserializeObject<List<Registros>>(text);
            }
            return reg;
        }

        public List<Directora> AdquirirDirectoras()
        {
            List<Directora> listdir = new List<Directora>();
            ruta = Path.Combine(carpeta, directoras);
            using (StreamReader re = new StreamReader(ruta))
            {
                string text = re.ReadToEnd();
                listdir = JsonConvert.DeserializeObject<List<Directora>>(text);
            }
            return listdir;
        }

        public List<Hijo> AdquirirAlumnos()
        {
            List<Hijo> listalum = new List<Hijo>();
            ruta = Path.Combine(carpeta, hijos);
            using (StreamReader re = new StreamReader(ruta))
            {
                string json = re.ReadToEnd();
                listalum = JsonConvert.DeserializeObject<List<Hijo>>(json);
            }
            return listalum;
        }

        //Guardar en archivos----------------------------------------------------
        public void EscribirRegistro(Registros reg, bool suprimir)
        {
            string rutas = Path.Combine(carpeta, registros);
            List<Registros> listreg = new List<Registros>();
            listreg = AdquirirRegistros();
            if (listreg != null)
            {
                int cont = 0; bool br = true;
                foreach (var item in listreg)
                {
                    if (item.Id == reg.Id)
                    {
                        if (suprimir)
                            listreg.RemoveAt(cont);
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


        public void EscribirDirectora(Directora directivo, bool suprimir)
        {
            string rutas = Path.Combine(carpeta, directoras);
            List<Directora> listreg = new List<Directora>();
            listreg = AdquirirDirectoras();
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
                listreg = new List<Directora>();
                listreg.Add(directivo);
            }
            using (StreamWriter escritor = new StreamWriter(rutas, false))
            {
                string Serializar = JsonConvert.SerializeObject(listreg);
                escritor.Write(Serializar);
            }
        }

        public void EscribirUsuario(Usuario usu, bool suprimir)
        {

            string rutas = Path.Combine(carpeta, usuarios);
            List<Usuario> listusu = new List<Usuario>();
            listusu = AdquirirUsuario();
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
                listusu = new List<Usuario>();
                listusu.Add(usu);
            }

            using (StreamWriter escritor = new StreamWriter(ruta, false))
            {
                string Serializar = JsonConvert.SerializeObject(listusu);
                escritor.Write(Serializar);
            }
        }

        public void EscribirAlumno(Hijo alumno, bool suprimir)
        {
            string rutas = Path.Combine(carpeta, hijos);
            List<Hijo> listalum = new List<Hijo>();
            listalum = AdquirirAlumnos();
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
                listalum = new List<Hijo>();
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
