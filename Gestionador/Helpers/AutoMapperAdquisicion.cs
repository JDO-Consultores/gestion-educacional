using AutoMapper;
using Gestionador.Model;
using System.Collections.Generic;

namespace Gestionador.Helpers
{
    public class AutoMapperAdquisicion : Profile
    {
        public AutoMapperAdquisicion()
        {
            CreateMap<tbl_Adquisicion, tbl_Adquisicion>()
                 .ForMember(dest => dest.ID, opt => opt.Ignore())
                 .ForMember(dest => dest.tbl_ServiciosAdquisicion, opt => opt.Ignore())
                 .ForMember(dest => dest.tbl_PagoAdquisicion, opt => opt.Ignore())
                 .ForMember(dest => dest.tbl_Observaciones, opt => opt.Ignore())
                 .ForMember(dest => dest.tbl_HistorialAdquisicion, opt => opt.Ignore());
        }

        public static bool HasProperty(dynamic obj, string name)
        {
            if (obj is IDictionary<string, object> dict)
            {
                return dict.ContainsKey(name);
            }

            return obj.GetType().GetProperty(name) != null;
        }
    }
}