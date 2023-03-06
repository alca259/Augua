using Home.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Home.Infrastructure.Data.Mappings;

internal class AuthScopeMapping : IEntityTypeConfiguration<AuthScope>
{
    public void Configure(EntityTypeBuilder<AuthScope> builder)
    {
        builder.HasKey(pk => pk.Id);
        builder.ToTable("AuthScope");

        builder.Property(p => p.Id).HasColumnName("Id").UseIdentityColumn();
        builder.Property(p => p.ConcurrencyToken).HasColumnName("ConcurrencyToken").HasMaxLength(50).IsUnicode().IsConcurrencyToken();
        builder.Property(p => p.Description).HasColumnName("Description").IsUnicode();
        builder.Property(p => p.Descriptions).HasColumnName("Descriptions").IsUnicode();
        builder.Property(p => p.DisplayName).HasColumnName("DisplayName").IsUnicode();
        builder.Property(p => p.DisplayNames).HasColumnName("DisplayNames").IsUnicode();
        builder.Property(p => p.Name).HasColumnName("Name").HasMaxLength(200).IsUnicode();
        builder.Property(p => p.Properties).HasColumnName("Properties").IsUnicode();
        builder.Property(p => p.Resources).HasColumnName("Resources").IsUnicode();

        builder.HasIndex(i => i.Name).HasDatabaseName("IX_AuthScope_Name").IsUnique(true);
    }
}
