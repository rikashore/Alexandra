using System;
using System.Threading;
using System.Threading.Tasks;
using Alexandra.Commands.Parsers;
using Disqord;
using Disqord.Bot;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qmmands;

namespace Alexandra.Disqord
{
    public class LexDisqordBot : DiscordBot
    {
        public LexDisqordBot(IOptions<DiscordBotConfiguration> options, ILogger<DiscordBot> logger,
            IPrefixProvider prefixes, ICommandQueue queue, CommandService commands, IServiceProvider services,
            DiscordClient client) : base(options, logger, prefixes, queue, commands, services, client) { }

        protected override ValueTask AddTypeParsersAsync(CancellationToken cancellationToken = default)
        {
            Commands.AddTypeParser(new CodeBlockTypeParser());
            return base.AddTypeParsersAsync(cancellationToken);
        }
    }
}