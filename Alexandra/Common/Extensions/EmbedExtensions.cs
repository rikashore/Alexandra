using Alexandra.Common.Globals;
using Disqord;

namespace Alexandra.Common.Extensions
{
    public static class EmbedExtensions
    {
        public static LocalEmbed WithLexColor(this LocalEmbed builder)
            => builder.WithColor(LexGlobals.LexColor);

        public static LocalEmbed WithErrorColor(this LocalEmbed builder)
            => builder.WithColor(new Color(161, 11, 11));
    }
}