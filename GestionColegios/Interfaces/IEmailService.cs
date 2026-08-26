using System.Threading.Tasks;

namespace GestionColegios.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string subject, string content, string to);
    }
}
