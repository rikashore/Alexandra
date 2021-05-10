using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Alexandra.Database
{
    public class LexDbContextFactory : IDesignTimeDbContextFactory<LexDbContext>
    {
        public LexDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<LexDbContext>();
            optionsBuilder.UseMySql(
                "",
                new MySqlServerVersion(new Version(8, 0, 21)));

            return new LexDbContext(optionsBuilder.Options);
        }
    }
}