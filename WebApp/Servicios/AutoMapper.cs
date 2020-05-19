using AutoMapper;
using Contratos;
using Lógica_de_Negocios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios
{
    public sealed class AutoMapper
    {
        private static AutoMapper instancia = null;
        public static AutoMapper Instancia
        {
            get
            {
                if (instancia == null)
                    instancia = new AutoMapper();
                return instancia;
            }
        }

        // AUTOMAPPER
        //https://automapper.org/  https://docs.automapper.org/en/latest/Getting-started.html  
        //https://docs.automapper.org/en/latest/Setup.html  https://docs.automapper.org/en/stable/Nested-mappings.html
        //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/generics/generic-methods
        //https://stackoverflow.com/questions/2690623/what-is-the-dynamic-type-in-c-sharp-4-0-used-for

        // Ejemplo para usar la funcion ==> Mappear<T,U> (LogicaDirectora x, new Directora())
        public dynamic Mapear<T, D>(T source, D dest)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<LogicaDirectora, Directora>();
                cfg.CreateMap<LogicaInstitucion, Institucion>();
                cfg.CreateMap<LogicaSala, Sala>();
                cfg.CreateMap<LogicaUsuario, Usuario>();
                cfg.CreateMap<LogicaPadre, Padre>();
                cfg.CreateMap<LogicaHijo, Hijo>();
                cfg.CreateMap<LogicaNota, Nota>();
                cfg.CreateMap<LogicaComentario, Comentario>();
            });
            config.AssertConfigurationIsValid();
            return new Mapper(config).Map<D>(source); 
        }
        //public dynamic Mapear<T,D> (T source, D dest)
        //{
        //    var config = new MapperConfiguration(cfg => cfg.CreateMap<T, D>());
        //    //var mapper = new Mapper(config);
        //    //dest = mapper.Map<D>(source);
        //    dest = new Mapper(config).Map<D>(source);
        //    return dest;
        //}


        // Automappear Lista //////////////////////////////////////////////////////////////////////////////
        public List<U> ConvertirLista<T, U>(List<T> lista, U args)
        {
            List<U> resultado = new List<U>();
            foreach (var elem in lista)
            {
                resultado.Add(Mapear(elem, args));
            }
            return resultado;
        }

    }
}
