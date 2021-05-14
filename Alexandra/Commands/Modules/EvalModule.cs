using System.Threading.Tasks;
using Alexandra.Common.Types;
using Alexandra.Services;
using Disqord;
using Disqord.Bot;
using Qmmands;

namespace Alexandra.Commands.Modules
{
    [Group("eval")]
    public class EvalModule : DiscordModuleBase
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
        public DiscordCommandResult EvalLuaAsync([Remainder]string codeString)
        {
            var parseSuccess = _parseService.TryParseCodeBlock(codeString, out CodeBlock codeBlock);
            
            if(parseSuccess)
            {
                var evalResult = _evalService.EvalLuaCode(codeBlock);
                return Response(evalResult);
            }
            
            return Response("It seems you haven't given me code to execute");
        }
    }
}