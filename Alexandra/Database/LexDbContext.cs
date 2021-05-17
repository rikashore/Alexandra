using Alexandra.Common.Extensions;
using Alexandra.Database.Entities;
using Disqord;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Alexandra.Database
{
    public class LexDbContext : DbContext
    {
        public DbSet<Note> Notes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        
        public LexDbContext(DbContextOptions<LexDbContext> options) : base(options) 
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var snowflakeConverter = new ValueConverter<Snowflake, ulong>(
                static snowflake => snowflake,
                static @ulong => new Snowflake(@ulong));

            modelBuilder.UseValueConverterForType<Snowflake>(snowflakeConverter);

            modelBuilder.Entity<Tag>().HasKey("Name", "GuildId");
        }
    }
}