using Home.API.Services;
using Home.API.ViewModel;
using Home.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;

namespace Home.API.Controllers;

[Route("account")]
[ApiExplorerSettings(IgnoreApi = true)]
public sealed class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly ILoginService<User> _loginService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<User> userManager,
        ILoginService<User> loginService,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _loginService = loginService;
        _logger = logger;
    }

    [HttpGet("login")]
    public async Task<IActionResult> Login(string returnUrl = "")
    {
        var vm = new LoginViewModel
        {
            ReturnUrl = returnUrl,
#if DEBUG
            Userlogin = "administrator",
            Password = "demo"
#endif
        };

        var hasReturnUrl = Request.QueryString.Value?.Contains("ReturnUrl") ?? false;

        if (User.IsAuthenticated() && User is not WindowsPrincipal && !hasReturnUrl)
        {
            var userApp = await _loginService.FindByUserId(User.GetSubjectId());
            if (userApp != null)
            {
                return await RedirectSuccessLogin(vm.ReturnUrl, vm.RememberLogin, userApp, false);
            }
        }

        ViewData["ReturnUrl"] = vm.ReturnUrl;
        return View(vm);
    }

    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        ViewData["ReturnUrl"] = vm.ReturnUrl;
        if (!ModelState.IsValid) return View(vm);

        vm.Userlogin = vm.Userlogin.Trim();
        var userApp = await _loginService.FindByUsername(vm.Userlogin);

        if (userApp != null && userApp.LockoutEnabled && userApp.LockoutEnd != null && userApp.LockoutEnd > DateTimeOffset.UtcNow)
        {
            ModelState.AddModelError(string.Empty, "Cuenta bloqueada temporalmente.");
            _logger.LogDebug("User: {User} AccountLockoutTemporal", vm.Userlogin);
        }
        else if (userApp != null && await _loginService.ValidateCredentials(userApp, vm.Password))
        {
            return await RedirectSuccessLogin(vm.ReturnUrl, vm.RememberLogin, userApp);
        }
        else
        {
            if (userApp != null)
                await _userManager.AccessFailedAsync(userApp);

            ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrecta.");
        }

        return View(vm);
    }

    [HttpGet("logout")]
    public async Task<IActionResult> LogOff()
    {
        await _loginService.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogOffPost()
    {
        await _loginService.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    private async Task<IActionResult> RedirectSuccessLogin(string returnUrl, bool rememberLogin, User userApp, bool signIn = true)
    {
        _logger.LogDebug("User: {User} RedirectSuccessLogin", userApp.UserName);

        if (signIn)
            await _loginService.SignInAsync(userApp, returnUrl, rememberLogin);

        return RedirectToHomepage(returnUrl);
    }

    private string GetRedirectUrlToHomepage(string returnUrl = null)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return returnUrl;
        }

        returnUrl = "~/";
        return returnUrl;
    }

    private IActionResult RedirectToHomepage(string returnUrl = null)
    {
        return Redirect(GetRedirectUrlToHomepage(returnUrl));
    }
}
