using Alexandra.Commands.Modules;
using Alexandra.Services;
using Disqord.Bot;
using Microsoft.Extensions.DependencyInjection;

namespace Alexandra.Commands.Bases.ModuleBases
{
    public class LexEvalModuleBase : LexModuleBase
    {
        protected EvalService EvalService => Context.Services.GetRequiredService<EvalService>();
        protected ParseService ParseService => Context.Services.GetRequiredService<ParseService>();
        
        protected DiscordCommandResult InvalidCodeResponse()
            => Response("It seems you haven't given me code to execute");
    }
}