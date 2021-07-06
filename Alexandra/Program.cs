using System;
using System.Linq;      
using Alexandra.Common.Extensions;
using Alexandra.Common.Globals;
using Alexandra.Common.Utilities;
using Alexandra.Database;
using Alexandra.Disqord;
using Alexandra.Services;
using Disqord;
using Disqord.Bot.Hosting;
using Disqord.Gateway;
using MerriamWebster.NET;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Octokit;
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
                        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}",theme: SystemConsoleTheme.Grayscale)
                        .CreateLogger();
                    x.AddSerilog(logger, true);

                    x.Services.Remove(x.Services.First(serviceDescriptor => serviceDescriptor.ServiceType == typeof(ILogger<>)));
                    x.Services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
                })
                .ConfigureDiscordBot<LexDisqordBot>((context, bot) =>
                {
                    bot.Token = context.Configuration["discord:token"];
                    bot.Intents = GatewayIntents.All;
                    bot.OwnerIds = new[] {new Snowflake(LexGlobals.AuthorId)};
                    bot.Activities = new[] { new LocalActivity("lexhelp", ActivityType.Playing) };
                    bot.UseMentionPrefix = true;
                    bot.Prefixes = context.Configuration.GetSection("discord:prefixes").Get<string[]>();
                })
                .ConfigureServices((context, services) =>
                {
                    var connection = context.Configuration["database:connection"];
                    var mwConfig = context.Configuration.GetSection("MerriamWebster").Get<MerriamWebsterConfig>();

                    services.AddHttpClient<ColorService>();
                    services.AddHttpClient<SearchService>();
                    
                    services.AddDbContext<LexDbContext>(x => 
                            x.UseNpgsql(connection).UseSnakeCaseNamingConvention())
                        .AddSingleton(new GitHubClient(new ProductHeaderValue("Alexandra-The-Discord-Bot")))
                        .AddSingleton<Random>()
                        .RegisterMerriamWebster(mwConfig)
                        .AddLexServices();
                })
                .Build();

            try
            {
                Console.WriteLine(LexGlobals.LexAscii);
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