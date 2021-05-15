using System.Threading.Tasks;
using Alexandra.Database.Entities;
using Disqord;
using Microsoft.EntityFrameworkCore;

namespace Alexandra.Database.Helpers
{
    public class TagHelper
    {
        private readonly LexDbContext _lexDbContext;

        public TagHelper(LexDbContext lexDbContext)
        {
            _lexDbContext = lexDbContext;
        }

        public async Task<Tag> RetrieveTagAsync(string tagName, Snowflake guildId)
        {
            var tag = await _lexDbContext.Tags
                .FirstOrDefaultAsync(x => x.Name == tagName && x.GuildId == guildId);

            return tag;
        }

        public async Task AddTagAsync(string tagName, string content, Snowflake guildId, Snowflake ownerId)
        {
            _lexDbContext.Add(new Tag {Name = tagName, Content = content, GuildId = guildId, OwnerId = ownerId});

            await _lexDbContext.SaveChangesAsync();
        }

        public async Task DeleteTagAsync(Tag tag)
        {
            _lexDbContext.Tags.Remove(tag);
            await _lexDbContext.SaveChangesAsync();
        }

        public async Task EditTagAsync(Tag tag, string newContent)
        {
            tag.Content = newContent;
            await _lexDbContext.SaveChangesAsync();
        }
    }
}