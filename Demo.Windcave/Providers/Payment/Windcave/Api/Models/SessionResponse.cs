using Newtonsoft.Json;

namespace Demo.Commerce.Providers.Payment.Windcave.Api.Models;

/// <summary>
/// Container for properties that are returned by a session create or query.
/// Not all possible properties are defined here
/// </summary>
public class SessionResponse : BaseSessionModel
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("state")]
    public string State { get; set; } = string.Empty;

    [JsonProperty("links")]
    public List<Link> Links { get; set; } = [];

    [JsonProperty("customerId")]
    public string CustomerId { get; set; } = string.Empty;


    [JsonProperty("transactions")]
    public List<Transaction>? Transactions { get; set; }

    // derived properties
    public string? RedirectUrl => Links?.FirstOrDefault(l => l.Rel.Equals("hpp", StringComparison.OrdinalIgnoreCase))?.Href;
    public string? QueryUrl => Links?.FirstOrDefault(l => l.Rel.Equals("self", StringComparison.OrdinalIgnoreCase))?.Href;

    public bool HasTransactions => Transactions?.Any() ?? false;
    public Transaction? FirstTransaction => Transactions?.FirstOrDefault();
}
public class Link
{
    [JsonProperty("href")]
    public string Href { get; set; } = string.Empty;

    [JsonProperty("rel")]
    public string Rel { get; set; } = string.Empty;

    [JsonProperty("method")]
    public string Method { get; set; } = string.Empty;
}

public class Transaction
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("username")]
    public string Username { get; set; } = string.Empty;

    [JsonProperty("authorised")]
    public bool Authorised { get; set; }

    [JsonProperty("allowRetry")]
    public bool AllowRetry { get; set; }

    [JsonProperty("reCo")]
    public string ReCo { get; set; } = string.Empty;

    [JsonProperty("responseText")]
    public string ResponseText { get; set; } = string.Empty;

    [JsonProperty("authCode")]
    public string AuthCode { get; set; } = string.Empty;

    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("method")]
    public string Method { get; set; } = string.Empty;

    [JsonProperty("localTimeZone")]
    public string LocalTimeZone { get; set; } = string.Empty;

    [JsonProperty("dateTimeUtc")]
    public DateTime DateTimeUtc { get; set; }

    [JsonProperty("dateTimeLocal")]
    public DateTime DateTimeLocal { get; set; }

    [JsonProperty("settlementDate")]
    public string SettlementDate { get; set; } = string.Empty;

    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    [JsonProperty("balanceAmount")]
    public decimal BalanceAmount { get; set; }

    [JsonProperty("currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonProperty("currencyNumeric")]
    public int CurrencyNumeric { get; set; }

    [JsonProperty("clientType")]
    public string ClientType { get; set; } = string.Empty;

    [JsonProperty("merchantReference")]
    public string MerchantReference { get; set; } = string.Empty;

    [JsonProperty("sessionId")]
    public string SessionId { get; set; } = string.Empty;

    [JsonProperty("amountSurcharge")]
    public decimal AmountSurcharge { get; set; }

    [JsonProperty("isSurcharge")]
    public bool IsSurcharge { get; set; }

    [JsonProperty("card")]
    public Card? Card { get; set; }

    [JsonProperty("links")]
    public List<Link>? Links { get; set; }
}

public class Card
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("cardHolderName")]
    public string CardHolderName { get; set; } = string.Empty;

    [JsonProperty("cardNumber")]
    public string CardNumber { get; set; } = string.Empty;

    [JsonProperty("dateExpiryMonth")]
    public int DateExpiryMonth { get; set; }

    [JsonProperty("dateExpiryYear")]
    public int DateExpiryYear { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("issuerCountryCode")]
    public string IssuerCountryCode { get; set; } = string.Empty;

    [JsonProperty("mac")]
    public string Mac { get; set; } = string.Empty;
}