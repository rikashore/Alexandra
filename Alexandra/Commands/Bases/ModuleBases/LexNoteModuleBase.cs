using Disqord.Bot;

namespace Alexandra.Commands.Bases.ModuleBases
{
    public class LexNoteModuleBase : LexModuleBase
    {
        protected DiscordCommandResult InvalidAccessResponse()
            => Response("I cannot let you edit other's notes");

        protected DiscordCommandResult NoteNotFoundResponse()
            => Response("No note with that ID could be found");
    }
}