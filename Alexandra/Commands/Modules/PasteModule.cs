using System.Linq;
using System.Threading.Tasks;
using Alexandra.Commands.Bases;
using Alexandra.Common.Extensions;
using Alexandra.Common.Globals;
using Alexandra.Common.Utilities;
using Alexandra.Services;
using Disqord;
using Disqord.Bot;
using Qmmands;

namespace Alexandra.Commands.Modules
{
    [Group("paste")]
    [Description("Paste related commands")]
    [Name("Paste")]
    public class PasteModule : LexModuleBase
    {
        private readonly MystPasteService _mystPasteService;

        public PasteModule(MystPasteService mystPasteService)
        {
            _mystPasteService = mystPasteService;
        }

        [Command("get")]
        [Description("Get a paste")]
        public async Task<DiscordCommandResult> GetPastesAsync([Description("The id of the paste to grab")] string id)
        {
            var pasties = await _mystPasteService.GetPasties(id);

            if (pasties.Count == 1)
                return Response(Markdown.CodeBlock(pasties[0].Code.CutIfLong()));
            
            return Pages(new PastyListPageProvider(pasties.ToPastyPageList()));
        }

        [Command("lang", "language")]
        public async Task<DiscordCommandResult> GetLanguageByNameInfoAsync(string language)
        {
            var lang = await _mystPasteService.GetLanguageByNameAsync(language);

            var parser = Context.Bot.Commands.GetTypeParser<Color>();
            
            Color langColor;

            if (lang.HexColor == "#ffffff")
                langColor = LexGlobals.LexColor;
            else
            {
                var res = await parser.ParseAsync(null, lang.HexColor, Context);
                langColor = res.Value;
            }
            
            var le = new LocalEmbed()
                .WithTitle(lang.Name)
                .WithColor(langColor)
                .AddField("Extensions", lang.Extensions.Count == 0
                    ? "No extensions"
                    : string.Join(' ', lang.Extensions.Select(x => Markdown.Code(x))))
                .AddField("Mimes", lang.Mimes.Count == 0
                    ? "No mimes"
                    : string.Join(' ', lang.Mimes.Select(x => Markdown.Code(x))))
                .AddField("Aliases", lang.Aliases.Count == 0
                    ? "No aliases"
                    : string.Join(' ', lang.Aliases))
                .AddField("Editor Mode", lang.Mode);

            return Response(le);
        }
        
        [Command("ext", "extension")]
        public async Task<DiscordCommandResult> GetLanguageByExtensionInfoAsync(string extension)
        {
            var lang = await _mystPasteService.GetLanguageByExtensionAsync(extension);

            var parser = Context.Bot.Commands.GetTypeParser<Color>();

            Color langColor;

            if (lang.HexColor == "#ffffff")
                langColor = LexGlobals.LexColor;
            else
            {
                var res = await parser.ParseAsync(null, lang.HexColor, Context);
                langColor = res.Value;
            }

            var le = new LocalEmbed()
                .WithTitle(lang.Name)
                .WithColor(langColor)
                .AddField("Extensions", lang.Extensions.Count == 0
                    ? "No extensions"
                    : string.Join(' ', lang.Extensions.Select(x => Markdown.Code(x))))
                .AddField("Mimes", lang.Mimes.Count == 0
                    ? "No mimes"
                    : string.Join(' ', lang.Mimes.Select(x => Markdown.Code(x))))
                .AddField("Aliases", lang.Aliases.Count == 0
                    ? "No aliases"
                    : string.Join(' ', lang.Aliases))
                .AddField("Editor Mode", lang.Mode);

            return Response(le);
        }
    }
}