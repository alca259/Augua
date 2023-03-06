using Microsoft.AspNetCore.Identity;

namespace Home.Domain.Identity;

public class UserRole : IdentityUserRole<string>
{
    public virtual User FkUser { get; set; }
    public virtual Role FkRole { get; set; }
}
