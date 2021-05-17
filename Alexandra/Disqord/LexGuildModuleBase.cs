using System;
using System.Linq;
using System.Threading.Tasks;
using Alexandra.Services;
using Disqord.Bot;
using Microsoft.Extensions.DependencyInjection;

namespace Alexandra.Disqord
{
    public class LexGuildModuleBase : DiscordGuildModuleBase
    {
        protected TagService TagService => Context.Services.GetRequiredService<TagService>();
        
        protected DiscordCommandResult NoResultsFoundResponse() 
            => Response("It seems not result has been found.");

        protected DiscordCommandResult NotFoundResponse(string item)
            => Response($"It seems, a/an {item} with that name could not be found, perchance try again.");

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