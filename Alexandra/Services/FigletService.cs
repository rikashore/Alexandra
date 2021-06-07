using System.Collections.Generic;
using System.IO;
using System.Linq;
using Figgle;
using Microsoft.Extensions.Logging;

namespace Alexandra.Services
{
    public class FigletService : LexService
    {
        public FigletService(ILogger<FigletService> logger) : base(logger)
        { }

        public string GetRenderedText(string text) 
            => $"```\n{FiggleFonts.Standard.Render(text.ToLower())}\n```";
    }
}