namespace GHotel.Application.Utilities;

public interface IEmailSenderUtility
{
    Task SendEmailConfirmationAsync(string subject, string email, string link, CancellationToken cancellationToken);
    Task SendEmailAsync(string toEmail, string subject, string htmlMessage, CancellationToken cancellationToken);
}
