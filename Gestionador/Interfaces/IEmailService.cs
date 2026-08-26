using System.Threading.Tasks;

namespace Gestionador.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string subject, string content, string to);
    }
}
