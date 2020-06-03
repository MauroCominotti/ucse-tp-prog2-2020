using AutoMapper;
using Contratos;
using LogicaDeNegocios;
using System.Collections.Generic;
using System.Linq;

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

        // LogicaDirectora directora = new LogicaDirectora();
        // Ejemplo para usar la funcion ==> Mappear<LogicaDirectora, Directora> (directora);
        // correccion para mapear solo algunas propiedades https://stackoverflow.com/questions/954480/automapper-ignore-the-rest/31182390#31182390
        public Destination Mapear<Source, Destination>(Source source)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<LogicaDirectora, Directora>(MemberList.Destination);
                cfg.CreateMap<Directora, LogicaDirectora>(MemberList.Source);
                cfg.CreateMap<LogicaDocente, Docente>(MemberList.Destination);
                cfg.CreateMap<Docente, LogicaDocente>(MemberList.Source);
                cfg.CreateMap<LogicaUsuario, UsuarioLogueado>(MemberList.Destination);
                cfg.CreateMap<UsuarioLogueado, LogicaUsuario>(MemberList.Source);
                cfg.CreateMap<LogicaInstitucion, Institucion>(MemberList.Destination);
                cfg.CreateMap<Institucion, LogicaInstitucion>(MemberList.Source);
                cfg.CreateMap<LogicaSala, Sala>();
                cfg.CreateMap<Sala, LogicaSala>();
                cfg.CreateMap<LogicaUsuario, Usuario>(MemberList.Destination);
                cfg.CreateMap<Usuario, LogicaUsuario>(MemberList.Source); // solo los campos de Source son mapeados
                cfg.CreateMap<LogicaPadre, Padre>(MemberList.Destination);
                cfg.CreateMap<Padre, LogicaPadre>(MemberList.Source);
                cfg.CreateMap<LogicaHijo, Hijo>(MemberList.Destination);
                cfg.CreateMap<Hijo, LogicaHijo>(MemberList.Source);
                cfg.CreateMap<LogicaNota, Nota>();
                cfg.CreateMap<Nota, LogicaNota>();
                cfg.CreateMap<LogicaComentario, Comentario>();
                cfg.CreateMap<Comentario, LogicaComentario>();
            });
            config.AssertConfigurationIsValid();
            return new Mapper(config).Map<Destination>(source);
        }
        // Automappear Lista //////////////////////////////////////////////////////////////////////////////
        public List<Destination> ConvertirLista<Source, Destination>(List<Source> lista)
        {
            List<Destination> resultado = new List<Destination>();
            return lista.Select(x => Mapear<Source, Destination>(x)).ToList();
        }

    }
}
