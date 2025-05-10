namespace SlqStudio.Application.Services.EmailService;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string recipientEmail, string subject, string body);
}