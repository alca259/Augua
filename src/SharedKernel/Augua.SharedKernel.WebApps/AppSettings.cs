namespace Augua.SharedKernel.WebApps;

public class AppSettings : ICloneable
{
    public string IdentityUrl { get; set; }
    public string ApiUrl { get; set; }
    public string CdnUrl { get; set; }
    public string ClientID { get; set; }
    public int LevelLog { get; set; } = LogLevel.Warn;
    public string AppName { get; set; } = "Claroluz";

    public object Clone()
    {
        return new AppSettings
        {
            ApiUrl = ApiUrl,
            IdentityUrl = IdentityUrl,
            ClientID = ClientID,
            CdnUrl = CdnUrl,
            LevelLog = LevelLog,
            AppName = AppName
        };
    }
}
