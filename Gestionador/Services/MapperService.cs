using Gestionador.Helpers;
using Gestionador.Interfaces;
using Gestionador.Model;
using Gestionador.Models;
using Gestionador.Responses;
using System.Linq;
using System.Web.UI.WebControls;

namespace Gestionador.Services
{
    public class MapperService : IMapperService
    {
        public UserResponse MapToUserResponse(tbl_Usuarios entity)
        {
            if (entity == null)
            {
                return null;
            }

            var response = new UserResponse
            {
                ID = entity.ID,
                Nombre = entity.Nombre,
                Apellido = entity.Apellido,
                Creado = entity.Creado,
                IsActive = entity.IsActive,
                Username = entity.Username,
                Roles = entity.tbl_RolesUsuarios.Select(MapToRolesUsuarios).ToList(),
                IsAdmin = entity.tbl_RolesUsuarios.Any(s => s.RolID == 1),
                
            };
            return response;
        }

        public RolesResponse MapToRolResponse(tbl_Roles entity)
        {
            if (entity == null)
            {
                return null;
            }

            var response = new RolesResponse
            {
                ID = entity.ID,
                Rol = entity.Rol,
            };
            return response;
        }

        public RolesUsuarios MapToRolesUsuarios(tbl_RolesUsuarios entity)
        {
            if (entity == null)
            {
                return null;
            }

            var response = new RolesUsuarios
            {
                ID = entity.ID,
                RolID = entity.RolID,
                UsuarioID = entity.UsuarioID,
                Roles = MapToRolResponse(entity.tbl_Roles)
            };
            return response;
        }

        public RegionesResponse MapToRegionesResponse(tbl_Regiones entity)
        {
            if (entity == null)
            {
                return null;
            }

            var response = new RegionesResponse
            {
                ID = entity.ID,
                NroRegion = entity.NroRegion,
                Region = entity.Region,
                Simbologia = entity.Simbologia,
                IsActive = entity.IsActive
            };
            return response;
        }

        public ComunasResponse MapToComunasResponse(tbl_Comunas entity)
        {
            if (entity == null)
            {
                return null;
            }

            var response = new ComunasResponse
            {
                ID = entity.ID,
                Comuna = entity.Comuna,
                IsActive = entity.IsActive,
                RegionID = entity.RegionID,
                Region = MapToRegionesResponse(entity.tbl_Regiones)
            };
            return response;
        }

        public CementerioReponse MapToCementeriosResponse(tbl_Cementerios entity)
        {
            if (entity == null)
            {
                return null;
            }

            var response = new CementerioReponse
            {
                ID = entity.ID,
                Cementerio = entity.Cementerio,
                IsActive = entity.IsActive,
                Patios = entity.tbl_Patios?.Select(MapToPatiosResponse).ToList(),
            };
            return response;
        }

        public ConceptoResponse MapToConceptosResponse(tbl_Conceptos entity)
        {
            if (entity == null)
            {
                return null;
            }

            var response = new ConceptoResponse
            {
                ID = entity.ID,
                Concepto = entity.Concepto,
                CategoriaID = entity.CategoriaID,
                IsActive = entity.IsActive,
                IsNicho = entity.IsNicho,                       
                ServiciosConceptos = entity.tbl_ConceptosServicios?.Select(MapToServicioConceptoResponse).ToList()
            };
            return response;
        }

        public ServiciosConceptosResponse MapToServicioConceptoResponse(tbl_ConceptosServicios entity)
        {
            if (entity == null)
            {
                return null;
            }

            var response = new ServiciosConceptosResponse
            {
                ID = entity.ID,
                ConceptoID = entity.ConceptoID,
                ServicioID = entity.ServicioID,
                SeccionID = entity.SeccionID,
                IsActive = entity.IsActive,
                Precio = entity.Precio,             
                TipoMonedaID = entity.TipoMonedaID,
                TipoMoneda = MapToTipoMonedaResponse(entity.tbl_TipoMonedas),
                Servicios = MapToServicioResponse(entity.tbl_Servicios),
            };
            return response;
        }

