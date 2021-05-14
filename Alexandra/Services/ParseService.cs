using System;
using Alexandra.Common.Types;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Alexandra.Services
{
    public class ParseService : LexService
    {
        public ParseService(ILogger<ParseService> logger) : base(logger)
        { }

        public bool TryParseCodeBlock(string s, out CodeBlock codeBlock)
        {
            codeBlock = null;
            
            var cs1 = s.IndexOf($"```", StringComparison.OrdinalIgnoreCase);
            if (cs1 == -1)
                return false;
            
            cs1 = s.IndexOf('\n', cs1) + 1;
            var cs2 = s.LastIndexOf("```", StringComparison.OrdinalIgnoreCase);

            if (cs1 == -1 || cs2 == -1)
                return false;

            codeBlock = new CodeBlock(s.Substring(cs1, cs2 - cs1));
            return true;
        }
    }
}