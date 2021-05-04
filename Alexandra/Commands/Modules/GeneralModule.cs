using System.Diagnostics;
using System.Threading.Tasks;
using Disqord.Bot;
using Disqord.Rest;
using Qmmands;

namespace Alexandra.Commands.Modules
{
    public class GeneralModule : DiscordModuleBase
    {
        [Command("ping")]
        [Description("Plays a quick game of Ping Pong.")]
        public async Task PingAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            var msg = await Response("Pong: *loading* response time");
            stopwatch.Stop();

            await msg.ModifyAsync(x => x.Content = $"Pong: {stopwatch.ElapsedMilliseconds}ms response time");
        }
    }
}