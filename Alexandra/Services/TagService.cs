using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alexandra.Database;
using Alexandra.Database.Entities;
using Disqord;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MinimumEditDistance;
using Serilog;

namespace Alexandra.Services
{
    public class TagService : LexService
    {
        private readonly LexDbContext _lexDbContext;

        public TagService(ILogger<TagService> logger,LexDbContext lexDbContext) : base(logger)
        {
            _lexDbContext = lexDbContext;
        }

        public async Task<Tag> RetrieveTagAsync(string tagName, Snowflake guildId)
        {
            var tag = await _lexDbContext.Tags
                .FirstOrDefaultAsync(x => x.GuildId == guildId && EF.Functions.ILike(x.Name, tagName));

            return tag;
        }

        public async Task<List<Tag>> RetrieveTagsAsync(Snowflake guildId)
        {
            return await _lexDbContext.Tags.Where(x => x.GuildId == guildId).ToListAsync();
        }

        public async Task<List<Tag>> SearchTagsAsync(Snowflake guildId, string searchQuery)
        {
            var tags = await RetrieveTagsAsync(guildId);
            return tags
                .Select(x => (Levenshtein.CalculateDistance(x.Name, searchQuery, 2), x))
                .Where(x => x.Item1 <= 5)
                .OrderBy(x => x.Item1)
                .Select(x => x.Item2)
                .ToList();
        }
        
        public async Task<int> GetTagRankAsync(Tag tag)
        {
            var tags = await RetrieveTagsAsync(tag.GuildId);
            return tags.OrderByDescending(x => x.Uses).TakeWhile(x => x.Name != tag.Name).Count() + 1;
        }

        public async Task AddTagAsync(Tag tag)
        {
            await _lexDbContext.Tags.AddAsync(tag);
            await _lexDbContext.SaveChangesAsync();
        }

        public async Task DeleteTagAsync(Tag tag)
        {
            _lexDbContext.Tags.Attach(tag);
            _lexDbContext.Tags.Remove(tag);
            await _lexDbContext.SaveChangesAsync();
        }

        public async Task EditTagAsync(Tag tag, string newContent)
        {
            tag.Content = newContent;
            tag.Revisions++;
            await UpdateTagAsync(tag);
        }

        public async Task UpdateTagAsync(Tag tag)
        {
            _lexDbContext.Tags.Update(tag);
            await _lexDbContext.SaveChangesAsync();
        }
    }
}