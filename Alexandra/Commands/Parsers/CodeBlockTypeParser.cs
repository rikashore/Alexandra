using System;
using System.Threading.Tasks;
using Alexandra.Common.Types;
using Disqord.Bot;
using Qmmands;

namespace Alexandra.Commands.Parsers
{
    public class CodeBlockTypeParser : DiscordTypeParser<CodeBlock>
    {
        public override ValueTask<TypeParserResult<CodeBlock>> ParseAsync(Parameter parameter, string value, DiscordCommandContext context)
        {
            var cs1 = value.IndexOf($"```", StringComparison.OrdinalIgnoreCase);
            if (cs1 == -1)
                return Failure("Not a valid code block");
            
            cs1 = value.IndexOf('\n', cs1) + 1;
            var cs2 = value.LastIndexOf("```", StringComparison.OrdinalIgnoreCase);

            if (cs1 == -1 || cs2 == -1)
                return Failure("Not a valid code block");

            var code = value.Substring(cs1, cs2 - cs1);
            return Success(new CodeBlock(code));
        }
    }
}