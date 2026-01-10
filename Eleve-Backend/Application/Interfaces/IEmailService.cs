namespace Eleve_Backend.Application.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to,string subject, string body);
    }
}
