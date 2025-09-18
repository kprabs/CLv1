#pragma warning disable S3994
#pragma warning disable CS8603
#pragma warning disable CS8597
#pragma warning disable S2360
using CoreLib.Application.Common.Constants;
using CoreLib.Application.Common.Models;
using CoreLib.Application.Common.Utility;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CoreLib.Infrastructure.Persistence
{
    internal static partial class HttpClientWrapperLogMessages
    {
        [LoggerMessage(Level = LogLevel.Information, EventId = 1, Message = "method = {method}, url = {url}")]
        internal static partial void RequestInfo(this ILogger logger, string method, string url);

        [LoggerMessage(Level = LogLevel.Information, EventId = 2, Message = "method = {method}, url = {url}, postedData = {postedData}")]
        internal static partial void RequestInfoWithContent(this ILogger logger, string method, string url, IEnumerable<KeyValuePair<string, string>> postedData);

        [LoggerMessage(Level = LogLevel.Information, EventId = 3, Message = "method = {method}, url = {url}, postedData = {@postedData}")]
        internal static partial void RequestInfoWithObjectContent(this ILogger logger, string method, string url, object postedData);

        [LoggerMessage(Level = LogLevel.Information, EventId = 4, Message = "response = {response}")]
        internal static partial void Response(this ILogger logger, string response);

        [LoggerMessage(Level = LogLevel.Information, EventId = 5)]
        internal static partial void AccessTokenAvailable(this ILogger logger);

        [LoggerMessage(Level = LogLevel.Information, EventId = 6, Message = "url = {url}")]
        internal static partial void AccessTokenUrl(this ILogger logger, string url);

        [LoggerMessage(Level = LogLevel.Information, EventId = 7, Message = "method = {method}, url = {url}, headers = {headers}, postedData = {@postedData}")]
        internal static partial void RequestInfoWithHeadersAndContentObject(this ILogger logger, string method, string url, Dictionary<string, string> headers, object postedData);
    }

    public class HttpClientWrapper(ILogger logger)
    {
        /// <summary>
        /// For getting the resources from a web api
        /// </summary>
        /// <param name="url">API Url</param>
        /// <returns>A Task with result object of type T</returns>
        public async Task<T> GetAsync<T>(string apiUrl, object? queryObject, bool isAccessToken = false) where T : class
        {
            return await GetAsync<T>(apiUrl, queryObject, null, isAccessToken);
        }

#pragma warning disable S4005
        /// <summary>
        /// For getting the resources from a web api
        /// </summary>
        /// <param name="url">API Url</param>
        /// <returns>A Task with result object of type T</returns>
        public async Task<T> GetAsync<T>(string apiUrl, object? queryObject, Dictionary<string, string>? headers,
                bool isAccessToken = false, string? access_token = null, int customTime = TimeoutConstants.DefaultTimeout) where T : class
        {
            var result = default(T);
            ApiTimmer apiTimmer = new();
            string apiName = getAPINameFromURl(apiUrl);

            TimeSpan _timeout = apiTimmer.getTimeSpan(customTime).Subtract(TimeSpan.FromSeconds(2));
            using (HttpClient httpClient = new())
            {
                //Append parameters to url
                var queryStringUrl = queryObject != null ? ObjectToQueryString(apiUrl, queryObject) : apiUrl;

                logger.RequestInfo("GET", queryStringUrl);

                HttpRequestMessage request = new(HttpMethod.Get, queryStringUrl);
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }
                if (isAccessToken)
                {
                    await AddAccessTokenInRequestHeaderAsync(access_token, request);
                }
                using (CancellationTokenSource cts = new(_timeout))
                {
                    HttpResponseMessage response = new();
                    var task = Task.Run(async () =>
                    {
                        response = await httpClient.SendAsync(request, cts.Token);
                    });
                    if (await Task.WhenAny(task, Task.Delay(_timeout, cts.Token)) == task)
                    {
                        await task;
                    }
                    else
                    {
                        response.StatusCode = (System.Net.HttpStatusCode)CustomHTTPCode.TimeoutStatusCode;
                        response.ReasonPhrase = CustomHTTPCode.TimeoutStatusMessage;
                        logger.LogError($"{apiName} - TimeOut420 :: {response} ");
                    }

                    await response.Content.ReadAsStringAsync().ContinueWith(x =>
                    {
                        logger.Response(x.Result);
                        result = JsonConvert.DeserializeObject<T>(x.Result);
                    });
                }
            }
            return result;
        }

        /// <summary>
        /// For creating a new item over a web api using POST
        /// </summary>
        /// <param name="apiUrl">API Url</param>
        /// <param name="postObject">The object to be created</param>
        /// <returns>A Task with created item</returns>
        public async Task<TResultType> PostAsync<TResultType, TRequestType>(string apiUrl, TRequestType postObject, Dictionary<string, string>? headers,
            bool isAccessToken = false, string access_token = null, int customTime = TimeoutConstants.DefaultTimeout) where TRequestType : class where TResultType : class
        {
            TResultType? result = null;
            ApiTimmer apiTimmer = new();

            TimeSpan _timeout = apiTimmer.getTimeSpan(customTime).Subtract(TimeSpan.FromSeconds(2));
            using (HttpClient client = new())
            {
                HttpRequestMessage request = new(HttpMethod.Post, apiUrl);

                string postObjectJson = postObject.SerializeObjectToJson();

                logger.RequestInfoWithObjectContent("POST", apiUrl, postObject);

                HttpContent content = new StringContent(postObjectJson, Encoding.UTF8, "application/json");
                request.Content = content;

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }
                if (isAccessToken)
                {
                    await AddAccessTokenInRequestHeaderAsync(access_token, request);
                }
                using (CancellationTokenSource cts = new(_timeout))
                {
                    HttpResponseMessage response = new();
                    var task = Task.Run(async () =>
                    {
                        response = await client.SendAsync(request, cts.Token).ConfigureAwait(false);
                    });
                    if (await Task.WhenAny(task, Task.Delay(_timeout, cts.Token)) == task)
                    {
                        await task;
                    }
                    else
                    {
                        response.StatusCode = (System.Net.HttpStatusCode)CustomHTTPCode.TimeoutStatusCode;
                        response.ReasonPhrase = CustomHTTPCode.TimeoutStatusMessage;
                        logger.LogError($"Request Url: {apiUrl} - TimeOut420 :: {response} ");
                    }

                    if (response == null)
                    {
                        logger.LogError($"Request Url: {apiUrl} - No response recieved");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content == null)
                    {
                        logger.LogInformation($"Request Url: {apiUrl} - HttpStatusCode: 200 with no content");
                    }
                    else
                    {
                        await response.Content.ReadAsStringAsync().ContinueWith(x =>
                        {
                            logger.Response(x.Result);
                            result = JsonConvert.DeserializeObject<TResultType>(x.Result);
                        });
                    }
                }
            }
            return result;
        }

        public async Task<string> PostXmlAsync<T>(string apiUrl, T postObject, Dictionary<string, string>? headers, int customTime = TimeoutConstants.DefaultTimeout,
            bool isAccessToken = false, string access_token = null) where T : class
        {
            string? result;
            ApiTimmer apiTimmer = new();
            string apiName = getAPINameFromURl(apiUrl);
            TimeSpan _timeout = apiTimmer.getTimeSpan(customTime).Subtract(TimeSpan.FromSeconds(2));

            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);

                string postObjectXml;
                XmlDocument xmlDoc = new();
                XmlSerializer xmlSerializer = new(postObject.GetType());

                logger.RequestInfoWithHeadersAndContentObject("POST", apiUrl, headers, postObject);
                using (MemoryStream xmlStream = new())
                {
                    xmlSerializer.Serialize(xmlStream, postObject);
                    xmlStream.Position = 0;
                    xmlDoc.Load(xmlStream);
                    postObjectXml = xmlDoc.InnerXml;
                }

                HttpContent content = new StringContent(postObjectXml, Encoding.UTF8, "application/xml");
                request.Content = content;

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }
                if (isAccessToken)
                {
                    await AddAccessTokenInRequestHeaderAsync(access_token, request);
                }
                using (CancellationTokenSource cts = new CancellationTokenSource(_timeout))
                {
                    HttpResponseMessage response = new HttpResponseMessage();
                    var task = Task.Run(async () =>
                    {
                        response = await client.SendAsync(request, cts.Token).ConfigureAwait(false);
                    });
                    if (await Task.WhenAny(task, Task.Delay(_timeout, cts.Token)) == task)
                    {
                        await task;
                    }
                    else
                    {
                        response.StatusCode = (System.Net.HttpStatusCode)CustomHTTPCode.TimeoutStatusCode;
                        response.ReasonPhrase = CustomHTTPCode.TimeoutStatusMessage;
                        logger.LogError($"{apiName} - TimeOut420 :: {response} ");
                    }
                    result = await response.Content.ReadAsStringAsync();
                }
                logger.Response(result);
            }
            return result;
        }

        /// <summary>
        /// For creating a new item over a web api using POST
        /// When response object is different from request object
        /// </summary>
        /// <param name="apiUrl">API Url</param>
        /// <param name="postObject">The object to be created</param>
        /// <returns>A Task with created item</returns>
        public async Task<TResultType> PostAsync<TResultType, TRequestType>(string apiUrl, TRequestType postObject, bool isAccessToken = false, string access_token = null,
            int customTime = TimeoutConstants.DefaultTimeout) where TResultType : class where TRequestType : class
        {
            TResultType? result = null;
            ApiTimmer apiTimmer = new ApiTimmer();
            string apiName = getAPINameFromURl(apiUrl);
            //TimeSpan _timeout = apiTimmer.getTimeSpan(apiName).Subtract(TimeSpan.FromSeconds(2));
            TimeSpan _timeout = apiTimmer.getTimeSpan(customTime).Subtract(TimeSpan.FromSeconds(2));
            using (HttpClient client = new())
            {
                HttpRequestMessage request = new(HttpMethod.Post, apiUrl);

                string postObjectJson = postObject.SerializeObjectToJson();

                logger.RequestInfoWithObjectContent("POST", apiUrl, postObject);

                HttpContent content = new StringContent(postObjectJson, Encoding.UTF8, "application/json");
                request.Content = content;

                if (isAccessToken)
                {
                    await AddAccessTokenInRequestHeaderAsync(access_token, request);
                }
                using (CancellationTokenSource cts = new CancellationTokenSource(_timeout))
                {
                    HttpResponseMessage response = new HttpResponseMessage();
                    var task = Task.Run(async () =>
                    {
                        response = await client.SendAsync(request, cts.Token).ConfigureAwait(false);
                    });
                    if (await Task.WhenAny(task, Task.Delay(_timeout, cts.Token)) == task)
                    {
                        await task;
                    }
                    else
                    {
                        response.StatusCode = (System.Net.HttpStatusCode)CustomHTTPCode.TimeoutStatusCode;
                        response.ReasonPhrase = CustomHTTPCode.TimeoutStatusMessage;
                        logger.LogError($"{apiName} - TimeOut420 :: {response} ");
                    }

                    await response.Content.ReadAsStringAsync().ContinueWith(x =>
                    {
                        logger.Response(x.Result);
                        result = JsonConvert.DeserializeObject<TResultType>(x.Result);
                    });
                }
            }
            return result;
        }

        public async Task<T> PostAsyncEncoded<T>(string apiUri,
            IList<KeyValuePair<string, string>> postObject, Dictionary<string, string>? headers = null, int customTime = TimeoutConstants.DefaultTimeout, bool isAccessToken = false, string access_token = null) where T : class
        {
            var result = default(T);
            ApiTimmer apiTimmer = new ApiTimmer();
            string apiName = getAPINameFromURl(apiUri);
            TimeSpan _timeout = apiTimmer.getTimeSpan(customTime).Subtract(TimeSpan.FromSeconds(2));
            using (HttpClient client = new())
            {
                HttpRequestMessage request = new(HttpMethod.Post, apiUri);

                HttpContent content = new FormUrlEncodedContent(postObject);
                request.Content = content;

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                if (isAccessToken)
                {
                    await AddAccessTokenInRequestHeaderAsync(access_token, request);
                }
                using (CancellationTokenSource cts = new CancellationTokenSource(_timeout))
                {
                    HttpResponseMessage response = new HttpResponseMessage();
                    var task = Task.Run(async () =>
                    {
                        response = await client.SendAsync(request, cts.Token).ConfigureAwait(false);
                    });
                    if (await Task.WhenAny(task, Task.Delay(_timeout, cts.Token)) == task)
                    {
                        await task;
                    }
                    else
                    {
                        response.StatusCode = (System.Net.HttpStatusCode)CustomHTTPCode.TimeoutStatusCode;
                        response.ReasonPhrase = CustomHTTPCode.TimeoutStatusMessage;
                        logger.LogError($"{apiName} - TimeOut420 :: {response} ");
                    }

                    await response.Content.ReadAsStringAsync().ContinueWith(x =>
                    {
                        result = JsonConvert.DeserializeObject<T>(x.Result);
                    });
                }
            }
            return result;
        }

        /// <summary>
        /// For updating an existing item over a web api using PUT
        /// </summary>
        /// <param name="apiUrl">API Url</param>
        /// <param name="putObject">The object to be edited</param>
        public async Task PutAsync<T>(string apiUrl, T putObject, Dictionary<string, string>? headers,
            bool isAccessToken = false, string access_token = null, int customTime = TimeoutConstants.DefaultTimeout) where T : class
        {
            ApiTimmer apiTimmer = new ApiTimmer();
            string apiName = getAPINameFromURl(apiUrl);
            TimeSpan _timeout = apiTimmer.getTimeSpan(customTime).Subtract(TimeSpan.FromSeconds(2));
            using (HttpClient client = new())
            {
                var request = new HttpRequestMessage(HttpMethod.Put, apiUrl);

                string putObjectJson = putObject.SerializeObjectToJson();

                logger.RequestInfoWithObjectContent("PUT", apiUrl, putObject);

                HttpContent content = new StringContent(putObjectJson, Encoding.UTF8, "application/json");
                request.Content = content;

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                if (isAccessToken)
                {
                    await AddAccessTokenInRequestHeaderAsync(access_token, request);
                }
                using (CancellationTokenSource cts = new CancellationTokenSource(_timeout))
                {
                    HttpResponseMessage response = new HttpResponseMessage();
                    var task = Task.Run(async () =>
                    {
                        response = await client.SendAsync(request, cts.Token).ConfigureAwait(false);
                    });
                    if (await Task.WhenAny(task, Task.Delay(_timeout, cts.Token)) == task)
                    {
                        await task;
                    }
                    else
                    {
                        response.StatusCode = (System.Net.HttpStatusCode)CustomHTTPCode.TimeoutStatusCode;
                        response.ReasonPhrase = CustomHTTPCode.TimeoutStatusMessage;
                        logger.LogError($"{apiName} - TimeOut420 :: {response} ");
                    }
                }
            }
        }

        private string getAPINameFromURl(string apiUrl)
        {
            Uri _uri = new Uri(apiUrl);
            string serviceName = _uri.Segments.LastOrDefault().Split(new[] { ';' }).First();
            return serviceName;
        }

        private async Task AddAccessTokenInRequestHeaderAsync(string? access_token, HttpRequestMessage? request)
        {
            if (string.IsNullOrEmpty(access_token))
            {
                var accessTokenInfo = await GetAccessToken();
                access_token = accessTokenInfo?.access_token;
            }
            if (!string.IsNullOrWhiteSpace(access_token))
            {
                logger.AccessTokenAvailable();
                request.Headers.Add("Authorization", $"{AccessTokenParams.Bearer} {access_token}");
            }
        }

        //create query string
        private static string ObjectToQueryString(string url, object obj, bool isCamelCase = true)
        {
            var properties = obj.GetType().GetProperties();
            var queryStringParams = new List<KeyValuePair<string, string>>();

            foreach (var property in properties)
            {
                string propertyName = isCamelCase ? System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(property.Name) : property.Name;
                if (property.PropertyType == typeof(List<string>))
                {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    var listValues = (List<string>)property.GetValue(obj);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                    if (listValues == null || listValues.Count <= 0)
                    {
                        continue;
                    }
                    foreach (var item in listValues)
                    {
                        queryStringParams.Add(new KeyValuePair<string, string>(propertyName, item));
                    }
                }
                else
                {
                    var value = property.GetValue(obj)?.ToString();
                    if (!string.IsNullOrEmpty(value))
                    {
                        queryStringParams.Add(new KeyValuePair<string, string>(propertyName, value));
                    }
                }
            }
            var queryString = QueryHelpers.AddQueryString(url, queryStringParams);

            return queryString;
        }

        public async Task<ForgerockAccessTokenDTO> GetAccessToken()
        {
            var baseUrlConfigSection = ConfigurationHelper.config.GetSection(UrlConst.EnterpriseBaseUrls);
            var baseUrl = baseUrlConfigSection[UrlConst.ForgerocAccessToken] ?? string.Empty;
            var url = ComposeUrl(baseUrl, UrlConst.GetAccessToken);

            logger.AccessTokenUrl(url);
            List<KeyValuePair<string, string>> queryParams =
            [
                new KeyValuePair<string, string>(AccessTokenParams.GrantType, ConfigurationHelper.config.GetSection(AccessTokenParams.AccessTokenGrantType).Value ?? string.Empty),
                new KeyValuePair<string, string>(AccessTokenParams.ClientId, ConfigurationHelper.config.GetSection(AccessTokenParams.AccessTokenClientId).Value ?? string.Empty),
                new KeyValuePair<string, string>(AccessTokenParams.ClientSecret, ConfigurationHelper.config.GetSection(AccessTokenParams.AccessTokenClientSecret).Value ?? string.Empty),
            ];
            var accessTokenInfo = await PostAsyncEncoded<ForgerockAccessTokenDTO>(url, queryParams, null);
            return accessTokenInfo;
        }

        private static string ComposeUrl(string baseUrl, string relativeUrl)
        {
            UriBuilder uriBuilder = new(baseUrl)
            {
                Path = relativeUrl
            };
            return uriBuilder.ToString();
        }
    }
}
