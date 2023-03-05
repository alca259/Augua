using OpenIddict.EntityFrameworkCore.Models;

namespace Home.Domain.Identity;

public class AuthToken : OpenIddictEntityFrameworkCoreToken<string, AuthApplication, AuthAuthorization>
{
    public string FkApplicationID { get; set; }
    public string FkAuthorizationID { get; set; }
}
