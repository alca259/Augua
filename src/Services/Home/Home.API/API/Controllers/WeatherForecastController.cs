using Home.API.API.DTOs;
using Home.Domain.Identity;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Home.API.API.Controllers;

[ApiController]
[Authorize]
[Route("weather")]
public class WeatherForecastController : ControllerBase
{
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly IOpenIddictAuthorizationManager _authorizationManager;
    private readonly UserManager<User> _userManager;

    public WeatherForecastController(
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictAuthorizationManager authorizationManager,
        UserManager<User> userManager)
    {
        _applicationManager = applicationManager;
        _authorizationManager = authorizationManager;
        _userManager = userManager;
    }

    private static readonly string[] Summaries = new[]
    {
        "Congelante", "Vigorizante", "Frío", "Bueno", "Suave", "Cálido", "Apacible", "Caliente", "Sofocante", "Abrasador"
    };

    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpGet("message")]
    public async Task<IActionResult> GetMessage(CancellationToken token = default)
    {
        var sub = User.FindFirstValue(Claims.Subject);
        if (string.IsNullOrEmpty(sub))
            return BadRequest();

        var authorizations = (await _authorizationManager.FindBySubjectAsync(sub, token).ToListAsync(token)).Cast<AuthAuthorization>().ToList();

        if (!authorizations.Any())
            return BadRequest();

        authorizations = authorizations.OrderByDescending(o => o.CreationDate).ToList();

        var application = await _applicationManager.FindByIdAsync(authorizations.First().ApplicationId.ToString(), token);
        if (application == null)
            return BadRequest();

        return Content($"{await _applicationManager.GetDisplayNameAsync(application, token)} has been successfully authenticated.");
    }
}