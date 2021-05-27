using System.Collections.Generic;
using System.Threading.Tasks;
using Alexandra.Commands.Bases;
using Alexandra.Common.Extensions;
using Alexandra.Common.Utilities;
using Disqord;
using Disqord.Bot;
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
                    
        public SearchModule(GitHubClient lexGithubClient)
        {
            _lexGithubClient = lexGithubClient;
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
                        eb.AddField(item.Name, $"{item.Bio ?? ""} ({Markdown.Link("GitHub page", item.HtmlUrl)})");
                    

                    return Response(eb);
                }
                default:
                {
                    var fieldBuilders = new List<LocalEmbedFieldBuilder>(result.Items.Count);

                    foreach (var item in result.Items)
                    {
                        fieldBuilders.Add(new LocalEmbedFieldBuilder().WithName(item.Name)
                            .WithValue($"{item.Name}, {item.Bio ?? ""} ({Markdown.Link("GitHub page", item.HtmlUrl)})"));
                    }

                    var config =
                        FieldBasedPageProviderConfiguration.Default.WithContent(
                            $"I have found {result.Items.Count} results");

                    return Pages(new FieldBasedPageProvider(fieldBuilders, config));
                }
            }
        }

        private string GetRepoSearchResultDescription(Repository repository)
        {
            if (repository.Description is null)
                return "";
            
            return repository.Description.Length < 1000 ? repository.Description : "Description too long";
        }
    }
}