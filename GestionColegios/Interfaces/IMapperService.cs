using GestionColegios.Model;
using GestionColegios.Models;
using GestionColegios.Responses;

namespace GestionColegios.Interfaces
{
    public interface IMapperService
    {
        UserResponse MapToUserResponse(tbl_Usuarios entity);
        RolesResponse MapToRolResponse(tbl_Roles entity);
        RolesUsuarios MapToRolesUsuarios(tbl_RolesUsuarios entity); 
        RegionesResponse MapToRegionesResponse(tbl_Region entity);
        ComunasResponse MapToComunasResponse(tbl_Comuna entity);
    }
}