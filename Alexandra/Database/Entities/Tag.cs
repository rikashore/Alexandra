using System.ComponentModel.DataAnnotations;
using Disqord;

namespace Alexandra.Database.Entities
{
    public class Tag
    {
        public string Name { get; set; }
        public Snowflake GuildId { get; set; }
        public Snowflake OwnerId { get; set; }
        public string Content { get; set; }
        public uint Revisions { get; set; }
        public uint Uses { get; set; }
    }
}