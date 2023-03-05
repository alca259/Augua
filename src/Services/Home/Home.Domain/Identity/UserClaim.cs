using Microsoft.AspNetCore.Identity;

namespace Home.Domain.Identity;

public class UserClaim : IdentityUserClaim<long>
{
    public virtual User FkUser { get; set; }
}
