namespace EmailSender;

public interface IMailSender
{
    Task SendEmail(Message message);
}