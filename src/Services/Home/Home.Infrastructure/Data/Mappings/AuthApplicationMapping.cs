using Home.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Home.Infrastructure.Data.Mappings;

internal class AuthApplicationMapping : IEntityTypeConfiguration<AuthApplication>
{
    public void Configure(EntityTypeBuilder<AuthApplication> builder)
    {
        builder.HasKey(pk => pk.Id);
        builder.ToTable("AuthApplication");

        builder.Property(p => p.Id).HasColumnName("Id").UseIdentityColumn();
        builder.Property(p => p.ClientId).HasColumnName("ClientId").HasMaxLength(100).IsUnicode();
        builder.Property(p => p.ClientSecret).HasColumnName("ClientSecret").IsUnicode();
        builder.Property(p => p.ConcurrencyToken).HasColumnName("ConcurrencyToken").HasMaxLength(50).IsUnicode().IsConcurrencyToken();
        builder.Property(p => p.ConsentType).HasColumnName("ConsentType").HasMaxLength(50).IsUnicode();
        builder.Property(p => p.DisplayName).HasColumnName("DisplayName").IsUnicode();
        builder.Property(p => p.DisplayNames).HasColumnName("DisplayNames").IsUnicode();
        builder.Property(p => p.Permissions).HasColumnName("Permissions").IsUnicode();
        builder.Property(p => p.PostLogoutRedirectUris).HasColumnName("PostLogoutRedirectUris").IsUnicode();
        builder.Property(p => p.Properties).HasColumnName("Properties").IsUnicode();
        builder.Property(p => p.RedirectUris).HasColumnName("RedirectUris").IsUnicode();
        builder.Property(p => p.Requirements).HasColumnName("Requirements").IsUnicode();
        builder.Property(p => p.Type).HasColumnName("Type").HasMaxLength(50).IsUnicode();

        builder.HasIndex(i => i.ClientId).HasDatabaseName("IX_AuthApplication_ClientId").IsUnique(true);
    }
}
