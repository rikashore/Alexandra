using Alexandra.Common.Extensions;
using Disqord;
using Microsoft.Extensions.Logging;
using MoonSharp.Interpreter;

namespace Alexandra.Services
{
    public class EvalService : LexService
    {
        public EvalService(ILogger<EvalService> logger) : base(logger)
        { }

        public LocalEmbedBuilder EvalLuaCode(string codeBlock)
        {
            try
            {
                var script = new Script();
                var res = script.DoString(codeBlock);

                object resString = res.Type switch
                {
                    DataType.String => res.String,
                    DataType.Number => res.Number,
                    DataType.Boolean => res.Boolean,
                    DataType.Function => res.Function,
                    DataType.Nil => "null",
                    DataType.Void => "void",
                    DataType.Table => res.Table,
                    DataType.Tuple => res.Tuple,
                    DataType.UserData => "no return type",
                    DataType.Thread => "no return type",
                    DataType.ClrFunction => res.Callback.Name,
                    DataType.TailCallRequest => res.UserData.Descriptor.Name,
                    DataType.YieldRequest => "no return type",
                    _ => "no return type"
                };

                var codeReturn = Markdown.CodeBlock("lua", resString.ToString());

                var resEmbed = new LocalEmbedBuilder()
                    .WithTitle("Evaluation Success")
                    .WithDescription(codeReturn)
                    .WithLexColor();

                return resEmbed;
            }
            catch (ScriptRuntimeException scriptRuntimeException)
            {
                var errCodeBlock = Markdown.CodeBlock("lua", scriptRuntimeException.DecoratedMessage);

                var errEmbed = new LocalEmbedBuilder()
                    .WithTitle("An error occured, Runtime Exception")
                    .WithDescription(errCodeBlock)
                    .WithLexColor();
                
                Logger.LogError("Unsuccessfully evaluated Lua code, Runtime Exception");
                
                return errEmbed;
            }
            catch (SyntaxErrorException syntaxErrorException)
            {
                var errCodeBlock =  Markdown.CodeBlock("lua", syntaxErrorException.DecoratedMessage);

                var errEmbed = new LocalEmbedBuilder()
                    .WithTitle("An error occured, Syntax Exception")
                    .WithDescription(errCodeBlock)
                    .WithLexColor();
                
                Logger.LogError("Unsuccessfully evaluated Lua code, Syntax exception");

                return errEmbed;
            }
        }
    }
}