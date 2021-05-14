using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Alexandra.Common.Extensions;
using Alexandra.Common.Globals;
using Alexandra.Database;
using Alexandra.Database.Helpers;
using Alexandra.Disqord;
using Disqord;
using Disqord.Bot.Hosting;
using Disqord.Gateway;
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

                    x.Services.Remove(x.Services.First(x => x.ServiceType == typeof(ILogger<>)));
                    x.Services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
                })
                .ConfigureDiscordBot<LexDisqordBot>((context, bot) =>
                {
                    bot.Token = context.Configuration["token"];
                    bot.Intents = GatewayIntents.All;
                    bot.OwnerIds = new Snowflake[] {new Snowflake(LexGlobals.AuthorId)};
                    bot.UseMentionPrefix = true;
                    bot.Prefixes = new[] {"lex"};
                })
                .ConfigureServices((context, services) =>
                {
                    var connection = context.Configuration["dbconn"];
                    services.AddDbContext<LexDbContext>(x => 
                            x.UseMySql(connection, new MySqlServerVersion(new Version(8, 0, 21))))
                        .AddSingleton<HttpClient>()
                        .AddSingleton(new GitHubClient(new Octokit.ProductHeaderValue("Alexandra-The-Discord-Bot")))
                        .AddSingleton<TagHelper>()
                        .AddSingleton<NoteHelper>()
                        .AddLexServices()
                        .AddSingleton<Random>();
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