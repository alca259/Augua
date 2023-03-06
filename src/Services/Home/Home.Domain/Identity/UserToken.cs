using Microsoft.AspNetCore.Identity;

namespace Home.Domain.Identity;

public class UserToken : IdentityUserToken<string>
{
    public virtual User FkUser { get; set; }
}
