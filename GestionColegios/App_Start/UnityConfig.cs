using GestionColegios.Interfaces;
using GestionColegios.Services;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

namespace GestionColegios
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            container.RegisterType<IAuthenticationService, AuthenticationService>();
            container.RegisterType<IJwtTokenService, JwtTokenService>();
            container.RegisterType<IMapperService, MapperService>();
            container.RegisterType<IUserService, UserServices>();
            container.RegisterType<IEmailService, EmailService>();
            container.RegisterType<ISearchService, SearchService>();
            container.RegisterType<IAlumnosServices, AlumnosService>();
            container.RegisterType<IMatriculaService, MatriculaService>();
            container.RegisterType<IAnioEscolarService, AnioEscolarService>();
            container.RegisterType<IProfesorJefeService, ProfesorJefeService>();
            container.RegisterType<IApoderadoService, ApoderadoService>();
            container.RegisterType<IHomeService, HomeService>();
            container.RegisterType<IDocumentoService, DocumentoService>();
            container.RegisterType<ICertificadoService, CertificadoService>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}