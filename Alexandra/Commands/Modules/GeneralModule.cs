using System.Diagnostics;
using System.Threading.Tasks;
using Alexandra.Common.Globals;
using Disqord;
using Disqord.Bot;
using Disqord.Gateway;
using Disqord.Rest;
using Qmmands;

namespace Alexandra.Commands.Modules
{
    public class GeneralModule : DiscordModuleBase
    {
        [Command("ping")]
        [Description("Plays a quick game of Ping Pong.")]
        public async Task Ping()
        {
            var stopwatch = Stopwatch.StartNew();
            var msg = await Response("Pong: *loading* response time");
            stopwatch.Stop();

            await msg.ModifyAsync(x => x.Content = $"Pong: {stopwatch.ElapsedMilliseconds}ms response time");
        }

        [Command("info")]
        [Description("Get some info about Alexandra")]
        public DiscordCommandResult Info()
        {
            var authorString = Context.Bot.GetUser(LexGlobals.AuthorId).ToString();

            var embedBuilder = new LocalEmbedBuilder()
                .WithColor(LexGlobals.LexColor)
                .WithTitle(Context.Bot.CurrentUser.Name)
                .AddField("Author", authorString)
                .AddField("Source Code", Markdown.Link("GitHub", LexGlobals.LexRepo), true)
                .AddField("Octokit.net", Markdown.Link("GitHub", LexGlobals.OctokitRepo), true)
                .AddField("Library", Markdown.Link("Disqord " + Library.Version, Library.RepositoryUrl), true);

            return Response(embedBuilder);
        }
    }
}