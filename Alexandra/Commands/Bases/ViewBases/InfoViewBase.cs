using System.Threading.Tasks;
using Alexandra.Common.Extensions;
using Alexandra.Common.Globals;
using Disqord;
using Disqord.Extensions.Interactivity.Menus;

namespace Alexandra.Commands.Bases.ViewBases
{
    public class InfoViewBase : ViewBase
    {
        private static readonly LocalEmbed MainInfoEmbed = new LocalEmbed()
            .WithLexColor()
            .WithTitle("Alexandra")
            .AddField("Author", "shift-eleven#7304")
            .AddField("Source Code", Markdown.Link("GitHub", LexGlobals.LexRepo), true)
            .AddField("Octokit.net", Markdown.Link("GitHub", LexGlobals.OctokitRepo), true)
            .AddField("Library", Markdown.Link("Disqord " + Library.Version, Library.RepositoryUrl), true);

        private static readonly LocalEmbed SecondaryInfoEmbed = new LocalEmbed()
            .WithLexColor()
            .WithTitle("Some more info")
            .WithDescription(
                "Alexandra was made to help me around my guild, it contains a bunch of fun commands and misc commands. It's also inspired by victorian London, my favourite era of time!");

        public InfoViewBase() : base(new LocalMessage().WithEmbeds(MainInfoEmbed))
        {
            AddComponent(new LinkButtonViewComponent("https://github.com/shift-eleven/Alexandra")
            {
                Label = "Source",
                Position = 2
            });
        }

        [Button(Label = "Main info", Style = LocalButtonComponentStyle.Secondary)]
        public ValueTask MainInfo(ButtonEventArgs e)
        {
            if (TemplateMessage.Embeds[0].Title == "Alexandra")
                return default;

            TemplateMessage.Embeds[0] = MainInfoEmbed;
            ReportChanges();
            return default;
        }

        [Button(Label = "Some more info", Style = LocalButtonComponentStyle.Secondary)]
        public ValueTask SecondaryInfo(ButtonEventArgs e)
        {
            if (TemplateMessage.Embeds[0].Title == "Some more info")
                return default;

            TemplateMessage.Embeds[0] = SecondaryInfoEmbed;
            ReportChanges();
            return default;
        }
    }
}