using System.Collections.Generic;
using System.Threading.Tasks;
using Alexandra.Common.Extensions;
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
        
        [Group("search")]
        public class SearchModule : DiscordModuleBase
        {
            private readonly GitHubClient _lexGithubClient;
            
            public SearchModule(GitHubClient lexGithubClient)
            {
                _lexGithubClient = lexGithubClient;
            }
            
            [Command("repo")]
            public async Task<DiscordCommandResult> GitSearchReposAsync([Name("Search Query"), Remainder] string searchQuery)
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
                            string description;

                            if (item.Description is null)
                                description = "";
                            else if (item.Description.Length < 1000)
                                description = item.Description;
                            else
                                description = "Description too long";
                            
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
            public async Task<DiscordCommandResult> GitSearchUsersAsync([Name("Search Query"), Remainder] string searchQuery)
            {
                var request = new SearchUsersRequest(searchQuery);

                var result = await _lexGithubClient.Search.SearchUsers(request);
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
                                .WithValue($"{item.Name}, {item.Bio ?? ""} ({Markdown.Link("Link", item.HtmlUrl)})"));
                        }

                        var config =
                            FieldBasedPageProviderConfiguration.Default.WithContent(
                                $"I have found {result.Items.Count} results");

                        return Pages(new FieldBasedPageProvider(fieldBuilders, config));
                    }
                }
            }
        }


        [Command("repo")]
        public async Task<DiscordCommandResult> GetRepoAsync([Name("Repository")] string repoName)
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
        public async Task<DiscordCommandResult> GetRepoAsync(
            [Name("Username")] string username, 
            [Name("Repository Name")] string repoName)
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

        [Command("user")]
        public async Task<DiscordCommandResult> GetUserAsync(string username)
        {
            try
            {
                var result = await _lexGithubClient.User.Get(username);
                
                var gitUsername = result.Name ?? result.HtmlUrl.Substring(result.HtmlUrl.LastIndexOf('/') + 1);

                var eb = new LocalEmbedBuilder()
                    .WithTitle(gitUsername)
                    .WithUrl(result.HtmlUrl)
                    .WithThumbnailUrl(result.AvatarUrl)
                    .WithDescription(result.Bio ?? "No Bio")
                    .WithLexColor()
                    .AddField("Followers", result.Followers.ToString(), true)
                    .AddField("Following", result.Following.ToString(), true)
                    .AddBlankField(true)
                    .AddField("Location", result.Location ?? "No Location")
                    .AddField("Account created at", result.CreatedAt.ToString());

                return Response(eb);
            }
            catch (NotFoundException)
            {
                return Response("It seems no user with that name could be found, perchance try again.");
            }
        }

        [Command("issue")]
        public async Task<DiscordCommandResult> GetIssueAsync(string ownerName, string name, int issueNumber)
        {
            try
            {
                var result = await _lexGithubClient.Issue.Get(ownerName, name, issueNumber);

                var resultText = result.Body.Length > 1024 ? result.Body[..1000] + "..." : result.Body;
                var userText = result.User.Name ??
                               result.User.HtmlUrl.Substring(result.User.HtmlUrl.LastIndexOf('/') + 1);

                var eb = new LocalEmbedBuilder()
                    .WithTitle(result.Title)
                    .WithUrl(result.HtmlUrl)
                    .WithDescription(resultText)
                    .WithLexColor()
                    .AddField("Created By", userText);

                return Response(eb);
            }
            catch (NotFoundException)
            {
                return Response("It seems that an issue could not be found");
            }
        }

        [Command("issue")]
        public async Task<DiscordCommandResult> GetIssueAsync(long repoId, int issueNumber)
        {
            try
            {
                var result = await _lexGithubClient.Issue.Get(repoId, issueNumber);

                var resultText = result.Body.Length > 1024 ? result.Body.Substring(0, 1000) + "..." : result.Body;
                var userText = result.User.Name ??
                               result.User.HtmlUrl.Substring(result.User.HtmlUrl.LastIndexOf('/') + 1);

                var eb = new LocalEmbedBuilder()
                    .WithTitle(result.Title)
                    .WithUrl(result.HtmlUrl)
                    .WithDescription(resultText)
                    .WithLexColor()
                    .AddField("Created By", userText)
                    .AddField(result.Repository.Name, Markdown.Link("GitHub", result.Repository.HtmlUrl));

                return Response(eb);
            }
            catch (NotFoundException)
            {
                return Response("It seems that an issue could not be found");
            }
        }
    }
}