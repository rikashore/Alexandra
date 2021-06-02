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
    [Group("git")]
    [Name("GitHub")]
    [Description("Commands relating to GitHub")]
    public class GithubModule : LexGuildModuleBase
    {
        private readonly GitHubClient _lexGithubClient;

        public GithubModule(GitHubClient lexGithubClient)
        {
            _lexGithubClient = lexGithubClient;
        }
        
        [Command("repo")]
        [Description("Get a particular repository")]
        public async Task<DiscordCommandResult> GetRepoAsync([Name("Repository")] string repoName)
        {
            var details = repoName.Split("/");
            if (details.Length < 2)
                return Response("It seems like you haven't provided a suitable repo name.");

            try
            {
                var result = await _lexGithubClient.Repository.Get(details[0], details[1]);

                var eb = new LocalEmbed()
                    .WithTitle(result.Name)
                    .WithDescription(result.Description)
                    .WithLexColor()
                    .AddField("Language", result.Language)
                    .AddField("Source", Markdown.Link("Github", result.HtmlUrl));

                return Response(eb);
            }
            catch (NotFoundException)
            {
                return NotFoundResponse("repository");
            }

        }

        [Command("repo")]
        [Description("Get a particular repository")]
        public async Task<DiscordCommandResult> GetRepoAsync(
            [Name("Username"), Description("The owner of the repository")] string username, 
            [Name("Repository Name"), Description("The name of the repository")] string repoName)
        {
            try
            {
                var result = await _lexGithubClient.Repository.Get(username, repoName);

                var eb = new LocalEmbed()
                    .WithTitle(result.Name)
                    .WithDescription(result.Description)
                    .WithLexColor()
                    .AddField("Language", result.Language)
                    .AddField("Source", Markdown.Link("Github", result.HtmlUrl));

                return Response(eb);
            }
            catch (NotFoundException)
            {
                return NotFoundResponse("repository");
            }
        }

        [Command("user")]
        [Description("Get a particular user")]
        public async Task<DiscordCommandResult> GetUserAsync([Description("The username of the user")] string username)
        {
            try
            {
                var result = await _lexGithubClient.User.Get(username);

                var eb = new LocalEmbed()
                    .WithTitle(result.Name ?? result.Login)
                    .WithUrl(result.HtmlUrl)
                    .WithThumbnailUrl(result.AvatarUrl)
                    .WithDescription(result.Bio ?? "No Bio")
                    .WithLexColor()
                    .AddField("Followers", result.Followers, true)
                    .AddField("Following", result.Following, true)
                    .AddField("Repositories", result.PublicRepos, true)
                    .AddField("Location", result.Location ?? "No Location")
                    .AddField("Account created at", result.CreatedAt.ToString());

                return Response(eb);
            }
            catch (NotFoundException)
            {
                return NotFoundResponse("user");
            }
        }

        [Command("issue")]
        [Description("Get a particular issue")]
        public async Task<DiscordCommandResult> GetIssueAsync(
            [Name("Owner Name"), Description("The owner of the repository where the issue is")] string ownerName, 
            [Description("The name of the repository")] string name, 
            [Description("The issue number")] int issueNumber)
        {
            try
            {
                var result = await _lexGithubClient.Issue.Get(ownerName, name, issueNumber);

                var resultText = result.Body.Length > 1024 ? result.Body[..1000] + "..." : result.Body;
                var userText = result.User.Name ?? result.User.Login;

                var eb = new LocalEmbed()
                    .WithTitle(result.Title)
                    .WithUrl(result.HtmlUrl)
                    .WithDescription(resultText)
                    .WithLexColor()
                    .AddField("Created By", userText);

                return Response(eb);
            }
            catch (NotFoundException)
            {
                return NotFoundResponse("issue");
            }
        }

        [Command("issue")]
        [Description("Get a particular issue")]
        public async Task<DiscordCommandResult> GetIssueAsync(
            [Name("Repo Id"), Description("The Id of the repository where the issue is")] long repoId, 
            [Name("Issue Number"), Description("The issue number")] int issueNumber)
        {
            try
            {
                var result = await _lexGithubClient.Issue.Get(repoId, issueNumber);

                var resultText = result.Body.Length > 1024 ? result.Body[..1000] + "..." : result.Body;
                var userText = result.User.Name ?? result.User.Login;

                var eb = new LocalEmbed()
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
                return NotFoundResponse("issue");
            }
        }
    }
}