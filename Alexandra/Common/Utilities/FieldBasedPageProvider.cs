using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Disqord;
using Disqord.Extensions.Interactivity.Menus.Paged;
using Alexandra.Common.Globals;

namespace Alexandra.Common.Utilities
{
    // Thanks Zack for inspiring me
    // https://github.com/Zackattak01/CheeseBot/blob/main/CheeseBot/Disqord/FieldBasedPageProvider.cs
    public class FieldBasedPageProvider : PageProvider
    {
        private readonly List<Page> _pages;

        private readonly FieldBasedPageProviderConfiguration _configuration;

        public override int PageCount { get; }

        public FieldBasedPageProvider(IEnumerable<LocalEmbedField> fields, FieldBasedPageProviderConfiguration configuration = null)
        {
            _configuration = configuration ?? FieldBasedPageProviderConfiguration.Default;
            _pages = new List<Page>();

            var fieldArray = fields as LocalEmbedField[] ?? fields.ToArray();
            var totalFields = fieldArray.Length;
            // this is ugly, but ambiguous calls and math.ceiling returning a double forced my hand
            PageCount = (int)Math.Ceiling((decimal)totalFields / _configuration.FieldsPerPage);
            
            CreatePages(fieldArray, _configuration.FieldsPerPage);
        }

        private void CreatePages(LocalEmbedField[] fields, int fieldsPerPage)
        {
            var loopLimit = PageCount * fieldsPerPage;
            for (var i = 0; i < loopLimit; i+= fieldsPerPage)
            {
                var embedBuilder = new LocalEmbed
                {
                    Fields = fields[i..Math.Min(i + fieldsPerPage, fields.Length)],
                    Color = LexGlobals.LexColor
                };

                _pages.Add(new Page().WithContent(_configuration.Content).AddEmbed(embedBuilder));
            }
        }

        public override ValueTask<Page> GetPageAsync(PagedViewBase menu)
            => new ValueTask<Page>(_pages[menu.CurrentPageIndex]);
        
    }
}