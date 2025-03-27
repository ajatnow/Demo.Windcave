using Demo.Commerce.Providers.Payment.Windcave.Api;
using Demo.Commerce.Providers.Payment.Windcave.Api.Models;
using Demo.Commerce.Utility.Web;
using Demo.Windcave.Repositories;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Logging;
using System.Web;
using Umbraco.Commerce.Core.Api;
using Umbraco.Commerce.Core.PaymentProviders;
using Umbraco.Commerce.Extensions;

namespace Demo.Commerce.Providers.Payment.Windcave;

[EnableCors("PaymentProvidersPolicy")]
[PaymentProvider("windcave", "Windcave", "Windcave payment provider for payments")]
public class WindcavePaymentProvider(UmbracoCommerceContext umbracoCommerce,
                                     IAppConfigurationRepository appConfigurationRepository,
                                     ILogger<WindcavePaymentProvider> logger,
                                     //IPortalStoreService portalStoreService,
                                     IWebContentClient webContentClient
                                    ) : PaymentProviderBase<WindcaveSettings>(umbracoCommerce)
{
    private readonly string _moduleName = "Demo.Commerce.Providers.Payment.Windcave";

    private readonly IAppConfigurationRepository _appConfigurationRepository = appConfigurationRepository;
    private readonly ILogger<WindcavePaymentProvider> _logger = logger;
    //private readonly IPortalStoreService _portalStoreService = portalStoreService;
    private readonly IWebContentClient _webContentClient = webContentClient;

    public override bool CanFetchPaymentStatus => false;
    public override bool CanCancelPayments => false;
    public override bool CanRefundPayments => false;
    public override bool CanCapturePayments => false;
    public override bool FinalizeAtContinueUrl => true; // this is true as we will finalize the order using the callback

    public override IEnumerable<TransactionMetaDataDefinition> TransactionMetaDataDefinitions =>
        [
            new TransactionMetaDataDefinition("windcaveTransactionId", "Windcave Transaction Id"),
            new TransactionMetaDataDefinition("windcaveMethod", "Windcave Method"),
            new TransactionMetaDataDefinition("windcaveCardHolder", "Windcave CardHolder Name"),
            new TransactionMetaDataDefinition("windcaveCardNumber", "Windcave CardNumber")
        ];

    public override async Task<PaymentFormResult> GenerateFormAsync(PaymentProviderContext<WindcaveSettings> ctx, CancellationToken cancellationToken = default)
    {
        // could potentially get the currency for this order using a custom service like this
        // var currency = _portalStoreService.GetCurrencyByGuid(ctx.Order.CurrencyId);

        // populate a suitable request object for this order to be processed for payment
        SessionRequest requestDto = new()
        {
            Type = "purchase",
            Currency = "AUD", // if retrieved actual currency object earlier then use - currency?.Code ?? "AUD",
            Amount = ctx.Order.TransactionAmount.Value,
            MerchantReference = ctx.Order.Id.ToString()
        };

        requestDto.CallbackInfo.Approved = ctx.Urls.ContinueUrl; // FinalizeAtContinueUrl
        requestDto.CallbackInfo.Declined = ctx.Urls.ErrorUrl; 
        requestDto.CallbackInfo.Cancelled = ctx.Urls.CancelUrl;
        // requestDto.NotificationUrl = ctx.Urls.CallbackUrl; // using FinalizeAtContinueUrl instead

        var client = new WindcaveClient(_appConfigurationRepository, _logger, _webContentClient);

        // here go start the session which will return a url for redirection to WHPP site
        var sessionResponse = await client.CreateSessionAsync(requestDto).ConfigureAwait(false);

        return new PaymentFormResult()
        {
            MetaData = new Dictionary<string, string>
                {
                    { "windcaveSessionId", sessionResponse.Id },
                },
            Form = new PaymentForm(sessionResponse.RedirectUrl, PaymentFormMethod.Get)
        };
    }

    /// <summary>
    /// Hooked into the Approved url in the CreateSession request, will auto finalise this order on OK return
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<CallbackResult> ProcessCallbackAsync(PaymentProviderContext<WindcaveSettings> ctx, CancellationToken cancellationToken = default)
    {
        // setup first for badness
        CallbackResult returnResult = CallbackResult.BadRequest();
        returnResult.HttpResponse = new HttpResponseMessage(System.Net.HttpStatusCode.Found);
        returnResult.HttpResponse.Headers.Location = new Uri(ctx.Urls.ErrorUrl);

        _logger.LogInformation("{ModuleName}: ProcessCallbackAsync: Arrived at callback ", _moduleName);

        // see if there is no session id
        if (ctx.Request.RequestUri is null || !ctx.Request.RequestUri.Query.Contains("sessionId"))
        {
            _logger.LogWarning("{ModuleName}: ProcessCallbackAsync: No session id found in request", _moduleName);

            return returnResult;
        } // no sessionid

        var sessionId = HttpUtility.ParseQueryString(ctx.Request.RequestUri.PathAndQuery.Split('?')[1]).Get("sessionId");
        if (string.IsNullOrEmpty(sessionId))
        {
            _logger.LogWarning("{ModuleName}: ProcessCallbackAsync: No session id found in query", _moduleName);

            return returnResult;
        }// no sessionid value

        // go retrieve the result of the last session
        var client = new WindcaveClient(_appConfigurationRepository, _logger, _webContentClient);
        var sessionResponse = await client.QuerySessionAsync(sessionId).ConfigureAwait(false);

        if (sessionResponse is null || string.IsNullOrEmpty(sessionResponse.Id))
        {
            _logger.LogWarning("{ModuleName}: ProcessCallbackAsync: QuerySession returned null or no id", _moduleName);

            return returnResult;
        } // has response

        if (!sessionResponse.HasTransactions || sessionResponse.FirstTransaction!.Authorised == false)
        {
            _logger.LogWarning("{ModuleName}: ProcessCallbackAsync: QuerySession not authorised or no transactions", _moduleName);

            return returnResult;
        } // response session is valid

        // at this point everything should be alright so create an OK callbackresult
        var firstTrans = sessionResponse.FirstTransaction;

        returnResult = CallbackResult.Ok(
            new TransactionInfo
            {
                TransactionId = firstTrans.Id,
                AmountAuthorized = firstTrans.Amount,
                PaymentStatus = Umbraco.Commerce.Core.Models.PaymentStatus.Authorized
            },
            new Dictionary<string, string>
            {
                { "windcaveTransactionId", firstTrans.Id },
                { "windcaveMethod", firstTrans.Method },
                { "windcaveCardHolder",  firstTrans.Card?.CardHolderName ?? "" },
                { "windcaveCardNumber",  firstTrans.Card?.CardNumber ?? "" },
            }
        );

        _logger.LogInformation("{ModuleName}: ProcessCallbackAsync: Completed callback for {merchRef}", _moduleName, firstTrans.MerchantReference );

        // returning an ok result will cause the finalization of this order 
        // that in turn will cause the OrderFinalizedNotificationHandler to be called which will set the order to the required status
        // and then the ContinueUrl method to be called
        return returnResult;
    }

    public override string GetCancelUrl(PaymentProviderContext<WindcaveSettings> context)
    {
        context.Settings.MustNotBeNull("settings");
        context.Settings.CancelUrl.MustNotBeNull("settings.CancelUrl");

        return context.Settings.CancelUrl;
    }

    public override string GetContinueUrl(PaymentProviderContext<WindcaveSettings> context)
    {
        context.Settings.MustNotBeNull("settings");
        context.Settings.CancelUrl.MustNotBeNull("settings.ContinueUrl");

        // set the location to navigate to and add a parameter for the order id
        return context.Settings.ContinueUrl + $"{context.Order.Id}";
    }

    public override string GetErrorUrl(PaymentProviderContext<WindcaveSettings> context)
    {
        context.Settings.MustNotBeNull("settings");
        context.Settings.CancelUrl.MustNotBeNull("settings.ErrorUrl");

        return context.Settings.ErrorUrl;
    }
}
