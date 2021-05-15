using System;
using Microsoft.Extensions.Logging;

namespace Alexandra.Services
{
    public class ParseService : LexService
    {
        public ParseService(ILogger<ParseService> logger) : base(logger)
        { }

        public bool TryParseCodeBlock(string s, out string codeBlock)
        {
            codeBlock = null;
            
            var cs1 = s.IndexOf($"```", StringComparison.OrdinalIgnoreCase);
            if (cs1 == -1)
                return false;
            
            cs1 = s.IndexOf('\n', cs1) + 1;
            var cs2 = s.LastIndexOf("```", StringComparison.OrdinalIgnoreCase);

            if (cs1 == -1 || cs2 == -1)
                return false;

            codeBlock = s.Substring(cs1, cs2 - cs1);
            return true;
        }
    }
}