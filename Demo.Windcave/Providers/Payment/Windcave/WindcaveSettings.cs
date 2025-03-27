using Umbraco.Commerce.Core.PaymentProviders;

namespace Demo.Commerce.Providers.Payment.Windcave;
public class WindcaveSettings
{
    [PaymentProviderSetting(Name = "Continue URL",
                            Description = "The URL to continue to after this provider has done processing. eg: /continue/",
                            SortOrder = 100)]
    public string ContinueUrl { get; set; } = string.Empty;

    [PaymentProviderSetting(Name = "Cancel URL",
                            Description = "The URL to return to if the payment attempt is canceled. eg: /cancel/",
                            SortOrder = 200)]
    public string CancelUrl { get; set; } = string.Empty;

    [PaymentProviderSetting(Name = "Error URL",
                            Description = "The URL to return to if the payment attempt errors. eg: /error/",
                            SortOrder = 300)]
    public string ErrorUrl { get; set; } = string.Empty;
}
