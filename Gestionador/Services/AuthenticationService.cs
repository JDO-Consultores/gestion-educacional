using Gestionador.Interfaces;
using Gestionador.Model;
using Gestionador.Responses;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Gestionador.Services
{
    public class AuthenticationService : BaseServices, IAuthenticationService
    {
        public AuthenticationService(Entities dbContext, IMapperService mapperService) : base(dbContext, mapperService)
        {
        }

        public async Task<UserResponse> ValidateUserAsync(string username, string password)
        {
            UserResponse user = null;
            if (await IsValidUser(x => x.Username == username && x.IsActive == true))
            {
                var userdb = await GetUserByUsernameAsync(username);
                if (VerifyPasswordHash(password, userdb.PasswordHash, userdb.PasswordSalt))
                {
                    user = _mapperService.MapToUserResponse(userdb);
                }
            }
            return user;
        }

        public bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(storedHash);
            }
        }

        public (byte[] passwordHash, byte[] passwordSalt) CreatePasswordHash(string password)
        {
            using (var hmac = new HMACSHA512())
            {
                var passwordSalt = hmac.Key;
                var passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return (passwordHash, passwordSalt);
            }
        }

        private async Task<bool> IsValidUser(Expression<Func<tbl_Usuarios, bool>> expression = null)
        {
            return await _dbContext.tbl_Usuarios.Where(expression).AnyAsync();
        }

        private async Task<tbl_Usuarios> GetUserByUsernameAsync(string username)
        {
            var usuario = await _dbContext.tbl_Usuarios.Include(x => x.tbl_RolesUsuarios).FirstOrDefaultAsync(x => x.Username == username && x.IsActive == true);

            if (usuario != null)
            {
                foreach (var rolUsuario in usuario.tbl_RolesUsuarios)
                {
                    _dbContext.Entry(rolUsuario).Reference(ru => ru.tbl_Roles).Load();
                }
            }
            return usuario;
        }
    }
}