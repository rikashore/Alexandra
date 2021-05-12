using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Alexandra.Database
{
    public class LexDbContextFactory : IDesignTimeDbContextFactory<LexDbContext>
    {
        public LexDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("./config.json")
                .Build();
            
            var optionsBuilder = new DbContextOptionsBuilder<LexDbContext>();
            optionsBuilder.UseMySql(
                config["dbconn"],
                new MySqlServerVersion(new Version(8, 0, 21)));

            return new LexDbContext(optionsBuilder.Options);
        }
    }
}