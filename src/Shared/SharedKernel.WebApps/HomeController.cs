using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog.Context;
using System.Net;

namespace SharedKernel.WebApps;

public class HomeController : Controller
{
    private readonly IOptionsSnapshot<AppSettings> _settings;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IOptionsSnapshot<AppSettings> settings, ILogger<HomeController> logger)
    {
        _settings = settings;
        _logger = logger;
    }

    public IActionResult Configuration()
    {
        var app = (AppSettings)_settings.Value.Clone();

        var domain = Request.Scheme + "://" + Request.Host;

        app.ApiUrl = app.ApiUrl.Replace("{domain}", domain);
        app.CdnUrl = app.CdnUrl.Replace("{domain}", domain);
        app.IdentityUrl = app.IdentityUrl.Replace("{domain}", domain);

        return Ok(app);
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public IActionResult Log([FromBody] LogDto log)
    {
        using (LogContext.PushProperty("UserContext", log.User))
        using (LogContext.PushProperty("ApplicationContext", _settings.Value.ClientID))
        {
            switch (log.Level)
            {
                case LogLevel.Debug:
                    _logger.LogDebug("{Url} {Message} {ExtraInfo}", log.Url, log.Message, log.ExtraInfo);
                    break;
                case LogLevel.Info:
                    _logger.LogInformation("{Url} {Message} {ExtraInfo}", log.Url, log.Message, log.ExtraInfo);
                    break;
                case LogLevel.Warn:
                    _logger.LogWarning("{Url} {Message} {ExtraInfo}", log.Url, log.Message, log.ExtraInfo);
                    break;
                case LogLevel.Error:
                    _logger.LogError("{Url} {Message} {ExtraInfo}", log.Url, log.Message, log.ExtraInfo);
                    break;
                case LogLevel.Fatal:
                    _logger.LogCritical("{Url} {Message} {ExtraInfo}", log.Url, log.Message, log.ExtraInfo);
                    break;
            }
        }
        return Ok();
    }
}
