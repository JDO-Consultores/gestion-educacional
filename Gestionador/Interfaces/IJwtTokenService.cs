using Gestionador.Responses;
using System.Collections.Generic;
using System.Security.Claims;

namespace Gestionador.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(UserResponse username, List<RolesUsuarios> roles);
        ClaimsPrincipal ValidateToken(string token);
        ClaimsIdentity ValidateUser(UserResponse user, List<RolesUsuarios> roles);
    }
}
