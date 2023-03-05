using Home.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Home.Infrastructure.Data.Mappings;

internal class RolesMapping : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(pk => pk.Id);
        builder.ToTable("Role");

        builder.Property(p => p.Id).HasColumnName("Id").IsRequired().HasMaxLength(128);
        builder.Property(p => p.Name).HasColumnName("Name").IsRequired().HasMaxLength(256);
        builder.Property(p => p.NormalizedName).HasColumnName("NormalizedName").HasMaxLength(256);
        builder.Property(p => p.ConcurrencyStamp).HasColumnName("ConcurrencyStamp").IsConcurrencyToken().HasMaxLength(50);

        builder.HasIndex(ix => ix.NormalizedName).HasDatabaseName("IX_Roles_Name").IsUnique(true);
    }
}
