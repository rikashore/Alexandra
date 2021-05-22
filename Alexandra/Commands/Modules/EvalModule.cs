using Alexandra.Commands.Bases;
using Alexandra.Services;
using Disqord;
using Disqord.Bot;
using Qmmands;

namespace Alexandra.Commands.Modules
{
    [Group("eval")]
    public class EvalModule : LexModuleBase
    {
        private readonly EvalService _evalService;
        private readonly ParseService _parseService;

        public EvalModule(EvalService evalService, ParseService parseService)
        {
            _evalService = evalService;
            _parseService = parseService;
        }
        
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
            
            var parseSuccess = _parseService.TryParseCodeBlock(codeString, out var codeBlock);

            if (!parseSuccess) 
                return InvalidCodeResponse();
            
            var evalResult = _evalService.EvalLuaCode(codeBlock);
            return Response(evalResult);
        }
    }
}