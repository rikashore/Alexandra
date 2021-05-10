using Microsoft.EntityFrameworkCore;

namespace Alexandra.Database
{
    public class LexDbContext : DbContext
    {
        public LexDbContext(DbContextOptions<LexDbContext> options) : base(options) 
        { }
    }
}