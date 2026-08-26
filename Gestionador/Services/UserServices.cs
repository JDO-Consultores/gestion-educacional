using Gestionador.Helpers;
using Gestionador.Interfaces;
using Gestionador.Model;
using Gestionador.Models;
using Gestionador.Responses;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Gestionador.Services
{
    public class UserServices : BaseServices, IUserService
    {
        private IAuthenticationService _authenticationService;
        private readonly IEmailService _emailService;

        public UserServices(Entities dbContext, IMapperService mapperService, IAuthenticationService authenticationService, IEmailService emailService) : base(dbContext, mapperService)
        {
            _authenticationService = authenticationService;
            _emailService = emailService;
        }

        public async Task<UserResponse> GetUserByUsername(string username)
        {
            return _mapperService.MapToUserResponse(await GetUserByUsernameAsync(username));
        }

        public async Task<UserResponse> GetUserByID(int id)
        {
            return _mapperService.MapToUserResponse(await GetUserByQuery(x => x.ID == id).SingleOrDefaultAsync());
        }

        public async Task<List<UserResponse>> GetUsers()
        {
            var result = await _dbContext.tbl_Usuarios.Where(w => w.IsSuperAdmin == false).ToListAsync();
            return result.Select(_mapperService.MapToUserResponse).ToList();
        }

        public List<RolesResponse> GetRoles()
        {
            return _dbContext.tbl_Roles.Select(_mapperService.MapToRolResponse).ToList();
        }

        public async Task<int> CreateUserAsync(int userId, UserCreateViewModel model)
        {
            var query = await GetUserByUsernameAsync(model.Username);
            if (query != null)
            {
                return -2;
            }
            if (!model.Password.Equals(model.PasswordConfirm))
            {
                return -1;
            }
            tbl_Usuarios user = new tbl_Usuarios
            {
                IsActive = model.IsActive,
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                Username = model.Username,
                Creado = DateTime.UtcNow,
            };

            if (!string.IsNullOrEmpty(model.Password))
            {
                var passwordHash = _authenticationService.CreatePasswordHash(model.Password);
                user.PasswordHash = passwordHash.passwordHash;
                user.PasswordSalt = passwordHash.passwordSalt;
            }

            _dbContext.tbl_Usuarios.Add(user);

            user.tbl_RolesUsuarios.Add(new tbl_RolesUsuarios { RolID = 2, UsuarioID = user.ID });

            if (model.IsAdmin)
            {
                if (!user.tbl_RolesUsuarios.Any(r => r.RolID == 1))
                {
                    user.tbl_RolesUsuarios.Add(new tbl_RolesUsuarios { RolID = 1, UsuarioID = user.ID });
                }
            }
            await _dbContext.SaveChangesAsync();
            return user.ID;
        }

        public async Task<int> UpsertUserAsync(int userId, UserCreateViewModel model)
        {
            tbl_Usuarios user;
            if (model.ID.HasValue)
            {
                user = await GetUserByQuery(x => x.ID == model.ID).SingleOrDefaultAsync();
                user.Nombre = model.Nombre;
                user.Apellido = model.Apellido;
                user.IsActive = model.IsActive;
                _dbContext.Entry(user).State = EntityState.Modified;
            }
            else
            {
                user = new tbl_Usuarios
                {
                    IsActive = model.IsActive,
                    Nombre = model.Nombre,
                    Apellido = model.Apellido,
                    Username = model.Username,
                    Creado = DateTime.UtcNow,
                };
                _dbContext.tbl_Usuarios.Add(user);
            }

            var passwordHash = _authenticationService.CreatePasswordHash(model.Password);
            user.PasswordHash = passwordHash.passwordHash;
            user.PasswordSalt = passwordHash.passwordSalt;

            if (!user.tbl_RolesUsuarios.Any(r => r.tbl_Roles.Rol == "Usuario"))
            {
                user.tbl_RolesUsuarios.Add(new tbl_RolesUsuarios { RolID = 2, UsuarioID = user.ID });
            }

            if (model.IsAdmin)
            {
                if (!user.tbl_RolesUsuarios.Any(r => r.tbl_Roles.Rol == "Administrador"))
                {
                    user.tbl_RolesUsuarios.Add(new tbl_RolesUsuarios { RolID = 1, UsuarioID = user.ID });
                }
            }
            else
            {
                var adminRole = user.tbl_RolesUsuarios.SingleOrDefault(r => r.RolID == 1 && r.UsuarioID == user.ID);
                if (adminRole != null)
                {
                    _dbContext.Entry(adminRole).State = EntityState.Deleted;
                }
            }

            await _dbContext.SaveChangesAsync();
            return user.ID;
        }

        public async Task<bool> ChangePasswordAsync(int userId, PasswordRequest request)
        {
            var user = await GetUserByQuery(x => x.ID == userId).SingleOrDefaultAsync();
            var passwordHash = _authenticationService.CreatePasswordHash(request.Password);
            user.PasswordHash = passwordHash.passwordHash;
            user.PasswordSalt = passwordHash.passwordSalt;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordViewModel model, string baseUrl)
        {
            var user = await GetUserByUsernameAsync(model.Email);

            if (user != null)
            {
                var rawToken = Guid.NewGuid().ToString();

                user.TokenHash = HashToken.HashTokenPass(rawToken);
                user.ExpirationDate = DateTime.Now.AddHours(1);
                user.IsUsed = false;
                await _dbContext.SaveChangesAsync();

                UrlHelper url = new UrlHelper();

                var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["templatePassword"]);
                var emailBody = File.ReadAllText(templatePath);
                
                emailBody = emailBody.Replace("{{$firstName}}", $"{user.Nombre} {user.Apellido}")
                             .Replace("{{$url}}", $"{baseUrl}/Account/ResetPassword?token={rawToken}");

                await _emailService.SendEmailAsync("Recuperar contraseña", emailBody, model.Email);

                return true;
            }
            return false;
        }


        public async Task<bool> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            var tokenHash = HashToken.HashTokenPass(model.Token);
            var tokenRecord = await _dbContext.tbl_Usuarios.FirstOrDefaultAsync(w => w.TokenHash == tokenHash && (bool)!w.IsUsed && w.ExpirationDate > DateTime.Now);

            if (tokenRecord != null)
            {
                var passwordHash = _authenticationService.CreatePasswordHash(model.Password);
                tokenRecord.PasswordHash = passwordHash.passwordHash;
                tokenRecord.PasswordSalt = passwordHash.passwordSalt;

                tokenRecord.IsUsed = null;
                tokenRecord.TokenHash = null;
                tokenRecord.ExpirationDate = null;

                _dbContext.Entry(tokenRecord).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> ConfirmarEliminar(int id)
        {
            var user = await GetUserByID(id);
            user.IsActive = false;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private async Task<tbl_Usuarios> GetUserByUsernameAsync(string username)
        {
            var usuario = await _dbContext.tbl_Usuarios.Include(x => x.tbl_RolesUsuarios).FirstOrDefaultAsync(x => x.Username == username);

            if (usuario != null)
            {
                foreach (var rolUsuario in usuario.tbl_RolesUsuarios)
                {
                    _dbContext.Entry(rolUsuario).Reference(ru => ru.tbl_Roles).Load();
                }
            }
            return usuario;
        }

        private IQueryable<tbl_Usuarios> GetUserByQuery(Expression<Func<tbl_Usuarios, bool>> expression = null)
        {
            var usuario = _dbContext.tbl_Usuarios.Include(x => x.tbl_RolesUsuarios);

            if (expression != null)
            {
                usuario = usuario.Where(expression);
            }
            return usuario;
        }
    }
}