using Identity.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        // In a real implementation, this would use SendGrid, MailKit, or AWS SES.
        // For now, we log the email content to the console/logs for verification.
        _logger.LogInformation("Sending Email to: {To}\nSubject: {Subject}\nBody: {Body}", to, subject, body);
        await Task.CompletedTask;
    }
}
