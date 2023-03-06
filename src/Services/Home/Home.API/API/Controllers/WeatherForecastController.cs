using Home.API.API.DTOs;
using Microsoft.AspNetCore.Authorization;
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

    public WeatherForecastController(
        IOpenIddictApplicationManager applicationManager)
    {
        _applicationManager = applicationManager;
    }

    private static readonly string[] Summaries = new[]
    {
        "Congelante", "Vigorizante", "Fr�o", "Bueno", "Suave", "C�lido", "Apacible", "Caliente", "Sofocante", "Abrasador"
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
    public async Task<IActionResult> GetMessage()
    {
        var sub = User.FindFirstValue(Claims.Subject);
        if (string.IsNullOrEmpty(sub))
            return BadRequest();

        var test = await _applicationManager.ListAsync().ToListAsync();

        var client = await _applicationManager.FindByClientIdAsync(sub);
        if (client == null)
            return BadRequest();

        return Content($"{await _applicationManager.GetDisplayNameAsync(client)} has been successfully authenticated.");
    }
}