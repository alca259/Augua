using Microsoft.AspNetCore.Identity;

namespace Home.Domain.Identity;

public class UserClaim : IdentityUserClaim<string>
{
    public virtual User FkUser { get; set; }
}
