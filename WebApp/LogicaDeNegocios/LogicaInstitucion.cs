using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaDeNegocios
{
    public class LogicaInstitucion : IEquatable<LogicaInstitucion>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Ciudad { get; set; }
        public string Provincia { get; set; }
        public bool Eliminado { get; set; }

        // https://docs.microsoft.com/en-us/dotnet/api/system.linq.enumerable.distinct?view=netcore-3.1
        public bool Equals(LogicaInstitucion other)
        {

            //Check whether the compared object is null.
            if (Object.ReferenceEquals(other, null)) return false;

            //Check whether the compared object references the same data.
            if (Object.ReferenceEquals(this, other)) return true;

            //Check whether the products' properties are equal.
            return Nombre.Equals(other.Nombre) && Direccion.Equals(other.Direccion);
        }

        // If Equals() returns true for a pair of objects
        // then GetHashCode() must return the same value for these objects.

        public override int GetHashCode() // TODO > Nose si es necesaria esta parte
        {

            //Get hash code for the Name field if it is not null.
            int hashProductName = Nombre == null ? 0 : Nombre.GetHashCode();

            //Get hash code for the Code field.
            int hashProductCode = Direccion.GetHashCode();

            //Calculate the hash code for the product.
            return hashProductName ^ hashProductCode;
        }

    }
}
