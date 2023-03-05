using Microsoft.AspNetCore.Identity;

namespace Home.Domain.Identity;

public class Role : IdentityRole<long>
{
    public virtual List<UserRole> FkUserRoles { get; set; }
}
