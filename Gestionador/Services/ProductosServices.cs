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
    public class ProductosServices : BaseServices, IProductosServices
    {
        public ProductosServices(Entities dbContext, IMapperService mapperService) : base(dbContext, mapperService)
        {
        }

        public async Task<List<ConceptoResponse>> GetProductosAsync()
        {
            var result = await _dbContext.tbl_Conceptos.Include(x => x.tbl_Categorias).OrderBy(x => x.Concepto).ToListAsync();
            return result.Select(_mapperService.MapToConceptosResponse).ToList();
        }

        public async Task<ConceptoResponse> GetProductoById(int id)
        {
            return _mapperService.MapToConceptosResponse(await _dbContext.tbl_Conceptos.FindAsync(id));
        }

        public async Task<int> UpsertProductoAsync(ConceptoRequest request)
        {
            tbl_Conceptos concepto;

            if (request.ID.HasValue)
            {
                concepto = await _dbContext.tbl_Conceptos.FindAsync(request.ID);
            }
            else
            {
                concepto = new tbl_Conceptos
                {
                };
                _dbContext.tbl_Conceptos.Add(concepto);
            }
            concepto.IsActive = request.IsActive;
            concepto.CategoriaID = 1;
            concepto.Concepto = request.Concepto.ToUpper();
            concepto.IsNicho = request.IsNicho;

            await _dbContext.SaveChangesAsync();
            return concepto.ID;
        }
    }
}