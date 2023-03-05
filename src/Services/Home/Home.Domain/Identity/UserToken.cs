using Microsoft.AspNetCore.Identity;

namespace Home.Domain.Identity;

public class UserToken : IdentityUserToken<long>
{
    public virtual User FkUser { get; set; }
}
