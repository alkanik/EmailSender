using EmailSender;
using EmailSender.Infrastructure;
using IncredibleBackend.Messaging.Extentions;
using IncredibleBackendContracts.Constants;
using IncredibleBackendContracts.Events;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog.Extensions.Logging;
using System.Configuration;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "EmailSender";
    })
    .ConfigureLogging((hostContext, logging) =>
    {
        logging.ClearProviders();
        logging.SetMinimumLevel(LogLevel.Trace);
        logging.AddNLog(hostContext.Configuration, new NLogProviderOptions() { LoggingConfigurationSectionName = "NLog" });
    })
    .ConfigureAppConfiguration((hostContext, configBuilder) =>
    {
        configBuilder
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .Build();
    })
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;
        services.Configure<EmailConfiguration>(configuration.GetSection("EmailConfiguration"));
        services.AddHostedService<Worker>();
        services.AddScoped<IMailSender, MailSender>();
        services.RegisterConsumersAndProducers((config) =>
        {
            config.AddConsumer<EmailConsumer>();
        }, (cfg, ctx) =>
        {
            cfg.ReceiveEndpoint(RabbitEndpoint.EmailCreate, c =>
            {
                c.ConfigureConsumer<EmailConsumer>(ctx);
            });
        }, null
        );
    })
    .Build();

await host.RunAsync();