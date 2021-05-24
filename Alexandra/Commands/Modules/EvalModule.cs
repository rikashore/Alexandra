using Alexandra.Commands.Bases.ModuleBases;
using Disqord;
using Disqord.Bot;
using Qmmands;

namespace Alexandra.Commands.Modules
{
    [Group("eval")]
    public class EvalModule : LexEvalModuleBase
    {
        [Command("Lua")]
        [Description("Evaluate some Lua code")]
        [RequireBotOwner]
        public DiscordCommandResult EvalLuaAsync([Remainder]string codeString = null)
        {
            if (codeString == null)
            {
                var reference = Context.Message.ReferencedMessage.GetValueOrDefault();
                if (reference is not null)
                    codeString = reference.Content;
                else
                    return InvalidCodeResponse();
            }
            
            var parseSuccess = ParseService.TryParseCodeBlock(codeString, out var codeBlock);

            if (!parseSuccess) 
                return InvalidCodeResponse();
            
            var evalResult = EvalService.EvalLuaCode(codeBlock);
            return Response(evalResult);
        }
    }
}