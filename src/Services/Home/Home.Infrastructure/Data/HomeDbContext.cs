using Home.Domain.Identity;
using Home.Infrastructure.Data.Mappings;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Home.Infrastructure.Data;

public class HomeDbContext : IdentityDbContext<User, Role, long, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
{
    public HomeDbContext(DbContextOptions<HomeDbContext> options) : base(options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        if (optionsBuilder.IsConfigured) return;
        optionsBuilder.UseSqlServer(@"Data Source=.\\GENSHIN;Initial Catalog=Augua;User Id=sa;Password=sa;Integrated Security=False;MultipleActiveResultSets=true");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Identity");

        modelBuilder.ApplyConfiguration(new AuthApplicationMapping());
        modelBuilder.ApplyConfiguration(new AuthAuthorizationMapping());
        modelBuilder.ApplyConfiguration(new AuthScopeMapping());
        modelBuilder.ApplyConfiguration(new AuthTokenMapping());
        modelBuilder.ApplyConfiguration(new RoleClaimsMapping());
        modelBuilder.ApplyConfiguration(new RolesMapping());
        modelBuilder.ApplyConfiguration(new UsersMapping());
        modelBuilder.ApplyConfiguration(new UserClaimsMapping());
        modelBuilder.ApplyConfiguration(new UserLoginsMapping());
        modelBuilder.ApplyConfiguration(new UserRolesMapping());
        modelBuilder.ApplyConfiguration(new UserTokensMapping());
    }

    #region Other DbSets
    public virtual DbSet<AuthApplication> AuthApplications { get; set; }
    public virtual DbSet<AuthAuthorization> AuthAuthorizations { get; set; }
    public virtual DbSet<AuthScope> AuthScopes { get; set; }
    public virtual DbSet<AuthToken> AuthTokens { get; set; }
    #endregion
}