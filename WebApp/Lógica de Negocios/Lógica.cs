using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lógica_de_Negocios
{
    public sealed class Lógica
    {
        private static Lógica instance = null;
        private Lógica()
        {

        }
        public static Lógica Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Lógica();
                }
                return instance;
            }
        }
    }
}
