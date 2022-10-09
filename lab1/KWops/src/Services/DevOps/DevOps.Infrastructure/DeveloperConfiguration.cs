using DevOps.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevOps.Infrastructure
{
    public class DeveloperConfiguration : IEntityTypeConfiguration<Developer>
    {
        public void Configure(EntityTypeBuilder<Developer> builder)
        {
            //Id is the primary key and has a maximum of 11 characters
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Id).HasMaxLength(11);

            builder.Property(d => d.FirstName).IsRequired();
            builder.Property(d => d.LastName).IsRequired();

            //p.Value made for accessing value, is this wrong? 
            builder.Property(d => d.Rating)
                .HasConversion(p => p.Value, d => new Percentage(d));
        }
    }
}
