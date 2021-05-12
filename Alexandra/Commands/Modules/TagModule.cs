using System.Threading.Tasks;
using Alexandra.Database.Helpers;
using Disqord.Bot;
using Qmmands;

namespace Alexandra.Commands.Modules
{
    [Group("tag", "tags")]
    [Name("Tag")]
    [Description("Commands relating to tags")]
    public class TagModule : DiscordGuildModuleBase
    {
        private readonly TagHelper _tagHelper;
        
        public TagModule(TagHelper tagHelper)
        {
            _tagHelper = tagHelper;
        }
        
        [Command]
        public async Task<DiscordCommandResult> GetTagAsync(string tagName)
        {
            var tag = await _tagHelper.RetrieveTagAsync(tagName, Context.GuildId);

            if (tag is null)
                return Response("It seems no tag with that name could be found.");

            return Response(tag.Content);
        }

        [Command("add")]
        public async Task<DiscordCommandResult> AddTagAsync(string tagName, [Remainder] string content)
        {
            var tag = await _tagHelper.RetrieveTagAsync(tagName, Context.GuildId);

            if (tag is not null)
                return Response("It seems a tag with this name already exists.");

            await _tagHelper.AddTagAsync(tagName, content, Context.GuildId, Context.Author.Id);
            return Response($"I have successfully added {tagName}.");
        }

        [Command("delete", "del", "remove")]
        public async Task<DiscordCommandResult> DeleteTagAsync(string tagName)
        {
            var tag = await _tagHelper.RetrieveTagAsync(tagName, Context.GuildId);

            if (tag is null)
                return Response("It seems no tag with that name could be found.");
            if (tag.OwnerId != Context.Author.Id)
                return Response("I cannot let you delete other's tags.");

            await _tagHelper.DeleteTagAsync(tag);
            return Response("I have successfully deleted that tag.");
        }

        [Command("edit")]
        public async Task<DiscordCommandResult> EditTagAsync(string tagName, [Remainder] string newContent)
        {
            var tag = await _tagHelper.RetrieveTagAsync(tagName, Context.GuildId);

            if (tag is null)
                return Response("It seems no tag with that name could be found.");
            if (tag.OwnerId != Context.Author.Id)
                return Response("I cannot let you edit other's tags.");

            await _tagHelper.EditTagAsync(tag, newContent);
            return Response("I have edited that tag successfully.");
        }
    }
}