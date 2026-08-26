using Gestionador.Interfaces;
using Gestionador.Model;
using Gestionador.Models;
using Gestionador.Responses;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Gestionador.Services
{
    public class ServiciosService : BaseServices, IServiciosService
    {
        public ServiciosService(Entities dbContext, IMapperService mapperService) : base(dbContext, mapperService)
        {
        }

        public async Task<List<ServiciosResponse>> GetServicios()
        {
            var result = await _dbContext.tbl_Servicios.Include(x => x.tbl_Categorias).OrderBy(x => x.Servicio).ToListAsync();
            return result.Select(_mapperService.MapToServicioResponse).ToList();
        }

        public async Task<ServiciosResponse> GetServicioById(int id)
        {
            return _mapperService.MapToServicioResponse(await _dbContext.tbl_Servicios.FindAsync(id));
        }

        public async Task<int> UpsertServicioAsync(ServicioRequest request)
        {
            tbl_Servicios servicios;

            if (request.ID.HasValue)
            {
                servicios = await _dbContext.tbl_Servicios.FindAsync(request.ID);
            }
            else
            {
                servicios = new tbl_Servicios{};
                _dbContext.tbl_Servicios.Add(servicios);
            }
            servicios.IsActive = request.IsActive;
            servicios.CategoriaID = request.CategoriaID;
            servicios.Servicio = request.Servicio.ToUpper();

            await _dbContext.SaveChangesAsync();
            return servicios.ID;
        }
    }
}
