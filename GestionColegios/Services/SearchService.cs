using GestionColegios.Interfaces;
using GestionColegios.Model;
using GestionColegios.Responses;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GestionColegios.Services
{
    [Authorize]
    public class SearchService : BaseServices, ISearchService
    {
        public SearchService(Entities dbContext, IMapperService mapperService) : base(dbContext, mapperService)
        {
        }

        public List<dynamic> GetRegiones()
        {
            var regiones = _dbContext.tbl_Region.OrderBy(w => w.ID).ToList();
            var regionesResponse = regiones.Select(s => _mapperService.MapToRegionesResponse(s)).ToList<dynamic>();
            return regionesResponse;
        }

        public async Task<List<ComunasResponse>> GetComunasByRegionIdAsync(int id)
        {
            var comunas = await _dbContext.tbl_Comuna.Where(w => w.RegionID == id && w.IsActive == true).ToListAsync();
            var comunasResponse = comunas.Select(s => _mapperService.MapToComunasResponse(s)).OrderBy(o => o.Comuna).ToList();
            return comunasResponse;
        }        
    }
}