        public MantencionConceptosResponse MapToMantencionConceptoResponse(tbl_ConceptosServicios entity)
        {
            if (entity == null)
            {
                return null;
            }

            var response = new MantencionConceptosResponse
            {
                ID = entity.ID,
                ConceptoID = entity.ConceptoID,
                MantencionID = entity.ServicioID,
                IsActive = entity.IsActive,
                Precio = entity.Precio,
                TipoMonedaID = entity.TipoMonedaID,
                TipoMoneda = MapToTipoMonedaResponse(entity.tbl_TipoMonedas),
                Mantenciones = MapToMantencionResponse(entity.tbl_Servicios),
            };
            return response;
        }
        public ServiciosConceptosResponse MapToServicioConceptoResponse(tbl_ServiciosAdquisicion entity)
        {
            if (entity == null)
            {
                return null;
            }

            var response = new ServiciosConceptosResponse
            {
                ID = entity.ID,
                ServicioID = entity.ServicioID,
                IsActive = entity.IsActive,
                Precio = entity.Precio,                
                Servicios = MapToServicioResponse(entity.tbl_Servicios),
            };
            return response;
        }
        public MantencionConceptosResponse MapToMantencionConceptoResponse(tbl_PagosMantenciones entity)
        {
            if (entity == null)
            {
                return null;
            }

            var response = new MantencionConceptosResponse
            {
                ID = entity.ID,
                MantencionID= entity.MantencionID,
                IsActive = entity.IsActive,
                Precio = entity.Precio,
                Mantenciones = MapToMantencionResponse(entity.tbl_Servicios),
            };
            return response;
        }

        public SeccionConceptosResponse MapToSeccionConceptoResponse(tbl_SeccionConceptos entity)
        {
            if (entity == null)
            {
                return null;
            }

            var response = new SeccionConceptosResponse
            {
                ConceptoID = entity.ConceptoID,
                ID = entity.ID,
                IsActive = entity.IsActive,
                SeccionID = entity.SeccionID,
                Stock = entity.Stock,
                Precio = entity.Precio,
                TipoMonedaID = entity.TipoMonedaID,
                Concepto = MapToConceptosResponse(entity.tbl_Conceptos),
                TipoMoneda = MapToTipoMonedaResponse(entity.tbl_TipoMonedas)
            };
            return response;
        }

        public FormasPagoResponse MapToFormasPagoResponse(tbl_FormasPago entity)
        {
            if (entity == null)
            {
                return null;
            }

            var resposne = new FormasPagoResponse
            {
                ID = entity.ID,
                FormaPago = entity.FormaPago,
                IsActive = entity.IsActive,
            };
            return resposne;
        }

        public CausaReponse MapToCausasResponse(tbl_Causas entity)
        {
            if (entity == null)
            {
                return null;
            }

            var response = new CausaReponse
            {
                ID = entity.ID,
                Causa = entity.Causa,
                IsActive = entity.IsActive,
            };
            return response;
        }

        public ValorMonedasResponse MapToValorMonedasResponse(tbl_ValorMonedas entity)
        {
            if (entity == null)
            {
                return null;
            }
            var response = new ValorMonedasResponse
            {
                ID = entity.ID,
                Valor = entity.Valor,
                Fecha = new System.DateTime(entity.Year, entity.Mes, entity.Dia),
                Month = entity.Mes,
                Day = entity.Dia,
                Year = entity.Year,
                TipoMonedaResponse = MapToTipoMonedaResponse(entity.tbl_TipoMonedas)
            };
            return response;
        }

        public TipoMonedaResponse MapToTipoMonedaResponse(tbl_TipoMonedas entity)
        {
            if (entity == null)
            {
                return null;
            }
            var response = new TipoMonedaResponse
            {
                ID = entity.ID,
                TipoMoneda = entity.TipoMoneda,
                IsActive = entity.IsActive,
                Periodicidad = entity.Periodicidad,
                Format = entity.Format,
                HasSymbol = entity.HasSymbol
            };
            return response;
        }

        public BancosResponse MapToBancoResponse(tbl_Bancos entity)
        {
            if (entity == null)
            {
                return null;
            }

            var response = new BancosResponse
            {
                ID = entity.ID,
                Banco = entity.Banco,
                IsActive = entity.IsActive,
            };
            return response;
        }

        public LugarDefuncionResponse MapToLugarDefuncionResponse(tbl_LugarDefuncion entity)
        {
            if (entity == null)
            {
                return null;
            }

            var response = new LugarDefuncionResponse
            {
                ID = entity.ID,
                Lugar = entity.Lugar,
                IsActive = entity.IsActive,
            };
            return response;
        }

