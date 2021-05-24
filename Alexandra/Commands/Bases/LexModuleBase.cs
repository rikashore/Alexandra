using Disqord.Bot;

namespace Alexandra.Commands.Bases
{
    public class LexModuleBase : DiscordModuleBase
    {
        protected virtual DiscordCommandResult InvalidAccessResponse()
            => Response("I cannot let you edit that");
    }
}