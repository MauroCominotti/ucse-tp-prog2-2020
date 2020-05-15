using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lógica_de_Negocios
{
    public static class Empresa
    {
        static string path = Directory.GetCurrentDirectory().Replace("\\bin\\Debug", "") + @"\Datos\ListaDirectoras.txt";
        public static List<LogicaDirectora> ListaDirectoras { get; set; }
        public static List<LogicaDirectora> ObtenerDirectoras() => ListaDirectoras;


        //--------------------------------- GUARDAR ------------------------------------------------------------------------------------
        static void Guardar(List<LogicaDirectora> Lista)
        {
            using (StreamWriter file = new StreamWriter(path, false))
            {
                string JsonContenido = JsonConvert.SerializeObject(Lista);
                file.Write(JsonContenido);
            }
        }
        //--------------------------------- LEER ------------------------------------------------------------------------------------
        static List<LogicaDirectora> LeerListasDirectoras()
        {
            if (!File.Exists(path))
            {
                File.Create(path);
                List<LogicaDirectora> lista = new List<LogicaDirectora>();
                return lista;
            }
            else
            {
                using (StreamReader file = new StreamReader(path))
                {
                    string JsonContenido = file.ReadToEnd();
                    List<LogicaDirectora> lista = JsonConvert.DeserializeObject<List<LogicaDirectora>>(JsonContenido);

                    if (lista == null)
                        lista = new List<LogicaDirectora>();

                    return (lista);
                }
            }
        }
    }
}
