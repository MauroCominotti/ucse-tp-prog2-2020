using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lógica_de_Negocios
{
    public static class Empresa
    {
        public static List<LogicaDirectora> ListaDirectoras { get; set; }
        public static List<LogicaDirectora> ObtenerDirectoras() => ListaDirectoras;
    }
}
