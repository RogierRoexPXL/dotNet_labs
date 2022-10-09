using HumanResources.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HumanResources.Infrastructure
{
    internal class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            //Number is the primary key and has a maximum of 11 characters
            builder.HasKey(e => e.Number);
            builder.Property(e => e.Number).HasMaxLength(11)
                .HasConversion(n => n.ToString(), s => new EmployeeNumber(s));
            //FirstName and LastName are required
            builder.Property(e => e.FirstName).IsRequired();
            builder.Property(e => e.LastName).IsRequired();
        }
    }
}
