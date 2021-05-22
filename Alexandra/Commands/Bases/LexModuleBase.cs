using Disqord.Bot;

namespace Alexandra.Commands.Bases
{
    public class LexModuleBase : DiscordModuleBase
    {
        protected DiscordCommandResult NoteNotFoundResponse()
            => Response("No note with that ID could be found");

        protected DiscordCommandResult InvalidAccessResponse()
            => Response("I cannot let you edit other's notes");

        protected DiscordCommandResult InvalidCodeResponse()
            => Response("It seems you haven't given me code to execute");
    }
}