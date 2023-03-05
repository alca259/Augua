using OpenIddict.EntityFrameworkCore.Models;

namespace Home.Domain.Identity;

public class AuthAuthorization : OpenIddictEntityFrameworkCoreAuthorization<string, AuthApplication, AuthToken>
{
    public string FkApplicationID { get; set; }
}
