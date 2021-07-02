using Alexandra.Common.Extensions;
using Disqord;

namespace Alexandra.Common.Utilities
{
    public class LexEmbed : LocalEmbed
    {
        public LexEmbed()
        {
            this.WithLexColor();
        }
    }
}