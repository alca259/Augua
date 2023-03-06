using OpenIddict.EntityFrameworkCore.Models;

namespace Home.Domain.Identity;

public class AuthToken : OpenIddictEntityFrameworkCoreToken<long, AuthApplication, AuthAuthorization>
{
    public long ApplicationId { get; set; }
    public long AuthorizationId { get; set; }
}
