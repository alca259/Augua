using Home.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Home.Infrastructure.Data.Mappings;

internal class AuthTokenMapping : IEntityTypeConfiguration<AuthToken>
{
    public void Configure(EntityTypeBuilder<AuthToken> builder)
    {
        builder.HasKey(pk => pk.Id);
        builder.ToTable("AuthToken");

        builder.Property(p => p.Id).HasColumnName("Id").UseIdentityColumn();
        builder.Property(p => p.FkApplicationID).HasColumnName("FkApplicationId").IsRequired();
        builder.Property(p => p.FkAuthorizationID).HasColumnName("FkAuthorizationId").IsRequired();
        builder.Property(p => p.ConcurrencyToken).HasColumnName("ConcurrencyToken").HasMaxLength(50).IsUnicode().IsConcurrencyToken();
        builder.Property(p => p.CreationDate).HasColumnName("CreationDate");
        builder.Property(p => p.ExpirationDate).HasColumnName("ExpirationDate");
        builder.Property(p => p.Payload).HasColumnName("Payload").IsUnicode();
        builder.Property(p => p.Properties).HasColumnName("Properties").IsUnicode();
        builder.Property(p => p.RedemptionDate).HasColumnName("RedemptionDate");
        builder.Property(p => p.ReferenceId).HasColumnName("ReferenceId").HasMaxLength(100).IsUnicode();
        builder.Property(p => p.Status).HasColumnName("Status").HasMaxLength(50).IsUnicode();
        builder.Property(p => p.Subject).HasColumnName("Subject").HasMaxLength(400).IsUnicode();
        builder.Property(p => p.Type).HasColumnName("Type").HasMaxLength(50).IsUnicode();

        builder.HasOne(fk => fk.Application).WithMany(m => m.Tokens).HasForeignKey(fk => fk.FkApplicationID).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(fk => fk.Authorization).WithMany(m => m.Tokens).HasForeignKey(fk => fk.FkAuthorizationID).OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(i => i.ReferenceId).HasDatabaseName("IX_AuthToken_ReferenceId").IsUnique(true);
        builder.HasIndex(i => i.FkAuthorizationID).HasDatabaseName("IX_AuthToken_AuthorizationId").IsUnique(false);
        builder.HasIndex(i => new { i.FkApplicationID, i.Status, i.Subject, i.Type }).HasDatabaseName("IX_AuthToken_ApplicationId_Status_Subject_Type").IsUnique(false);
    }
}
