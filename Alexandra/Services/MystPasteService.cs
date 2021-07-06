using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MystPaste.NET;

namespace Alexandra.Services
{
    public class MystPasteService : LexService
    {
        private readonly MystPasteClient _mystPasteClient;

        public MystPasteService(ILogger<MystPasteService> logger) : base(logger)
        {
            _mystPasteClient = new MystPasteClient();
        }

        public async Task<Paste> GetPasties(string id)
        {
            return await _mystPasteClient.Paste.GetPasteAsync(id);
        }

        public Task<Language> GetLanguageByNameAsync(string language)
        {
            return _mystPasteClient.Data.GetLanguageByNameAsync(language);
        }

        public Task<Language> GetLanguageByExtensionAsync(string extension)
        {
            return _mystPasteClient.Data.GetLanguageByExtensionAsync(extension);
        }
    }
}