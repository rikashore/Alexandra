using Alexandra.Common.Extensions;
using Alexandra.Common.Globals;
using Disqord;

namespace Alexandra.Common.Utilities
{
    public class LexEmbed : LocalEmbed
    {
        public LexEmbed()
        {
            WithLexColor();
        }

        private LexEmbed WithLexColor()
            => this.WithColor(LexGlobals.LexColor);

        public LexEmbed OverrideColor(Color? color)
        {
            if (color is null)
            {
                WithLexColor();
                return this;
            }

            this.WithColor(color);
            return this;
        }
    }
}