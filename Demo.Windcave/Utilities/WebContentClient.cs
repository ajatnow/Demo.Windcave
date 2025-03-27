using Azure;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Pkcs;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Demo.Commerce.Utility.Web
{
    public interface IWebContentClient
    {
        Task<WebContentResult<TOut>> GetAsync<TOut>(string url, Dictionary<string, string> additionalHeaders, bool isIgnoreNulls, string username, string password) where TOut : class;
        Task<WebContentResult<TOut>> PostAsync<TOut, TPost>(string url, TPost postData, Dictionary<string, string> additionalHeaders, bool isIgnoreNulls, string username, string password) where TOut : class;
    }

    public class WebContentClient : IWebContentClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerSettings _jsonSettings;

        public WebContentClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _jsonSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore };
        }

        public async Task<WebContentResult<TOut>> GetAsync<TOut>(string url,
                                                                 Dictionary<string, string> additionalHeaders,
                                                                 bool isIgnoreNulls,
                                                                 string username,
                                                                 string password
                                                                ) where TOut : class
        {
            using (var client = _httpClientFactory.CreateClient())
            {

                // use your favourte rest client
                //...

                // code omitted for brevity
                var rawData = "raw string from response received";

                // deserialize when there is data and depending on whether are ignoring nulls
                TOut convertedData = default(TOut);
                if (rawData != null)
                {
                    convertedData = isIgnoreNulls ? JsonConvert.DeserializeObject<TOut>(rawData, _jsonSettings) : JsonConvert.DeserializeObject<TOut>(rawData);
                }

                return new WebContentResult<TOut>()
                {
                    RawContent = rawData,
                    Result = convertedData,
                    HttpStatusCode = (int)HttpStatusCode.OK     // whatever the response returns(int)response.StatusCode
                };
            }
        }

        /// <summary>
        /// A POST action with body and user/pwd for Basic auth
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <typeparam name="TPost"></typeparam>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="additionalHeaders"></param>
        /// <param name="isIgnoreNulls"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<WebContentResult<TOut>> PostAsync<TOut, TPost>(string url,
                                                                         TPost postData,
                                                                         Dictionary<string, string> additionalHeaders,
                                                                         bool isIgnoreNulls,
                                                                         string username,
                                                                         string password
                                                                        ) where TOut : class
        {
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                // use your favourte rest client
                //... 
                // code omitted for brevity

                var rawData = "raw string from response received";
                // deserialize when there is data and depending on whether are ignoring nulls
                TOut convertedData = default(TOut);
                if (rawData != null)
                {
                    convertedData = isIgnoreNulls ? JsonConvert.DeserializeObject<TOut>(rawData, _jsonSettings) : JsonConvert.DeserializeObject<TOut>(rawData);
                }

                return new WebContentResult<TOut>()
                {
                    RawContent = rawData,
                    Result = convertedData,
                    HttpStatusCode = (int)HttpStatusCode.OK     // whatever the response returns(int)response.StatusCode
                };
            }
        }

    }
}
