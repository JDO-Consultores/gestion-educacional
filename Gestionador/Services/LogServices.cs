using Gestionador.Interfaces;
using Gestionador.Model;
using System.Threading.Tasks;

namespace Gestionador.Services
{
    public class LogServices : BaseServices, ILogService
    {
        public LogServices(Entities dbContext, IMapperService mapperService) : base(dbContext, mapperService)
        {
        }

        public async Task UpsertLog(int userId, string log)
        {

        }
    }
}