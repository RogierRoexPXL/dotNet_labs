using HumanResources.Domain;
using Microsoft.EntityFrameworkCore;

namespace HumanResources.Infrastructure
{
    internal class HumanResourcesContext : DbContext
    {
        public HumanResourcesContext(DbContextOptions options) : base(options) 
        {

        }

        public DbSet<Employee>? Employees { get; set; } //vraagteken zelf toegevoegd, goed/slecht??

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            new EmployeeConfiguration().Configure(builder.Entity<Employee>());
        }

    }
}
