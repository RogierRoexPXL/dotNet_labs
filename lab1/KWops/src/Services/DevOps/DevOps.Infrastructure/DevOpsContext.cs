using DevOps.Domain;
using Microsoft.EntityFrameworkCore;

namespace DevOps.Infrastructure
{
    public class DevOpsContext : DbContext
    {
        public DevOpsContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Developer> Developers { get; set; }
        public DbSet<Team> Teams { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            new DeveloperConfiguration().Configure(builder.Entity<Developer>());
            new TeamConfiguration().Configure(builder.Entity<Team>());
        }

    }
}
