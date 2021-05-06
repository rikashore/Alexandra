using System.Collections.Generic;
using System.Threading.Tasks;
using Alexandra.Common.Extensions;
using Alexandra.Common.Globals;
using Alexandra.Common.Utilities;
using Disqord;
using Disqord.Bot;
using Octokit;
using Qmmands;

namespace Alexandra.Commands.Modules
{
    [Group("git")]
    public class GithubModule : DiscordModuleBase
    {
        private readonly GitHubClient _lexGithubClient;

        public GithubModule(GitHubClient lexGithubClient)
        {
            _lexGithubClient = lexGithubClient;
        }
        
        [Command("search")]
        public async Task<DiscordCommandResult> GitSearchRepos([Remainder] string searchQuery = null)
        {
            var request = new SearchRepositoriesRequest(searchQuery);

            var result = await _lexGithubClient.Search.SearchRepo(request);
            switch (result.Items.Count)
            {
                case 0:
                    return Response("It seems no results have been found.");
                default:
                {
                    var fieldBuilders = new List<LocalEmbedFieldBuilder>(result.Items.Count);

                    foreach (var item in result.Items)
                    {
                        fieldBuilders.Add(new LocalEmbedFieldBuilder().WithName(item.Name)
                            .WithValue($"{item.Owner.Name} {Markdown.Link("Link", item.HtmlUrl)}"));
                    }

                    var config =
                        FieldBasedPageProviderConfiguration.Default.WithContent(
                            $"I have found {result.Items.Count} results");

                    return Pages(new FieldBasedPageProvider(fieldBuilders, config));
                }
            }
        }

        [Command("repo")]
        public async Task<DiscordCommandResult> GetRepo(string repoName)
        {
            var details = repoName.Split("/");
            if (details.Length < 2)
                return Response("It seems like you haven't provided a suitable repo name.");

            try
            {
                var result = await _lexGithubClient.Repository.Get(details[0], details[1]);

                var eb = new LocalEmbedBuilder()
                    .WithTitle(result.Name)
                    .WithDescription(result.Description)
                    .WithLexColor()
                    .AddField("Language", result.Language)
                    .AddField("Source", Markdown.Link("Github", result.HtmlUrl));

                return Response(eb);
            }
            catch (NotFoundException)
            {
                return Response("It seems, a repository with that name could not be found, perchance try again.");
            }

        }

        [Command("repo")]
        public async Task<DiscordCommandResult> GetRepo(string username, string repoName)
        {
            try
            {
                var result = await _lexGithubClient.Repository.Get(username, repoName);

                var eb = new LocalEmbedBuilder()
                    .WithTitle(result.Name)
                    .WithDescription(result.Description)
                    .WithLexColor()
                    .AddField("Language", result.Language)
                    .AddField("Source", Markdown.Link("Github", result.HtmlUrl));

                return Response(eb);
            }
            catch (NotFoundException)
            {
                return Response("It seems, a repository with that name could not be found, perchance try again.");
            }
        }
    }
}