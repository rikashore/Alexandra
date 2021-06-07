using Alexandra.Services;
using Disqord.Bot;
using Microsoft.Extensions.DependencyInjection;

namespace Alexandra.Commands.Bases.ModuleBases
{
    public class LexNoteModuleBase : LexModuleBase
    {
        protected NoteService NoteService => Context.Services.GetRequiredService<NoteService>(); 
            
        protected DiscordCommandResult InvalidAccessResponse()
            => Response("I cannot let you edit other's notes");

        protected DiscordCommandResult NoteNotFoundResponse()
            => Response("No note with that ID could be found");
    }
}