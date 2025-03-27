namespace Demo.Windcave.Models.Configuration;

/// <summary>
/// Container for custom api settings for the windcave service from the appsettings.json file
/// </summary>
public class WindcaveApiSettings
{
    public const string WindcaveApi = "Demo:WindcaveApi";

    public string ApiUrl { get; set; } = string.Empty;
    public string ApiUser { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}
