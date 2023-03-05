using Home.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Home.Infrastructure.Data.Mappings;

internal class UserRolesMapping : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasKey(pk => new { pk.UserId, pk.RoleId });
        builder.ToTable("UserRole");

        builder.Property(p => p.UserId).HasColumnName("UserId").IsRequired();
        builder.Property(p => p.RoleId).HasColumnName("RoleId").IsRequired();

        builder.HasOne(ur => ur.FkUser).WithMany(u => u.FkUserRoles).HasForeignKey(ur => ur.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(ur => ur.FkRole).WithMany(r => r.FkUserRoles).HasForeignKey(ur => ur.RoleId).OnDelete(DeleteBehavior.Cascade);
    }
}
