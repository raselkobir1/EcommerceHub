using EcommerceHub.Modules.Notifications.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace EcommerceHub.Modules.Notifications.Infrastructure.Services;

internal sealed class SendGridEmailService(
    IConfiguration configuration,
    ILogger<SendGridEmailService> logger)
    : IEmailService
{
    private readonly string _apiKey = configuration["SendGrid:ApiKey"]
        ?? throw new InvalidOperationException("SendGrid API key not configured.");
    private readonly string _senderEmail = configuration["SendGrid:SenderEmail"] ?? "noreply@ecommercehub.com";
    private readonly string _senderName = configuration["SendGrid:SenderName"] ?? "EcommerceHub";

    public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
    {
        var client = new SendGridClient(_apiKey);
        var from = new EmailAddress(_senderEmail, _senderName);
        var toAddress = new EmailAddress(to);
        var msg = MailHelper.CreateSingleEmail(from, toAddress, subject, null, htmlBody);

        var response = await client.SendEmailAsync(msg, ct);
        if (!response.IsSuccessStatusCode)
            logger.LogError("SendGrid failed for {To}: {Status}", to, response.StatusCode);
    }

    public async Task SendTemplatedAsync(string to, string templateId, object templateData, CancellationToken ct = default)
    {
        var client = new SendGridClient(_apiKey);
        var msg = new SendGridMessage
        {
            From = new EmailAddress(_senderEmail, _senderName),
            TemplateId = templateId
        };
        msg.AddTo(new EmailAddress(to));
        msg.SetTemplateData(templateData);

        var response = await client.SendEmailAsync(msg, ct);
        if (!response.IsSuccessStatusCode)
            logger.LogError("SendGrid template failed for {To}: {Status}", to, response.StatusCode);
    }
}
