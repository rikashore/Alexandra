using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Alexandra.Commands.Bases;
using Alexandra.Common.Extensions;
using Alexandra.Common.Utilities;
using Alexandra.Services;
using Disqord;
using Disqord.Bot;
using Disqord.Rest;
using Octokit;
using Qmmands;

namespace Alexandra.Commands.Modules
{
    [Name("Search")]
    [Description("Search related commands")]
    [Group("search")]
    public class SearchModule : LexGuildModuleBase
    {
        private readonly GitHubClient _lexGithubClient;
        private readonly ColorService _colorService;
                    
        public SearchModule(GitHubClient lexGithubClient, ColorService colorService)
        {
            _lexGithubClient = lexGithubClient;
            _colorService = colorService;
        }
                    
        [Command("repo")]
        [Description("Search for repositories")]
        public async Task<DiscordCommandResult> GitSearchReposAsync([Name("Search Query"), Remainder] string searchQuery)
        {
            var request = new SearchRepositoriesRequest(searchQuery);

            var result = await _lexGithubClient.Search.SearchRepo(request);
            switch (result.Items.Count)
            {
                case 0:
                    return NoResultsFoundResponse();
                default:
                {
                    var fieldBuilders = new List<LocalEmbedFieldBuilder>(result.Items.Count);

                    foreach (var item in result.Items)
                    {
                        var description = GetRepoSearchResultDescription(item);

                        fieldBuilders.Add(new LocalEmbedFieldBuilder().WithName(item.Name)
                            .WithValue($"{description} {Markdown.Link("Link", item.HtmlUrl)}"));
                    }

                    var config =
                        FieldBasedPageProviderConfiguration.Default.WithContent(
                            $"I have found {result.Items.Count} results");

                    return Pages(new FieldBasedPageProvider(fieldBuilders, config));
                }
            }
        }

        [Command("user")]
        [Description("Search for users")]
        public async Task<DiscordCommandResult> GitSearchUsersAsync([Name("Search Query"), Remainder] string searchQuery)
        {
            var request = new SearchUsersRequest(searchQuery);

            var result = await _lexGithubClient.Search.SearchUsers(request);
            switch (result.Items.Count)
            {
                case 0:
                    return NoResultsFoundResponse();
                case <= 5:
                {
                    var eb = new LocalEmbedBuilder()
                        .WithTitle("Search Results")
                        .WithLexColor();
                    
                    foreach (var item in result.Items)
                        eb.AddField(item.Name, $"{GetUserSearchResultBio(item)} ({Markdown.Link("GitHub page", item.HtmlUrl)})");
                    

                    return Response(eb);
                }
                default:
                {
                    var fieldBuilders = new List<LocalEmbedFieldBuilder>(result.Items.Count);

                    foreach (var item in result.Items)
                    {
                        fieldBuilders.Add(new LocalEmbedFieldBuilder().WithName(item.Name)
                            .WithValue($"{item.Name}, {GetUserSearchResultBio(item)} ({Markdown.Link("GitHub page", item.HtmlUrl)})"));
                    }

                    var config =
                        FieldBasedPageProviderConfiguration.Default.WithContent(
                            $"I have found {result.Items.Count} results");

                    return Pages(new FieldBasedPageProvider(fieldBuilders, config));
                }
            }
        }

        [Command("color"), RunMode(RunMode.Parallel)]
        [Description("Search for a particular shade of your choosing\nUses The Color API")]
        public async Task SearchColorAsync(Color color)
        {
            var result = await _colorService.GetColorInfo("hex", color.ToString().Substring(1));
            var colorImagePath = _colorService.GetColorImage(color.ToString());
            using (var colorImage = new LocalAttachment(colorImagePath, "colorImage.png"))
            {
                var eb = new LocalEmbedBuilder()
                    .WithTitle(result.Name.Value)
                    .WithColor(color)
                    .WithImageUrl("attachment://colorImage.png");

                if (result.Name.ExactMatchName)
                    eb.AddField("Hex value", result.Hex.Value);
                else
                    eb.AddField("Closest Match Hex", result.Name.ClosestNamedHex);

                eb.AddField("RGB", result.Rgb.Value)
                    .AddField("HSL", result.Hsl.Value)
                    .AddField("HSV", result.Hsv.Value)
                    .AddField("CMYK", result.Cmyk.Value);

                var mb = new LocalMessageBuilder()
                    .WithAttachments(colorImage)
                    .WithEmbed(eb)
                    .Build();

                await Response(mb);
            }
            
            File.Delete(colorImagePath);
        }
        
        [Command("color"), RunMode(RunMode.Parallel)]
        [Description("Search for a particular shade of your choosing\nUses The Color API")]
        public async Task SearchColorAsync(
            [Description("The R component"), Range(0, 256)] int r, 
            [Description("The G component"), Range(0, 256)] int g, 
            [Description("The B component"), Range(0, 256)] int b)
        {
            var color = new Color(Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b));
            var result = await _colorService.GetColorInfo("hex", color.ToString().Substring(1));
            var colorImagePath = _colorService.GetColorImage(color.ToString());

            using (var colorImage = new LocalAttachment(colorImagePath, "colorImage.png"))
            {
                var eb = new LocalEmbedBuilder()
                    .WithTitle(result.Name.Value)
                    .WithColor(color)
                    .WithImageUrl("attachment://colorImage.png");

                if (result.Name.ExactMatchName)
                    eb.AddField("Hex value", result.Hex.Value);
                else
                    eb.AddField("Closest Match Hex", result.Name.ClosestNamedHex);

                eb.AddField("RGB", result.Rgb.Value)
                    .AddField("HSL", result.Hsl.Value)
                    .AddField("HSV", result.Hsv.Value)
                    .AddField("CMYK", result.Cmyk.Value);

                var mb = new LocalMessageBuilder()
                    .WithAttachments(colorImage)
                    .WithEmbed(eb)
                    .Build();
                
                await Response(mb);
            }
            
            File.Delete(colorImagePath);
        }

        private string GetRepoSearchResultDescription(Repository repository)
        {
            if (repository.Description is null)
                return "";
            
            return repository.Description.Length < 1000 ? repository.Description : "Description too long";
        }

        private string GetUserSearchResultBio(User user)
        {
            if (user.Bio is null)
                return "";
            
            return user.Bio.Length < 1000 ? user.Bio : "Description too long";
        }
    }
}