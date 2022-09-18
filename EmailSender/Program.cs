using EmailSender;
using EmailSender.Infrastructure;
using IncredibleBackend.Messaging.Extentions;
using IncredibleBackendContracts.Constants;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using NLog;
using NLog.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
LogManager.Configuration.Variables[$"{builder.Environment: LOG_DIRECTORY}"] = "Logs";

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "EmailSender";
    })
    .ConfigureLogging((hostContext, logging) =>
    {
        logging.ClearProviders();
        logging.AddNLog();
    })
    //.ConfigureAppConfiguration((hostContext, configBuilder) =>
    //{
    //    configBuilder
    //        .SetBasePath(Directory.GetCurrentDirectory())
    //        .AddJsonFile("appsettings.json")
    //        .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    //        .Build();
    //})
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