        public PersonaResponse MapToCompradorResponse(dynamic entity, bool isComprador)
        {
            if (entity == null)
            {
                return null;
            }

            var response = new PersonaResponse
            {
                ID = entity.ID,
                Rut = isComprador ? entity.CompRut : entity.RefRut,
                Apellido = isComprador ? entity.CompApellido : entity.RefApellido,
                Nombre = isComprador ? entity.CompNombre : entity.RefNombre,
                ComunaID = isComprador ? entity.CompComunaID : entity.RefComunaID,
                Direccion1 = isComprador ? entity.CompDireccion : entity.RefDireccion,
                DirNum = isComprador ? entity.CompNum : entity.RefNum,
                Email = isComprador ? entity.CompEmail : entity.RefEmail,
                Telefono = isComprador ? entity.CompTelefono : entity.RefTelefono,
                IsActive = AutoMapperAdquisicion.HasProperty(entity, "IsActive") ? entity.IsActive : false,

                Comuna = MapToComunasResponse(isComprador ? entity.tbl_Comunas : entity.tbl_Comunas1),
            };
            return response;
        }

        public PersonaResponse MapToMaestroResponse(dynamic entity)
        {
            if (entity == null)
            {
                return null;
            }

            var response = new PersonaResponse
            {
                ID = entity.ID,
                Rut = entity.Rut,
                Apellido = entity.Apellido,
                Nombre = entity.Nombre,
                ComunaID = entity.ComunaID,
                Direccion1 = entity.Direccion,
                DirNum = entity.Num,
                Email = entity.Email,
                Telefono = entity.Telefono,
                IsActive = AutoMapperAdquisicion.HasProperty(entity, "IsActive") ? entity.IsActive : false,

                Comuna = MapToComunasResponse(entity.tbl_Comunas),
            };
            return response;
        }

        public PatiosResponse MapToPatiosResponse(tbl_Patios entity)
        {
            if (entity == null)
            {
                return null;
            }
            var response = new PatiosResponse
            {
                ID = entity.ID,
                Patio = entity.Patio,
                IsActive = entity.IsActive,
                CementerioID = (int)entity.CementerioID,
                Cementerio = entity.tbl_Cementerios?.Cementerio,
                Sectores = entity.tbl_Secciones?.Select(MapToSectoresResponse).ToList(),
            };
            return response;
        }

        public SectoresResponse MapToSectoresResponse(tbl_Secciones entity)
        {
            if (entity == null)
            {
                return null;
            }
            var response = new SectoresResponse
            {
                ID = entity.ID,
                Sector = entity.Seccion,
                IsActive = entity.IsActive,
                SeccionConcepto = entity.tbl_SeccionConceptos?.Select(MapToSeccionConceptoResponse).ToList(),                
            };
            return response;
        }

        public SectoresResponse MapToSectoresPatioResponse(tbl_Secciones entity)
        {
            if (entity == null)
            {
                return null;
            }
            var response = new SectoresResponse
            {
                ID = entity.ID,
                Sector = entity.Seccion,
                IsActive = entity.IsActive,
                Patios = MapToPatiosResponse(entity.tbl_Patios),
                SeccionConcepto = entity.tbl_SeccionConceptos?.Select(MapToSeccionConceptoResponse).ToList(),
            };
            return response;
        }

        public ServiciosResponse MapToServicioResponse(tbl_Servicios entity)
        {
            if (entity == null)
            {
                return null;
            }
            var response = new ServiciosResponse
            {
                ID = entity.ID,
                Servicio = entity.Servicio,
                CategoriaID = entity.CategoriaID,
                IsActive = entity.IsActive,
                Categoria = entity.tbl_Categorias.Categoria,
                Categorias = MapToCategoriaResponse(entity.tbl_Categorias)
            };
            return response;
        }

        public MantencionResponse MapToMantencionResponse(tbl_Servicios entity)
        {
            if (entity == null)
            {
                return null;
            }
            var response = new MantencionResponse
            {
                ID = entity.ID,
                Mantencion = entity.Servicio,
                CategoriaID = entity.CategoriaID,
                IsActive = entity.IsActive,
                Categoria = entity.tbl_Categorias.Categoria,
                Categorias = MapToCategoriaResponse(entity.tbl_Categorias)
            };
            return response;
        }

        public CategoriasResponse MapToCategoriaResponse(tbl_Categorias entity)
        {
            if (entity == null)
            {
                return null;
            }
            return new CategoriasResponse
            {
                ID= entity.ID,
                Categoria = entity.Categoria
            };
        }

