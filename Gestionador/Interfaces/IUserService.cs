using Gestionador.Models;
using Gestionador.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace Gestionador.Interfaces
{
    public interface IUserService
    {
        Task<UserResponse> GetUserByUsername(string username);
        Task<UserResponse> GetUserByID(int id);
        Task<List<UserResponse>> GetUsers();
        List<RolesResponse> GetRoles();
        Task<bool> ConfirmarEliminar(int id);
        Task<int> CreateUserAsync(int userId, UserCreateViewModel user);
        Task<int> UpsertUserAsync(int userId, UserCreateViewModel user);
        Task<bool> ChangePasswordAsync(int userId, PasswordRequest request);
        Task<bool> ForgotPasswordAsync(ForgotPasswordViewModel model, string baseUrl);
        Task<bool> ResetPasswordAsync(ResetPasswordViewModel model);
    }
}