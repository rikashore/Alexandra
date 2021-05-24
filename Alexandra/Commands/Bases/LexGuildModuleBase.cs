using Disqord.Bot;

namespace Alexandra.Commands.Bases
{
    public class LexGuildModuleBase : DiscordGuildModuleBase
    {
        protected DiscordCommandResult NoResultsFoundResponse() 
            => Response("It seems not result has been found.");

        protected DiscordCommandResult NotFoundResponse(string item)
            => Response($"It seems, a/an {item} with that name could not be found, perchance try again.");
    }
}