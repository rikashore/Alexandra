using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Alexandra.Commands.Bases;
using Alexandra.Common.Extensions;
using Alexandra.Common.Globals;
using Alexandra.Common.Utilities;
using Alexandra.Services;
using Disqord;
using Disqord.Bot;
using Disqord.Gateway;
using Disqord.Rest;
using Qmmands;

namespace Alexandra.Commands.Modules
{
    [Name("Fun")]
    [Description("A few miscellaneous and fun commands")]
    public class FunModule : LexGuildModuleBase
    {
        private readonly ColorService _colorService;
        private readonly FigletService _figletService;
        private readonly SearchService _searchService;

        public FunModule(ColorService colorService, FigletService figletService, SearchService searchService)
        {
            _colorService = colorService;
            _figletService = figletService;
            _searchService = searchService;
        }

        [Command("avatar", "av")]
        [Description("Receive a portrait of yourself or another person")]
        public DiscordCommandResult AvatarAsync([Description("The member for whom you wish to receive a portrait")] IMember member = null)
        {
            member ??= Context.Author;
            var avatarUrl = member.GetAvatarUrl();

            var eb = new LocalEmbed()
                .WithTitle($"A portrait of {member.Name}#{member.Discriminator}")
                .WithUrl(avatarUrl)
                .WithLexColor()
                .WithImageUrl(avatarUrl);

            return Response(eb);
        }

        [Command("userinfo", "whois")]
        [Description("Gain information about yourself or another user")]
        public DiscordCommandResult UserInfo([Description("The user for whom you wish to receive information")] IMember member = null)
        {
            member ??= Context.Author;
            
            var roles = member.GetRoles();
            var topRole = roles.Values.OrderByDescending(x => x.Position).First();

            var eb = new LocalEmbed()
                .WithTitle(member.Tag)
                .WithThumbnailUrl(member.GetAvatarUrl())
                .WithColor(topRole.Color ?? LexGlobals.LexColor)
                .AddField("Id", member.Id, true)
                .AddField("Nickname", member.Nick ?? "No nickname in this guild", true)
                .AddField("Is Bot", member.IsBot ? "Yes" : "No", true)
                .AddField("Joined At", member.JoinedAt.Value.ToString("f"), true)
                .AddField("Created At", member.CreatedAt().ToString("f"));

            return Response(eb);
        }

        [Command("serverinfo", "server")]
        [Description("Gain information about your server")]
        public async Task<DiscordCommandResult> ServerInfoAsync()
        {
            var owner = await Context.Guild.FetchMemberAsync(Context.Guild.OwnerId);
            var botMemberCount = Context.Guild.Members.Values.Count(x => x.IsBot);

            var eb = new LocalEmbed()
                .WithTitle(Context.Guild.Name)
                .WithDescription($"**ID:** {Context.Guild.Id}\n**Owner:** {owner}")
                .WithThumbnailUrl(Context.Guild.GetIconUrl())
                .WithFooter("Created")
                .WithTimestamp(Context.Guild.CreatedAt())
                .WithLexColor()
                .AddField("Member count", $"{Context.Guild.MemberCount} ({botMemberCount} bots)")
                .AddField("Channels", Context.Guild.GetChannels().Count)
                .AddField("Roles", Context.Guild.Roles.Count);

            return Response(eb);
        }

        [Command("color"), RunMode(RunMode.Parallel)]
        [Description("Brews random colors")]
        public async Task RandomColorAsync()
        {
            var color = Color.Random;
            var colorImagePath = _colorService.GetColorImage(color.ToString());
            using (var colorImage = LocalAttachment.File(colorImagePath, "colorImage.png"))
            {
                var eb = new LocalEmbed()
                    .WithColor(color)
                    .WithDescription($"Hex: {color.ToString()}\nRGB: {color.R} {color.G} {color.B}")
                    .WithImageUrl("attachment://colorImage.png");

                var mb = new LocalMessage()
                    .WithAttachments(colorImage)
                    .WithEmbed(eb);

                await Response(mb);
            }
            
            File.Delete(colorImagePath);
        }
        
        [Command("color"), RunMode(RunMode.Parallel)]
        [Description("Create an image for a particular color")]
        public async Task RandomColorAsync(Color color)
        {
            var colorImagePath = _colorService.GetColorImage(color.ToString());
            using (var colorImage = LocalAttachment.File(colorImagePath, "colorImage.png"))
            {
                var eb = new LocalEmbed()
                    .WithColor(color)
                    .WithDescription($"Hex: {color.ToString()}\nRGB: {color.R} {color.G} {color.B}")
                    .WithImageUrl("attachment://colorImage.png");

                var mb = new LocalMessage()
                    .WithAttachments(colorImage)
                    .WithEmbed(eb);

                await Response(mb);
            }
            
            File.Delete(colorImagePath);
        }
        
        [Command("word", "define", "def")]
        [Description("Grab the definition of the word from Merriam Webster")]
        public async Task<DiscordCommandResult> SearchWordDefinitionAsync([Remainder] string word)
        {
            var result = await _searchService.GetDefinitionAsync(word);
            
            switch (result.Entries.Count)
            {
                case 0:
                    return NoResultsFoundResponse();
                case <= 5:
                {
                    var i = 0;
                    var le = new LocalEmbed()
                        .WithTitle(result.SearchText)
                        .WithLexColor();

                    foreach (var entry in result.Entries)
                    {
                        foreach (var def in entry.ShortDefs)
                            le.AddField($"{++i}", def);
                    }

                    return Response(le);
                }
                default:
                {
                    var fieldBuilders = new List<LocalEmbedField>(result.Entries.Count);
                    var i = 0;
            
                    foreach (var entry in result.Entries)
                    {
                        foreach (var def in entry.ShortDefs)
                        {
                            fieldBuilders.Add(new LocalEmbedField().WithName($"{i++}").WithValue(def));
                        }
                    }

                    var config = FieldBasedPageProviderConfiguration.Default.WithContent($"I found {result.Entries.Count} definitions");
                    return Pages(new FieldBasedPageProvider(fieldBuilders, config));
                }
            }
        }

        [Command("rw", "word")]
        public async Task<DiscordCommandResult> GetRandomWordAsync()
        {
            var result = await _searchService.GetRandomWordResponseAsync();
            
            var le = new LocalEmbed()
                .WithTitle(result.Word)
                .WithDescription(result.Definition)
                .WithLexColor()
                .AddField("Pronunciation", result.Pronunciation);

            return Response(le);
        }

        [Command("choose", "choice")]
        [Description("Receive a choice")]
        public DiscordCommandResult ChoiceAsync([Description("The string of choices"), Remainder] string choice)
        {
            var choices = choice.Split("|");

            return Response(choices.Length < 2 ? "I require more options." : $"I choose {choices.Random()}");
        }

        [Command("fig", "figlet")]
        [Description("Create some text in a FIGLet font")]
        public DiscordCommandResult FigletFontAsync([Description("The text to render"), Remainder] string text)
        {
            return Response(text.Length >= 25 ? "Your text to render can not be longer than 2000 characters" : _figletService.GetRenderedText(text));
        }
    }
}