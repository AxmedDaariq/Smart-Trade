using System.Threading.Tasks;

namespace Identity.Application.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}
