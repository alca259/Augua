using Home.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Home.Infrastructure.Data.Mappings;

internal class AuthAuthorizationMapping : IEntityTypeConfiguration<AuthAuthorization>
{
    public void Configure(EntityTypeBuilder<AuthAuthorization> builder)
    {
        builder.HasKey(pk => pk.Id);
        builder.ToTable("AuthAuthorization");

        builder.Property(p => p.Id).HasColumnName("Id").UseIdentityColumn();
        builder.Property(p => p.FkApplicationID).HasColumnName("FkApplicationId").IsRequired();
        builder.Property(p => p.ConcurrencyToken).HasColumnName("ConcurrencyToken").HasMaxLength(50).IsUnicode().IsConcurrencyToken();
        builder.Property(p => p.CreationDate).HasColumnName("CreationDate");
        builder.Property(p => p.Properties).HasColumnName("Properties").IsUnicode();
        builder.Property(p => p.Scopes).HasColumnName("Scopes").IsUnicode();
        builder.Property(p => p.Status).HasColumnName("Status").HasMaxLength(50).IsUnicode();
        builder.Property(p => p.Subject).HasColumnName("Subject").HasMaxLength(400).IsUnicode();
        builder.Property(p => p.Type).HasColumnName("Type").HasMaxLength(50).IsUnicode();

        builder.HasOne(fk => fk.Application).WithMany(m => m.Authorizations).HasForeignKey(fk => fk.FkApplicationID).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(i => new { i.FkApplicationID, i.Status, i.Subject, i.Type }).HasDatabaseName("IX_AuthAuthorization_ApplicationId_Status_Subject_Type").IsUnique(false);
    }
}
