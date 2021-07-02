using System;
using System.Net.Http;
using System.Threading.Tasks;
using Alexandra.Common.Extensions;
using Alexandra.Common.Types;
using ImageMagick;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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

        public async Task<ColorInfoData> GetColorInfo(string colorString)
        {
            var response = await _client.GetAsync($"https://www.thecolorapi.com/id?hex={colorString}");
            var result = await response.Content.ReadAsStreamAsync();
            return result.DeserializeTo<ColorInfoData>();
        }
    }
}