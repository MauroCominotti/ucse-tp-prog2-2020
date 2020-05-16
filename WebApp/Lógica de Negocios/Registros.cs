using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contratos;

namespace Lógica_de_Negocios
{
    public class Registros
    {
        public int Id { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public Roles[] Roles { get; set; }
    }
}
