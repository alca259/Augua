using OpenIddict.EntityFrameworkCore.Models;

namespace Home.Domain.Identity;

public class AuthApplication : OpenIddictEntityFrameworkCoreApplication<long, AuthAuthorization, AuthToken>
{
}
