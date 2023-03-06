using OpenIddict.EntityFrameworkCore.Models;

namespace Home.Domain.Identity;

public class AuthToken : OpenIddictEntityFrameworkCoreToken<long, AuthApplication, AuthAuthorization>
{
    public long FkApplicationID { get; set; }
    public long FkAuthorizationID { get; set; }
}
