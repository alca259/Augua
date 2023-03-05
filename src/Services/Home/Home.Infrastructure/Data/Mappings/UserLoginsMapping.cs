using Home.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Home.Infrastructure.Data.Mappings;

internal class UserLoginsMapping : IEntityTypeConfiguration<UserLogin>
{
    public void Configure(EntityTypeBuilder<UserLogin> builder)
    {
        builder.HasKey(pk => new { pk.LoginProvider, pk.ProviderKey, pk.UserId });
        builder.ToTable("UserLogin");

        builder.Property(p => p.LoginProvider).HasColumnName("LoginProvider").IsRequired().HasMaxLength(128);
        builder.Property(p => p.ProviderKey).HasColumnName("ProviderKey").IsRequired().HasMaxLength(128);
        builder.Property(p => p.UserId).HasColumnName("UserId").IsRequired();
        builder.Property(p => p.ProviderDisplayName).HasColumnName("ProviderDisplayName");
    }
}
