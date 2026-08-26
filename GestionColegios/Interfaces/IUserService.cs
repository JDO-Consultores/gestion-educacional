using GestionColegios.Models;
using GestionColegios.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestionColegios.Interfaces
{
    public interface IUserService
    {
        Task<UserResponse> GetUserByUsername(string username);
        Task<UserResponse> GetUserByID(int id);
        Task<List<UserResponse>> GetUsers();
        List<RolesResponse> GetRoles();
        Task<bool> CambiarEstadoUsuarioAsync(int id);
        Task<int> CreateUserAsync(int userId, UserCreateViewModel user);
        Task<int> UpsertUserAsync(int userId, UserViewModel user);
        Task<bool> ChangePasswordAsync(int userId, PasswordRequest request);
        Task<bool> ForgotPasswordAsync(ForgotPasswordViewModel model, string baseUrl);
        Task<bool> ResetPasswordAsync(ResetPasswordViewModel model);
    }
}