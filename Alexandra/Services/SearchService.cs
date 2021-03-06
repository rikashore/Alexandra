using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Alexandra.Common.Extensions;
using Alexandra.Common.Types;
using MerriamWebster.NET;
using MerriamWebster.NET.Dto;
using MerriamWebster.NET.Parsing;
using Microsoft.Extensions.Logging;

namespace Alexandra.Services
{
    public class SearchService : LexService
    {
        private readonly HttpClient _httpClient;
        private readonly IEntryParser _entryParser;
        
        public SearchService(ILogger<SearchService> logger, HttpClient httpClient, IEntryParser entryParser) : base(logger)
        {
            _httpClient = httpClient;
            _entryParser = entryParser;
        }

        public async Task<RandomWordData> GetRandomWordResponseAsync()
        {
            var response = await _httpClient.GetAsync("https://random-words-api.vercel.app/word");
            var res = await response.Content.ReadAsStreamAsync();
            var result = res.DeserializeTo<List<RandomWordData>>();
            return result?[0];
        }

        public async Task<EntryModel> GetDefinitionAsync(string word)
            => await _entryParser.GetAndParseAsync(Configuration.CollegiateDictionary, word);
        
    }
}