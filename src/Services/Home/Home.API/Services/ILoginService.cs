using System.Security.Claims;

namespace Home.API.Services;

public interface ILoginService<T>
{
    Task<bool> ValidateCredentials(T user, string password);

    Task<T> FindByUserId(string userId);

    Task<T> FindByUsername(string user);

    Task SignIn(T user);

    Task SignInAsync(T user, string returnUrl, bool rememberLogin, IEnumerable<Claim> aditionalClaims = null);

    Task SignOutAsync();
}
