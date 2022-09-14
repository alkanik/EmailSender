namespace EmailSender;

public interface IMailSender
{
    void SendEmail(Message message);
}