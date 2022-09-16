using IncredibleBackendContracts.Events;
using MailKit;
using MassTransit;
using NETCore.MailKit.Core;

namespace EmailSender;

public class EmailConsumer : IConsumer<EmailEvent>
{
    private readonly ILogger<EmailConsumer> _logger;
    private readonly IMailSender _mailSender;

    public EmailConsumer(ILogger<EmailConsumer> logger, IMailSender mailSender)
    {
        _logger = logger;
        _mailSender = mailSender;
    }

    public async Task Consume(ConsumeContext<EmailEvent> context)
    {
        _logger.LogInformation("Message recieved");

        var message = new Message(new string[] { $"{context.Message.Email}" }, $"{context.Message.Subject}", $"{context.Message.Body}");

        _logger.LogInformation("Message sent");
        await _mailSender.SendEmail(message);
    }
}