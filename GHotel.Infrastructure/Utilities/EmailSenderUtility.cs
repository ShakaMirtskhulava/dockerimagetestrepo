using System.Globalization;
using System.Net;
using System.Net.Mail;
using GHotel.Application.Utilities;
using GHotel.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace GHotel.Infrastructure.Utilities;

public class EmailSenderUtility : IEmailSenderUtility
{
    private readonly EmailConfiguration _emailConfiguration;

    public EmailSenderUtility(IOptions<EmailConfiguration> emaiConfiguration)
    {
        _emailConfiguration = emaiConfiguration.Value;
    }

    public async Task SendEmailConfirmationAsync(string subject, string email, string link, CancellationToken cancellationToken)
    {
        var message = $@"<p>Please confirm your email by clicking this link: <a href='{link}'>link</a></p>";
        await SendEmailAsync(email, subject, message, cancellationToken).ConfigureAwait(false);
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage, CancellationToken cancellationToken)
    {
        var mailMessage = new MailMessage();
        mailMessage.From = new MailAddress(_emailConfiguration.From);
        mailMessage.To.Add(new MailAddress(toEmail));
        mailMessage.Subject = subject;
        mailMessage.Body = htmlMessage;
        mailMessage.IsBodyHtml = true;

        var smtpClient = new SmtpClient(_emailConfiguration.Host)
        {
            Port = int.Parse(_emailConfiguration.Port,CultureInfo.CurrentCulture),
            Credentials = new NetworkCredential(_emailConfiguration.From, _emailConfiguration.Password),
            EnableSsl = true
        };
        await smtpClient.SendMailAsync(mailMessage, cancellationToken).ConfigureAwait(false);

    }

}
