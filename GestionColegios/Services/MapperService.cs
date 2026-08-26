using GestionColegios.Helpers;
using GestionColegios.Interfaces;
using GestionColegios.Model;
using GestionColegios.Models;
using GestionColegios.Responses;
using System.Linq;
using System.Web.UI.WebControls;

namespace GestionColegios.Services
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

        public RegionesResponse MapToRegionesResponse(tbl_Region entity)
        {
            if (entity == null)
            {
                return null;
            }

            var response = new RegionesResponse
            {
                ID = entity.ID,
                Region = entity.Nombre,
                IsActive = entity.IsActive
            };
            return response;
        }

        public ComunasResponse MapToComunasResponse(tbl_Comuna entity)
        {
            if (entity == null)
            {
                return null;
            }

            var response = new ComunasResponse
            {
                ID = entity.ID,
                Comuna = entity.Nombre,
                IsActive = entity.IsActive,
                RegionID = entity.RegionID,
                Region = MapToRegionesResponse(entity.tbl_Region)
            };
            return response;
        }        
    }
}