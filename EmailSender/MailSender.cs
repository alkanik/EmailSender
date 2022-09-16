using EmailSender.Infrastructure;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MimeKit;

namespace EmailSender;

public class MailSender : IMailSender
{
    private readonly EmailConfiguration _emailConfig;
    private readonly ILogger<MailSender> _logger;

    public MailSender(IOptions<EmailConfiguration> emailConfig, ILogger<MailSender> logger)
    {
        _emailConfig = emailConfig.Value;
        _logger = logger;
    }
    public async Task SendEmail(Message message)
    {
        var emailMessage = CreateEmailMessage(message);
        await Send(emailMessage);
    }

    private MimeMessage CreateEmailMessage(Message message)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("email", _emailConfig.From));
        emailMessage.To.AddRange(message.To);
        emailMessage.Subject = message.Subject;
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };

        return emailMessage;
    }

    private async Task Send(MimeMessage mailMessage)
    {
        using (var client = new SmtpClient())
        {
            try
            {
                client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfig.UserName, _emailConfig.Password);

                await client.SendAsync(mailMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }
    }
}
