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
        [Description("A game of ping-pong shall occur")]
        public async Task PingAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            var msg = await Response("Pong: *loading* response time");
            stopwatch.Stop();

            await msg.ModifyAsync(x => x.Content = $"Pong: {stopwatch.ElapsedMilliseconds}ms response time");
        }

        [Command("info")]
        [Description("Receive information about Alexandra")]
        public DiscordCommandResult InfoAsync()
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