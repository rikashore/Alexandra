using System.Collections.Generic;
using Disqord.Extensions.Interactivity.Menus.Paged;

namespace Alexandra.Common.Utilities
{
    public class PastyListPageProvider : ListPageProvider
    {
        public PastyListPageProvider(IEnumerable<Page> pages) : base(pages)
        { }

        public PastyListPageProvider(List<Page> pages) : base(pages)
        { }
    }
}