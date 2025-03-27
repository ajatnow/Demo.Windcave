using UCommModels = Umbraco.Commerce.Core.Models;

namespace Demo.Web.Models;


public class PaymentViewModel
{

    public UCommModels.OrderReadOnly? Order { get; set; }

    public UCommModels.PaymentMethodReadOnly? PaymentMethod { get; set; }

    public string PaymentMethodLogoSrc { get; set; } = string.Empty;
    public string OrdersPageUrl { get; set; } = string.Empty;

    public bool HasPaymentMethod => this.Order?.PaymentInfo.PaymentMethodId is not null;
    public bool HasOrderLines => this.Order?.OrderLines.Count > 0;
}