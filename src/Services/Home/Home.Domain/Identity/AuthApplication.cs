using OpenIddict.EntityFrameworkCore.Models;

namespace Home.Domain.Identity;

public class AuthApplication : OpenIddictEntityFrameworkCoreApplication<string, AuthAuthorization, AuthToken>
{
}
