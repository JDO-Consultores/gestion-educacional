using Gestionador.Interfaces;
using Gestionador.Model;
using System.Configuration;

namespace Gestionador.Services
{
    public class BaseServices
    {
        private protected readonly Entities _dbContext;
        private protected readonly IMapperService _mapperService;

        protected BaseServices(Entities dbContext, IMapperService mapperService)
        {
            _dbContext = dbContext;
            _mapperService = mapperService;
        }

        public string _JWTIssuer
        {
            get
            {
                return ConfigurationManager.AppSettings["JWT:Issuer"];
            }
        }

        public string _JWTSecretKey
        {
            get
            {
                return ConfigurationManager.AppSettings["JWT:SecretKey"];
            }
        }

        public string _JWTAudience
        {
            get
            {
                return ConfigurationManager.AppSettings["JWT:Audience"];
            }
        }
    }
}