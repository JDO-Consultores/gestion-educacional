using Gestionador.Interfaces;
using Gestionador.Model;
using Gestionador.Responses;
using Microsoft.AspNet.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Gestionador.Services
{
    public class JwtTokenService : BaseServices, IJwtTokenService
    {
        public JwtTokenService(Entities dbContext, IMapperService mapperService) : base(dbContext, mapperService)
        {
        }

        public string GenerateToken(UserResponse user, List<RolesUsuarios> roles)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JWTSecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, user.ID.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(ClaimTypes.Name, user.Nombre),
                new Claim(ClaimTypes.Email, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Roles.Rol));
            }

            var token = new JwtSecurityToken(
                issuer: _JWTIssuer,
                audience: _JWTAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsIdentity ValidateUser(UserResponse user, List<RolesUsuarios> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, user.ID.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(ClaimTypes.Name, user.Nombre),
                new Claim(ClaimTypes.Email, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Roles.Rol));
            }
            return new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JWTSecretKey));

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _JWTIssuer,
                ValidAudience = _JWTAudience,
                IssuerSigningKey = key,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch (SecurityTokenException)
            {
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}