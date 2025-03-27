using Demo.Commerce.Providers.Payment.Windcave.Api.Models;
using Demo.Commerce.Utility.Web;
using Demo.Windcave.Repositories;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace Demo.Commerce.Providers.Payment.Windcave.Api;

public class WindcaveClient(IAppConfigurationRepository appConfigurationRepository,
                            ILogger<WindcavePaymentProvider> logger,
                            IWebContentClient webContentClient)
{
    private readonly string _moduleName = "Demo.Commerce.Providers.Payment.Windcave.Api";

    private readonly IAppConfigurationRepository _appConfigurationRepository = appConfigurationRepository;
    private readonly ILogger<WindcavePaymentProvider> _logger = logger;
    private readonly IWebContentClient _webContentClient = webContentClient;

    /// <summary>
    /// Creates a session at the Windcave end and provides a sessionid for later use
    /// </summary>
    /// <param name="requestDto"></param>
    /// <returns></returns>
    public async Task<SessionResponse> CreateSessionAsync(SessionRequest requestDto)
    {
        var response = new SessionResponse();

        var windcaveAPIsettings = _appConfigurationRepository.WindcaveApiSettings;

        if (windcaveAPIsettings is not null)
        {
            string url = new StringBuilder()
                            .Append(windcaveAPIsettings?.ApiUrl ?? "")
                            .Append("v1/sessions")
                            .ToString();
            try
            {
                var apiResponse = await _webContentClient.PostAsync<SessionResponse, SessionRequest>(url, requestDto, null, true, windcaveAPIsettings!.ApiUser, windcaveAPIsettings.ApiKey);

                if (apiResponse?.HttpStatusCode == 202 && apiResponse.Result != null) // note that we expect a 202 response for success
                {
                    var data = apiResponse.Result;
                    if (data != null)
                    {
                        response = data;
                    }
                }
                else if (apiResponse?.HttpStatusCode != 202)
                {
                    _logger.LogError("{ModuleName}: CreateSessionAsync: Failed to create windcave session : {Message}", _moduleName, apiResponse?.RawContent ?? "");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{Modulename}: CreateSessionAsync ({Message})", _moduleName, ex.Message);
            }
        }
        return response;
    }

    /// <summary>
    /// A query for information for the specified session id
    /// Session has completed when httpstatus of 200 is received
    /// Session is pending when httpstatus of 202 is received. Need to retry
    /// </summary>
    /// <param name="sessionId"></param>
    /// <returns></returns>
    public async Task<SessionResponse> QuerySessionAsync(string sessionId)
    {
        var response = new SessionResponse();

        var windcaveAPIsettings = _appConfigurationRepository.WindcaveApiSettings;

        if (windcaveAPIsettings is not null)
        {
            string url = new StringBuilder()
                            .Append(windcaveAPIsettings?.ApiUrl ?? "")
                            .Append("v1/sessions/")
                            .Append(sessionId)
                            .ToString();

            bool isPending = true; // initialise so that can retry
            int currentTry = 0;
            int maxTries = 10;
            try
            {
                while (isPending)
                {
                    // query windcave for status of session by id
                    var apiResponse = await _webContentClient.GetAsync<SessionResponse>(url, null, true, windcaveAPIsettings!.ApiUser, windcaveAPIsettings.ApiKey);

                    if (apiResponse?.HttpStatusCode == 200 && apiResponse.Result != null) // note that we expect a 200 response for success
                    {
                        isPending = false; // can exit now
                        var data = apiResponse.Result;
                        if (data != null)
                        {
                            response = data;
                        }
                    }
                    else if (apiResponse?.HttpStatusCode == 202) // this response means is still pending
                    {
                        // get the partial response whilst pending
                        var data = apiResponse.Result;
                        if (data != null)
                        {
                            response = data;
                        }

                        currentTry++;
                        _logger.LogInformation("{ModuleName}: QuerySessionAsync: Transaction is still pending for session:{sessionId}, Try # {currentTry} of {maxTries}", _moduleName, sessionId, currentTry, maxTries);
                        isPending = currentTry < maxTries;
                        await Task.Delay(5000); // wait 5 secs
                    }
                    else if (apiResponse?.HttpStatusCode != 200 && apiResponse?.HttpStatusCode != 202)
                    {
                        // get the error object to find out the reported cause of failure
                        var messages = apiResponse?.RawContent;

                        if (apiResponse?.RawContent != null)
                        {
                            var responseError = JsonConvert.DeserializeObject<ErrorResponse>(apiResponse.RawContent);
                            messages = responseError?.ErrorMessage ?? "";
                        }
                        _logger.LogError("{ModuleName}: QuerySessionAsync: Failed to get windcave transaction session : {Message}", _moduleName, messages);

                        isPending = false;
                    }
                } // end while
            }
            catch (Exception ex)
            {
                _logger.LogError("{Modulename}: QuerySessionAsync ({Message})", _moduleName, ex.Message);
            }
        }
        return response;
    }

}
