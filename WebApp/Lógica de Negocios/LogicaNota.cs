using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lógica_de_Negocios
{
    public class LogicaNota
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime? FechaEventoAsociado { get; set; }
        public bool Leida { get; set; }
        public Comentario[] Comentarios { get; set; }
    }
}