        public AdquisicionServiciosResponse MapToAdquisicionServiciosResponse(tbl_ServiciosAdquisicion entity)
        {
            if (entity == null)
            {
                return null;
            }
            var response = new AdquisicionServiciosResponse
            {
                ID = entity.ID,
                ServicioID = entity.ServicioID,
                AdquisicionID = entity.AdquisicionID,
                FechaRegistro = entity.FechaRegistro.ToLocalTime(),
                IsActive = entity.IsActive,
                Precio = entity.Precio,
                Servicio = MapToServicioResponse(entity.tbl_Servicios),
                Maestro = MapToMaestroResponse(entity.tbl_Maestros)
            };
            return response;
        }

        public AdquisicionMantencionResponse MapToAdquisicionMantencionResponse(tbl_PagosMantenciones entity)
        {
            if (entity == null)
            {
                return null;
            }
            var response = new AdquisicionMantencionResponse
            {
                ID = entity.ID,
                MantencionID = entity.MantencionID,
                AdquisicionID = entity.AdquisicionID,
                FechaRegistro = entity.FechaRegistro.ToLocalTime(),
                IsActive = entity.IsActive,
                Precio = entity.Precio,
                Anio = entity.Anio,
                Observacion = entity.Observacion,
                Mantencion = MapToMantencionResponse(entity.tbl_Servicios)
            };
            return response;

        }
        public DifuntoViewModel MapToDifuntosResponse(tbl_Difuntos entity)
        {
            if (entity == null)
            {
                return null;
            }
            var response = new DifuntoViewModel
            {
                ID = entity.ID,
                CausaID = entity.CausaID,
                Edad = entity.Edad,
                FechaDefuncion = entity.FechaDefuncion,
                NombreApellido = entity.NombreApellido,
                Rut = entity.Rut,
                Causa = entity.tbl_Causas.Causa,
                FechaReuso = entity.FechaReuso,
                Reuso = entity.Reuso,
                Lugar = entity.tbl_LugarDefuncion.Lugar,
                LugarID = entity.LugarID,
                FechaRegistro = entity.FechaRegistro.ToLocalTime(),
                IsActive = entity.IsActive
            };
            return response;
        }

        public AdquisicionResponse MapToAdquisicionResponse(tbl_Adquisicion entity)
        {
            if (entity == null)
            {
                return null;
            }
            var response = new AdquisicionResponse
            {
                ID = entity.ID,
                NroFicha = entity.NroFicha,
                TranferredID = entity.TranferredID,
                NextTransferID = entity.NextTransferID,
                TranferredNroFicha = entity.TransferredNroFicha,
                NextTransferNroFicha = entity.NextTransferNroFicha,
                IsRegulated = entity.IsRegulated,
                IsTranferred = entity.IsTrasferred,
                CompNombre = entity.CompNombre,
                CompApellido = entity.CompApellido,
                CompRut = entity.CompRut,
                FechaAdquisicion = entity.FechaAdquisicion,
                Cementerio = entity.tbl_Cementerios.Cementerio,
                Comprador = MapToCompradorResponse(entity, true),
                Referente = MapToCompradorResponse(entity, false),
                UbiPatio = entity.tbl_Secciones.tbl_Patios.Patio.ToString(),
                UbiSector = entity.tbl_Secciones.Seccion,
                UbiNumero = entity.UbiNumero.ToString(),
                ProductoID = entity.ProductoID,
                Producto = entity.Producto,
                LetraNicho = entity.LetraNicho,
                PrecioProducto = entity.PrecioProducto,
                Estado = entity.tbl_Estados.Estado,
                TipoMonedaID = entity.TipoMonedaID,
                FechaTipoMoneda = entity.FechaTipoMoneda,
                PrecioTipoMoneda = entity.PrecioTipoMoneda,
                ValorTipoMonedaActual = entity.ValorTipoMonedaActual,
                TipoMoneda = MapToTipoMonedaResponse(entity.tbl_TipoMonedas),
                Sector = MapToSectoresResponse(entity.tbl_Secciones),
                SeccionConceptosResponse = MapToSeccionConceptoResponse(entity.tbl_Conceptos.tbl_SeccionConceptos.SingleOrDefault(w => w.ConceptoID == entity.ProductoID && w.SeccionID == entity.UbiSectorID && w.IsActive)),
                Difuntos = entity.tbl_Difuntos?.Select(MapToDifuntosResponse).OrderByDescending(o => o.FechaRegistro).ToList(),
                Servicios = entity.tbl_ServiciosAdquisicion?.Select(MapToAdquisicionServiciosResponse).OrderByDescending(o => o.FechaRegistro).ToList(),
                FormaPagos = entity.tbl_PagoAdquisicion?.Select(MapToAdquisicionPagoResponse).OrderByDescending(o => o.FechaRegistro).ToList(),
                Observaciones = entity.tbl_Observaciones?.Select(MapToAdquisicionObservacion).OrderByDescending(o => o.FechaRegistro).ToList(),
                Mantenciones = entity.tbl_PagosMantenciones?.Select(MapToAdquisicionMantencionResponse).OrderByDescending(o => o.Anio).ToList(),
                Historials = entity.tbl_HistorialAdquisicion?.Select(MapToHistorialResponse).OrderByDescending(o => o.FechaRegistro).ToList(),
            };
            return response;
        }

