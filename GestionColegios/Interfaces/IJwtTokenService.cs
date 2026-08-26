using GestionColegios.Responses;
using System.Collections.Generic;
using System.Security.Claims;

namespace GestionColegios.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(UserResponse username, List<RolesUsuarios> roles);
        ClaimsPrincipal ValidateToken(string token);
        ClaimsIdentity ValidateUser(UserResponse user, List<RolesUsuarios> roles);
    }
}
