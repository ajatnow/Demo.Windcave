using Newtonsoft.Json;
using System.Text;

namespace Demo.Commerce.Providers.Payment.Windcave.Api.Models;
public class ErrorResponse
{
    [JsonProperty("requestId")]
    public string RequestId { get; set; } = string.Empty;

    [JsonProperty("timestampUtc")]
    public DateTime TimestampUtc { get; set; }

    [JsonProperty("errors")]
    public List<Error> Errors { get; set; } = [];

    public string ErrorMessage => GetErrorMessages();

    private string GetErrorMessages()
    {
        var output = new StringBuilder();
        Errors.ForEach(e => output.AppendLine(e.Target + ":" + e.Message + ","));
        return output.ToString();
    }
}

public class Error
{
    [JsonProperty("target")]
    public string Target { get; set; }= string.Empty;

    [JsonProperty("message")]
    public string Message { get; set; }=string.Empty;
}
