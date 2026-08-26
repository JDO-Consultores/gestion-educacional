using Gestionador.Helpers.Enum;
using Gestionador.Interfaces;
using Gestionador.Model;
using Gestionador.Models;
using Gestionador.Responses;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Gestionador.Services
{
    public class ConfigService : BaseServices, IConfigService
    {
        public ConfigService(Entities dbContext, IMapperService mapperService) : base(dbContext, mapperService)
        {
        }

        public async Task<CausaReponse> GetCausaById(int id)
        {
            return _mapperService.MapToCausasResponse(await _dbContext.tbl_Causas.FindAsync(id));
        }

        public async Task<PersonaResponse> GetMaestroById(int id)
        {
            return _mapperService.MapToMaestroResponse(await _dbContext.tbl_Maestros.FindAsync(id));
        }

        public async Task<ValorMonedasResponse> GetValorMonedaById(int id)
        {
            return _mapperService.MapToValorMonedasResponse(await _dbContext.tbl_ValorMonedas.FindAsync(id));
        }

        public async Task<LugarDefuncionResponse> GetLugerDefuncionById(int id)
        {
            return _mapperService.MapToLugarDefuncionResponse(await _dbContext.tbl_LugarDefuncion.FindAsync(id));
        }

        public async Task<ParrocosResponse> GetParrocoById(int id)
        {
            return _mapperService.MapToParrocosResponse(await _dbContext.tbl_Parrocos.FindAsync(id));
        }

        public async Task<int> UpsertCausaAsync(CausaRequest request)
        {
            tbl_Causas causa;
            if (request.ID.HasValue)
            {
                causa = await _dbContext.tbl_Causas.FindAsync(request.ID);
            }
            else
            {
                causa = new tbl_Causas();
                _dbContext.tbl_Causas.Add(causa);
            }
            causa.Causa = request.Causa.ToUpper();
            causa.IsActive = request.IsActive;

            await _dbContext.SaveChangesAsync();
            return causa.ID;
        }

        public async Task<int> UpsertMaestroAsync(PersonaViewModel request)
        {
            tbl_Maestros maestro;
            if (request.ID.HasValue)
            {
                maestro = await _dbContext.tbl_Maestros.FindAsync(request.ID);
            }
            else
            {
                if (await _dbContext.tbl_Maestros.AnyAsync(w => w.Rut.Equals(request.Rut)))
                {
                    return -1;
                }
                maestro = new tbl_Maestros();
                _dbContext.tbl_Maestros.Add(maestro);
            }
            maestro.Direccion = request.Direccion1.ToUpper();
            maestro.Nombre = request.Nombre.ToUpper();
            maestro.Apellido = request.Apellido.ToUpper();
            maestro.Rut = request.Rut.ToUpper();
            maestro.Num = request.DirNum.ToUpper();
            maestro.Telefono = request.Telefono.ToUpper();
            maestro.Email = string.IsNullOrEmpty(request.Email) ? null : request.Email.ToUpper();
            maestro.IsActive = request.IsActive;
            maestro.ComunaID = request.ComunaID;

            await _dbContext.SaveChangesAsync();
            return maestro.ID;
        }

        public async Task<int> UpsertTipoMonedaAsync(ValorMonedaRequest request)
        {
            var tipoMoneda = await _dbContext.tbl_TipoMonedas.FirstOrDefaultAsync(t => t.ID == request.TipoMonedaID);

            if (tipoMoneda == null)
                return -1;

            bool existe = false;

            if (tipoMoneda.Periodicidad == PeriodicityType.DIARIA.ToString())
            {
                existe = await _dbContext.tbl_ValorMonedas.AnyAsync(u =>
                    u.Dia == request.Fecha.Day &&
                    u.Mes == request.Fecha.Month &&
                    u.Year == request.Fecha.Year &&
                    u.TipoMonedaID == request.TipoMonedaID &&
                    (!request.ID.HasValue || u.ID != request.ID.Value));
            }
            else if (tipoMoneda.Periodicidad == PeriodicityType.MENSUAL.ToString())
            {
                existe = await _dbContext.tbl_ValorMonedas.AnyAsync(u =>
                    u.Mes == request.Fecha.Month &&
                    u.Year == request.Fecha.Year &&
                    u.TipoMonedaID == request.TipoMonedaID &&
                    (!request.ID.HasValue || u.ID != request.ID.Value));
            }

            if (existe)
                return -1;

            tbl_ValorMonedas valorMoneda;
            if (request.ID.HasValue)
            {
                valorMoneda = await _dbContext.tbl_ValorMonedas.FindAsync(request.ID);
            }
            else
            {
                valorMoneda = new tbl_ValorMonedas { };
                _dbContext.tbl_ValorMonedas.Add(valorMoneda);
            }
            valorMoneda.Mes = request.Fecha.Month;
            valorMoneda.Year = request.Fecha.Year;
            valorMoneda.Dia = request.Fecha.Day;
            valorMoneda.FechaRegistro = DateTime.Now;
            valorMoneda.TipoMonedaID = request.TipoMonedaID;
            valorMoneda.Valor = request.Valor;

            await _dbContext.SaveChangesAsync();
            return valorMoneda.ID;
        }

        public async Task<int> UpsertLugarDefuncionAsync(LugarDefuncionRequest request)
        {
            tbl_LugarDefuncion lugar;
            if (request.ID.HasValue)
            {
                lugar = await _dbContext.tbl_LugarDefuncion.FindAsync(request.ID);
            }
            else
            {
                lugar = new tbl_LugarDefuncion();
                _dbContext.tbl_LugarDefuncion.Add(lugar);
            }
            lugar.Lugar = request.Lugar.ToUpper();
            lugar.IsActive = request.IsActive;

            await _dbContext.SaveChangesAsync();
            return lugar.ID;
        }

        public async Task<int> UpsertParrocoAsync(ParrocoRequest request)
        {
            tbl_Parrocos parroco;

            if (request.ID.HasValue)
            {
                parroco = await _dbContext.tbl_Parrocos.FindAsync(request.ID);
            }
            else
            {
                parroco = new tbl_Parrocos { };
                _dbContext.tbl_Parrocos.Add(parroco);
            }
            parroco.IsActive = request.IsActive;
            parroco.TipoAdministradorID = request.TipoAdministradorID;
            parroco.NombreParroco = request.Nombre.ToUpper();
            parroco.RutParroco = request.Rut.ToUpper();

            await _dbContext.SaveChangesAsync();
            return parroco.ID;
        }
    }
}