using Microsoft.AspNetCore.Identity;

namespace Home.Domain.Identity;

public class Role : IdentityRole<string>
{
    public virtual List<UserRole> FkUserRoles { get; set; }
}
