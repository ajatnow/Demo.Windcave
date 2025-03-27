using Newtonsoft.Json;

namespace Demo.Commerce.Providers.Payment.Windcave.Api.Models;

/// <summary>
/// Container for properties common to both request and responses
/// </summary>
public class BaseSessionModel
{
    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    [JsonProperty("currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonProperty("merchantReference")]
    public string MerchantReference { get; set; } = string.Empty;

    [JsonProperty("callbackUrls")]
    public CallbackUrls CallbackInfo { get; set; } = new CallbackUrls();

    [JsonProperty("notificationUrl")]
    public string NotificationUrl { get; set; } = string.Empty;
}

public class CallbackUrls
{
    [JsonProperty("approved")]
    public string Approved { get; set; } = string.Empty;

    [JsonProperty("declined")]
    public string Declined { get; set; } = string.Empty;

    [JsonProperty("cancelled")]
    public string Cancelled { get; set; } = string.Empty;
}
