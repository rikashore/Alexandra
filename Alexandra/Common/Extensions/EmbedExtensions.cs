using Alexandra.Common.Globals;
using Disqord;

namespace Alexandra.Common.Extensions
{
    public static class EmbedExtensions
    {
        public static LocalEmbedBuilder WithLexColor(this LocalEmbedBuilder builder)
            => builder.WithColor(LexGlobals.LexColor);
    }
}