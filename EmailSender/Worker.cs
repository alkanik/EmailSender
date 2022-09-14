namespace EmailSender;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("EmailSender running at: {time}", DateTimeOffset.Now);
            using (var scope = _serviceProvider.CreateScope())
            {
                IMailSender mailsender =
                    scope.ServiceProvider.GetRequiredService<IMailSender>();
                var message = new Message(new string[] { "IncredibleEmailSender@gmail.com" }, "Hi", "All you need is mail!");
                mailsender.SendEmail(message);
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}