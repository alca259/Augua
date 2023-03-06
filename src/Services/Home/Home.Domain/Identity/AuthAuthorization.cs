using OpenIddict.EntityFrameworkCore.Models;

namespace Home.Domain.Identity;

public class AuthAuthorization : OpenIddictEntityFrameworkCoreAuthorization<long, AuthApplication, AuthToken>
{
    public long FkApplicationID { get; set; }
}
