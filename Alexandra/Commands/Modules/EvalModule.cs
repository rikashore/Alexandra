using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Alexandra.Commands.Bases.ModuleBases;
using Alexandra.Commands.TypeParsers;
using Alexandra.Common.Extensions;
using Alexandra.Common.Globals;
using Alexandra.Common.Utilities;
using Disqord;
using Disqord.Bot;
using Disqord.Rest;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Logging;
using Qmmands;

namespace Alexandra.Commands.Modules
{
    [Group("eval")]
    [Name("Eval")]
    [Description("Commands that evaluate certain pieces of code")]
    [RequireBotOwner]
    public class EvalModule : LexEvalModuleBase
    {
        [Command("Lua"), RunMode(RunMode.Parallel)]
        [Description("Evaluate some Lua code")]
        public DiscordCommandResult EvalLuaAsync(
            [Name("Code String"), Description("The code to execute"), OverrideTypeParser(typeof(CodeBlockTypeParser)), Remainder] string codeString)
        {
            var evalResult = EvalService.EvalLuaCode(codeString);
            return Response(evalResult);
        }

        [Command, RunMode(RunMode.Parallel)]
        [Description("Eval some C#")]
        public async Task<DiscordCommandResult> EvalCSharpAsync([Remainder] string code)
        {
            if (code == null)
            {
                var messageRef = Context.Message.ReferencedMessage.GetValueOrDefault();
                if (messageRef is not null)
                    code = messageRef.Content;
                else
                    return Response("More code needed, sir.");
            }
            code = EvalUtils.ValidateCode(code);
            var scriptOptions = ScriptOptions.Default
                .WithImports(EvalUtils.EvalNamespaces)
                .WithReferences(AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location)));
            
            var script = CSharpScript.Create(code, scriptOptions, Context is DiscordGuildCommandContext ? typeof(EvalGuildGlobals) : typeof(EvalGlobals));
            try
            {
                using (Context.Bot.BeginTyping(Context.ChannelId))
                {
                    var stopwatch = Stopwatch.StartNew();
                    var diagnostics = script.Compile();
                    stopwatch.Stop();
                    if (diagnostics.Any(x => x.Severity == DiagnosticSeverity.Error))
                    {
                        var eb = new LocalEmbed()
                         .WithTitle("Compilation Failure")
                        .WithDescription($"{diagnostics.Length} {(diagnostics.Length > 1 ? "errors" : "error")}")
                        .WithColor(Color.Red)
                        .WithFooter($"{stopwatch.Elapsed.TotalMilliseconds}ms");
                         for (var i = 0; i < diagnostics.Length; i++)
                        {
                            if (i > 3)
                                break;
                            var diagnostic = diagnostics[i];
                            var lineSpan = diagnostic.Location.GetLineSpan().Span;
                            eb.AddField($"Error `{diagnostic.Id}` at {lineSpan.Start} - {lineSpan.End}", diagnostic.GetMessage());
                        }
                         
                        if (diagnostics.Length > 4) 
                            eb.AddField($"Skipped {diagnostics.Length - 4} {(diagnostics.Length - 4 > 1 ? "errors" : "error")}", "You should be able to fix it.");
                        return Response(eb);
                    }
                    var globals = Context is DiscordGuildCommandContext guildContext ? new EvalGuildGlobals(guildContext) : new EvalGlobals(Context);
                    var state = await script.RunAsync(globals, _ => true);
                    if (state.Exception != null)
                    {
                        var eb = new LocalEmbed()
                            .WithTitle("Execution Failure")
                            .WithDescription(state.Exception.ToString().SplitInParts(LocalEmbed.MAX_DESCRIPTION_LENGTH).First())
                            .WithColor(Color.Red)
                            .WithFooter($"{stopwatch.Elapsed.TotalMilliseconds}ms");
                        return Response(eb);
                    }
                    
                    switch (state.ReturnValue)
                    {
                        case null:
                        case string value when string.IsNullOrWhiteSpace(value):
                            return Reaction(new LocalEmoji("✅"));
                        case DiscordCommandResult commandResult:
                            return commandResult;
                        default:
                            return Response(state.ReturnValue.ToString());
                    }
                }
                
            }
            catch (Exception ex)
            {
                Context.Bot.Logger.LogError(ex, "An unexpected exception occurred when evaluating code.");
                return Response($"An unexpected exception occurred: {ex.Message}.");
            }
        }
    }
}