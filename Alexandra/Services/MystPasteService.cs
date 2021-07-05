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

        public async Task<List<Pasty>> GetPasties(string id)
        {
            var paste = await _mystPasteClient.Paste.GetPasteAsync(id);
            return paste.Pasties;
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