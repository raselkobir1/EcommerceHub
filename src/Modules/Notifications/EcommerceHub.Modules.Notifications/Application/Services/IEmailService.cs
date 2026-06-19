namespace EcommerceHub.Modules.Notifications.Application.Services;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default);
    Task SendTemplatedAsync(string to, string templateId, object templateData, CancellationToken ct = default);
}
