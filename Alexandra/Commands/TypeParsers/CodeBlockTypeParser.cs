using System;
using System.Threading.Tasks;
using Disqord.Bot;
using Qmmands;

namespace Alexandra.Commands.TypeParsers
{
    public class CodeBlockTypeParser : DiscordTypeParser<string>
    {
        public override ValueTask<TypeParserResult<string>> ParseAsync(Parameter parameter, string value, DiscordCommandContext context)
        {
            var cs1 = value.IndexOf($"```", StringComparison.OrdinalIgnoreCase);
            if (cs1 == -1)
                return Failure("It seems you haven't given me a valid code block.");
            
            cs1 = value.IndexOf('\n', cs1) + 1;
            var cs2 = value.LastIndexOf("```", StringComparison.OrdinalIgnoreCase);

            if (cs1 == -1 || cs2 == -1)
                return Failure("It seems you haven't given me a valid code block.");

            var codeBlock = value.Substring(cs1, cs2 - cs1);
            return Success(codeBlock);
        }
    }
}