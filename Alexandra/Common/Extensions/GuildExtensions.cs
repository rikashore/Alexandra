using System.Collections.Generic;
using Disqord.Gateway;
using Humanizer;

namespace Alexandra.Common.Extensions
{
    public static class GuildExtensions
    {
        public static string GetGuildFeatures(this CachedGuild guild)
        {
            var featureList = new List<string>();
            var features = guild.Features;

            if (features.Count == 0)
                return "No features";

            foreach (var feature in features)
            {
                featureList.Add(feature.Humanize().Transform(To.LowerCase, To.SentenceCase));
            }

            return featureList.NewlineQuote();
        }
    }
}