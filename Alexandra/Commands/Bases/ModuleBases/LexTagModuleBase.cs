using System.Linq;
using System.Threading.Tasks;
using Alexandra.Services;
using Disqord.Bot;
using Microsoft.Extensions.DependencyInjection;

namespace Alexandra.Commands.Bases.ModuleBases
{
    public class LexTagModuleBase : LexGuildModuleBase
    {
        protected TagService TagService => Context.Services.GetRequiredService<TagService>();

        protected async Task<DiscordCommandResult> TagNotFoundResponse(string name)
        {
            var closeTags = await TagService.SearchTagsAsync(Context.GuildId, name);
            if (closeTags.Count == 0) 
                return Response($"I couldn't find a tag with the name \"{name}\".");
            
            var didYouMean = " • " + string.Join("\n • ", closeTags.Take(3).Select(x => x.Name));
                return Response($"It seems I couldn't find a tag with the name \"{name}\", did you mean...\n{didYouMean}");
        }
    }
}