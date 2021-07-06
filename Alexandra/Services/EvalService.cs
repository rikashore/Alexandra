using Alexandra.Common.Extensions;
using Alexandra.Common.Utilities;
using Disqord;
using Microsoft.Extensions.Logging;
using MoonSharp.Interpreter;

namespace Alexandra.Services
{
    public class EvalService : LexService
    {
        public EvalService(ILogger<EvalService> logger) : base(logger)
        { }

        public LocalEmbed EvalLuaCode(string codeBlock)
        {
            try
            {
                var script = new Script(CoreModules.Preset_SoftSandbox);
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

                var resEmbed = new LexEmbed()
                    .WithTitle("Evaluation Success")
                    .WithDescription(codeReturn)
                    .OverrideColor(new Color(161, 11, 11));

                return resEmbed;
            }
            catch (ScriptRuntimeException scriptRuntimeException)
            {
                var errCodeBlock = Markdown.CodeBlock("lua", scriptRuntimeException.DecoratedMessage);

                var errEmbed = new LexEmbed()
                    .WithTitle("An error occured, Runtime Exception")
                    .WithDescription(errCodeBlock)
                    .OverrideColor(new Color(161, 11, 11));
                
                Logger.LogError("Unsuccessfully evaluated Lua code, Runtime Exception");
                
                return errEmbed;
            }
            catch (SyntaxErrorException syntaxErrorException)
            {
                var errCodeBlock =  Markdown.CodeBlock("lua", syntaxErrorException.DecoratedMessage);

                var errEmbed = new LexEmbed()
                    .WithTitle("An error occured, Syntax Exception")
                    .WithDescription(errCodeBlock)
                    .OverrideColor(new Color(161, 11, 11));
                
                Logger.LogError("Unsuccessfully evaluated Lua code, Syntax exception");

                return errEmbed;
            }
        }
    }
}