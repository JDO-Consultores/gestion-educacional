using Gestionador.Responses;
using System.Threading.Tasks;

namespace Gestionador.Interfaces
{
    public interface IAuthenticationService
    {
        Task<UserResponse> ValidateUserAsync(string username, string password);
        (byte[] passwordHash, byte[] passwordSalt) CreatePasswordHash(string password);
        bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt);
    }
}