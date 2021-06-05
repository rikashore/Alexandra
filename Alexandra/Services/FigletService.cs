using System.Collections.Generic;
using System.IO;
using System.Linq;
using Figgle;
using Microsoft.Extensions.Logging;

namespace Alexandra.Services
{
    public class FigletService : LexService
    {
        private readonly Dictionary<string, FiggleFont> _figletFonts = new Dictionary<string, FiggleFont>();
        
        public FigletService(ILogger<FigletService> logger) : base(logger)
        {
            AddFonts();
        }

        public void ValidateFontName(string fontName, out FiggleFont font)
        {
            font = null;
            var fontNames = _figletFonts.Keys.ToArray();

            if (fontNames.All(x => x != fontName)) return;

            font = _figletFonts[fontName];
        }

        public string GetRenderedText(string text, FiggleFont font) 
            => $"```\n{font.Render(text.ToLower())}\n```";

        public string GetStandardRenderedText(string text) 
            => GetRenderedText(text, _figletFonts["standard"]);

        private void AddFonts()
        {
            using (var threeDStream = File.OpenRead(@"Figlet\3D-ASCII.flf"))
            {
                var threeD = FiggleFontParser.Parse(threeDStream);
            
                _figletFonts.Add("3d", threeD);
            }

            using (var bloodyStream = File.OpenRead(@"Figlet\Bloody.flf"))
            {
                var bloody = FiggleFontParser.Parse(bloodyStream);
                
                _figletFonts.Add("bloody", bloody);
            }

            _figletFonts.Add("standard", FiggleFonts.Standard);
        }
    }
}