﻿using Home.Domain.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Home.API.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public sealed class UserInfoController : Controller
{
    private readonly UserManager<User> _userManager;

    public UserInfoController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    [Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
    [HttpGet("/connect/userinfo"), HttpPost("/connect/userinfo"), Produces("application/json")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> UserInfo()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidToken,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                        "The specified access token is bound to an account that no longer exists."
                }));
        }

        var claims = new Dictionary<string, object>(StringComparer.Ordinal)
        {
            [Claims.Subject] = await _userManager.GetUserIdAsync(user)
        };

        // Por cada scope configurado y solicitado, devolvemos la información correspondiente

        if (User.HasScope(Scopes.Email))
        {
            claims[Claims.Email] = await _userManager.GetEmailAsync(user);
            claims[Claims.EmailVerified] = await _userManager.IsEmailConfirmedAsync(user);
        }

        if (User.HasScope(Scopes.Phone))
        {
            claims[Claims.PhoneNumber] = await _userManager.GetPhoneNumberAsync(user);
            claims[Claims.PhoneNumberVerified] = await _userManager.IsPhoneNumberConfirmedAsync(user);
        }

        if (User.HasScope(Scopes.Roles))
        {
            claims[Claims.Role] = await _userManager.GetRolesAsync(user);
        }

        return Ok(claims);
    }
}
