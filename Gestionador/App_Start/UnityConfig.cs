using AutoMapper;
using Gestionador.Helpers;
using Gestionador.Interfaces;
using Gestionador.Services;
using Microsoft.Extensions.Logging.Abstractions;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

namespace Gestionador
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            container.RegisterType<IAuthenticationService, AuthenticationService>();
            container.RegisterType<IJwtTokenService, JwtTokenService>();
            container.RegisterType<IMapperService, MapperService>();
            container.RegisterType<ISearchService, SearchService>();
            container.RegisterType<ISolicitudesService, SolicitudesServices>();
            container.RegisterType<IUserService, UserServices>();
            container.RegisterType<ILogService, LogServices>();
            container.RegisterType<IServiciosService, ServiciosService>();
            container.RegisterType<IReportService, ReportService>();
            container.RegisterType<IProductosServices, ProductosServices>();
            container.RegisterType<IPatiosService, PatiosServices>();
            container.RegisterType<IConfigService, ConfigService>();
            container.RegisterType<IEmailService, EmailService>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperAdquisicion>();
            }, NullLoggerFactory.Instance);

            IMapper mapper = config.CreateMapper();
            container.RegisterInstance(mapper);
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}