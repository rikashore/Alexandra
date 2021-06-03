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

        public bool ValidateFontName(string fontName, out FiggleFont font)
        {
            font = null;
            var fontNames = _figletFonts.Keys.ToArray();

            if (fontNames.All(x => x != fontName)) return false;
            
            font = _figletFonts[fontName];
            return true;
        }

        public string GetRenderedText(string text, FiggleFont font) => $"```\n{font.Render(text.ToLower())}\n```";

        public FiggleFont GetFont(string fontName) => _figletFonts[fontName];

        private void AddFonts()
        {
            using (var threeDStream = File.OpenRead(@"Figlet\3D-ASCII.flf"))
            {
                var threeD = FiggleFontParser.Parse(threeDStream);
            
                _figletFonts.Add("3d", threeD);
            }

            using (var regularStream = File.OpenRead(@"Figlet\ANSI Regular.flf"))
            {
                var regular = FiggleFontParser.Parse(regularStream);
                
                _figletFonts.Add("regular", regular);
            }

            using (var shadowStream = File.OpenRead(@"Figlet\ANSI Shadow.flf"))
            {
                var shadow = FiggleFontParser.Parse(shadowStream);
                
                _figletFonts.Add("shadow", shadow);
            }
            
            using (var bloodyStream = File.OpenRead(@"Figlet\Bloody.flf"))
            {
                var bloody = FiggleFontParser.Parse(bloodyStream);
                
                _figletFonts.Add("bloody", bloody);
            }
            
            using (var calvinSStream = File.OpenRead(@"Figlet\Calvin S.flf"))
            {
                var calvinS = FiggleFontParser.Parse(calvinSStream);
                
                _figletFonts.Add("calvin-s", calvinS);
            }
            
            using (var deltaCorpsStream = File.OpenRead(@"Figlet\Delta Corps Priest 1.flf"))
            {
                var deltaCorps = FiggleFontParser.Parse(deltaCorpsStream);
                
                _figletFonts.Add("delta-corps", deltaCorps);
            }
            
            _figletFonts.Add("standard", FiggleFonts.Standard);
        }
    }
}