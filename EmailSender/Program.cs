using EmailSender;
using EmailSender.Infrastructure;
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
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddScoped<IMailSender, MailSender>();
        //var emailConfig = Configuration
        //.GetSection("EmailConfiguration")
        //.Get<EmailConfiguration>();
        //builder.Services.AddSingleton(emailConfig);

    })
    .Build();

await host.RunAsync();