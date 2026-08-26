using GestionColegios.Responses;
using System.Threading.Tasks;

namespace GestionColegios.Interfaces
{
    public interface IAuthenticationService
    {
        Task<UserResponse> ValidateUserAsync(string username, string password);
        (byte[] passwordHash, byte[] passwordSalt) CreatePasswordHash(string password);
        bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt);
    }
}
