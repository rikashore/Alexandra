using Disqord.Serialization.Json;

namespace Alexandra.Common.Types
{
    public class RandomWordData
    {
        [JsonProperty("word")]
        public string Word { get; set; }

        [JsonProperty("definition")]
        public string Definition { get; set; }

        [JsonProperty("pronunciation")]
        public string Pronunciation { get; set; }
    }
}