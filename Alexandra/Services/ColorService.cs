using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ImageMagick;
using Microsoft.Extensions.Logging;

namespace Alexandra.Services
{
    public class ColorService : LexService
    {
        private readonly HttpClient _client;
        
        public ColorService(ILogger<ColorService> logger, HttpClient client) : base(logger)
        {
            _client = client;
        }

        public string GetColorImage(string hexColor)
        {
            using var image = new MagickImage(new MagickColor(hexColor), 200, 100);
            var path = $"{Guid.NewGuid()}.png";
            image.Write(path);
            return path;
        }
    }
}