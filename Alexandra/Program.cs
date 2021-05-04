using System;
using System.Linq;
using System.Net.Http;
using Disqord.Bot.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Alexandra
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureAppConfiguration(x =>
                {
                    x.AddCommandLine(args);
                    x.AddJsonFile("config.json");
                })
                .ConfigureLogging(x =>
                {
                    var logger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                        .WriteTo.Console(theme: SystemConsoleTheme.Grayscale)
                        .CreateLogger();
                    x.AddSerilog(logger, true);

                    x.Services.Remove(x.Services.First(x => x.ServiceType == typeof(ILogger<>)));
                    x.Services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
                })
                .ConfigureDiscordBot((context, bot) =>
                {
                    bot.Token = context.Configuration["token"];
                    bot.UseMentionPrefix = true;
                    bot.Prefixes = new[] {"lex"};
                })
                .ConfigureServices(x =>
                {
                    x.AddSingleton<HttpClient>();
                })
                .Build();

            try
            {
                host.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadLine();
            }
        }
    }
}