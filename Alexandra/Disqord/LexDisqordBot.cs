using System;
using System.Threading;
using System.Threading.Tasks;
using Alexandra.Commands.TypeParsers;
using Disqord;
using Disqord.Bot;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Alexandra.Disqord
{
    public class LexDisqordBot : DiscordBot
    {
        public LexDisqordBot(IOptions<DiscordBotConfiguration> options, 
            ILogger<DiscordBot> logger,
            IServiceProvider services,
            DiscordClient client) 
            : base(options, logger, services, client) 
        { }

        protected override ValueTask AddTypeParsersAsync(CancellationToken cancellationToken = default)
        {
            Commands.AddTypeParser(new CodeBlockTypeParser());
            return base.AddTypeParsersAsync(cancellationToken);
        }
    }
}