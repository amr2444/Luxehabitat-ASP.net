using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public sealed class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("Documents");
        builder.ConfigureAuditableEntity();
        builder.Property(x => x.FileName).HasMaxLength(255).IsRequired();
        builder.Property(x => x.StoragePath).HasMaxLength(500).IsRequired();
        builder.Property(x => x.ContentType).HasMaxLength(150).IsRequired();
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.PropertyId);
        builder.HasIndex(x => x.RentalApplicationId);
        builder.HasOne(x => x.User)
            .WithMany(x => x.Documents)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(x => x.Property)
            .WithMany(x => x.Documents)
            .HasForeignKey(x => x.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.RentalApplication)
            .WithMany(x => x.Documents)
            .HasForeignKey(x => x.RentalApplicationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
