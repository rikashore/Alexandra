using Alexandra.Commands.Bases.ModuleBases;
using Alexandra.Commands.TypeParsers;
using Disqord;
using Disqord.Bot;
using Qmmands;

namespace Alexandra.Commands.Modules
{
    [Group("eval")]
    [Name("Eval")]
    [Description("Commands that evaluate certain pieces of code")]
    [RequireBotOwner]
    public class EvalModule : LexEvalModuleBase
    {
        [Command("Lua")]
        [Description("Evaluate some Lua code")]
        public DiscordCommandResult EvalLuaAsync(
            [Name("Code String"), Description("The code to execute"), OverrideTypeParser(typeof(CodeBlockTypeParser)), Remainder] string codeString)
        {
            var evalResult = EvalService.EvalLuaCode(codeString);
            return Response(evalResult);
        }
    }
}