        public AdquisicionBase MapToAdquisicionBaseResponse(tbl_Adquisicion entity)
        {
            if (entity == null)
            {
                return null;
            }
            var response = new AdquisicionBase
            {
                ID = entity.ID,
                NroFicha = entity.NroFicha,
                CompNombre = entity.CompNombre,
                CompApellido = entity.CompApellido,
                CompRut = entity.CompRut,
                RefNombre = entity.RefNombre,
                RefApellido = entity.RefApellido,
                FechaAdquisicion = entity.FechaAdquisicion.ToLocalTime(),
                CementerioID = entity.CementerioID,
                Cementerio = entity.tbl_Cementerios.Cementerio,    
                Estado = entity.tbl_Estados.Estado,
                TipoMoneda = MapToTipoMonedaResponse(entity.tbl_TipoMonedas)
            };
            return response;
        }

        public AdquisicionPagoResponse MapToAdquisicionPagoResponse(tbl_PagoAdquisicion entity)
        {
            if (entity == null)
            {
                return null;
            }
            var response = new AdquisicionPagoResponse
            {
                ID = entity.ID,
                FechaRegistro = entity.FechaRegistro.ToLocalTime(),
                FormaPagoID = entity.FormaPagoID,
                BancoID = entity.BancoID,
                Monto = entity.Monto,
                NroRecaudacion = entity.NroRecaudacion,
                NroCheque = entity.NroCheque,
                FechaPago = entity.FechaPago.ToLocalTime(),
                BancoResponse = MapToBancoResponse(entity.tbl_Bancos),
                FormaPagoResponse = MapToFormasPagoResponse(entity.tbl_FormasPago),
            };
            return response;
        }

        public AdquisicionObservacion MapToAdquisicionObservacion(tbl_Observaciones entity)
        {
            if (entity == null)
            {
                return null;
            }
            var response = new AdquisicionObservacion
            {
                ID = entity.ID,
                UsuarioID = entity.UsuarioID,
                Observacion = entity.Observacion,
                FechaRegistro = entity.FechaRegistro.ToLocalTime(),
                User = MapToUserResponse(entity.tbl_Usuarios)
            };
            return response;
        }

        public HistorialResponse MapToHistorialResponse(tbl_HistorialAdquisicion entity)
        {
            if (entity == null)
            {
                return null;
            }
            var response = new HistorialResponse
            {
                ID = entity.ID,
                UsuarioID = entity.UsuarioID,
                AdquisicionID = entity.AdquisicionID,
                Accion = entity.Accion,
                FechaRegistro = entity.FechaRegistro.ToLocalTime(),
                User = MapToUserResponse(entity.tbl_Usuarios)
            };
            return response;
        }

        public ParrocosResponse MapToParrocosResponse(tbl_Parrocos entity)
        {
            if (entity == null)
            {
                return null;
            }
            return new ParrocosResponse
            {
                ID = entity.ID,
                IsActive = entity.IsActive,
                NombreParroco = entity.NombreParroco,
                RutParroco = entity.RutParroco,
                TipoAdministradorID = entity.TipoAdministradorID,
                TipoAdministrador = MapToTipoAdministradorResponse(entity.tbl_TipoAdministrador)
            };
        }

        public TipoAdministradorResponse MapToTipoAdministradorResponse(tbl_TipoAdministrador entity)
        {
            if (entity == null)
            {
                return null;
            }
            return new TipoAdministradorResponse
            {
                ID = entity.ID,
                TipoAdministrador = entity.TipoAdministrador
            };
        }
    }
}