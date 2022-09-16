using EmailSender.Infrastructure;
using System.Configuration;

namespace EmailSender;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("EmailSender running at: {time}", DateTimeOffset.Now);

        var emailConfig = _configuration
                .GetSection("EmailConfiguration")
                .Get<EmailConfiguration>();
    }
}