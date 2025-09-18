using CoreLib.API.Attributes;
using CoreLib.Application.Common.Constants;
using CoreLib.Application.Common.Utility;
using CoreLib.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Globalization;

namespace CoreLib.API.Filters
{
    public class RequestTimeoutMiddleware(RequestDelegate next, ILogger<RequestTimeoutMiddleware> logger)
    {
        private TimeSpan _timeout;

        public async Task InvokeAsync(HttpContext context)
        {
            var apiName = ((dynamic)context.Request).RouteValues["action"];
            var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            int customTime = TimeoutConstants.DefaultTimeout;
            if (endpoint != null)
            {
                var customTimeoutFilter = endpoint.Metadata.GetMetadata<CustomTimeoutAttribute>();
                if(customTimeoutFilter != null)
                {
                    customTime = customTimeoutFilter.CustomTime;
                }
            }

            ApiTimmer apiTimmer = new();
            _timeout = apiTimmer.getTimeSpan(customTime);
            using (CancellationTokenSource cts = new(_timeout))
            {
                var token = cts.Token;
                context.RequestAborted = token;
                var task = Task.Run(async () =>
                {
                    await next(context);
                }, token);
                if (await Task.WhenAny(task, Task.Delay(_timeout, token)) == task)
                {
                    await task;
                }
                else
                {
                    context.Response.StatusCode = CustomHTTPCode.TimeoutStatusCode;
                    BaseMessage baseMessage = new()
                    {
                        ReturnCode = Convert.ToString(CustomHTTPCode.TimeoutStatusCode, CultureInfo.InvariantCulture),
                        ReturnCodeDescription = CustomHTTPCode.TimeoutStatusMessage
                    };
                    var result = JsonConvert.SerializeObject(baseMessage);
                    logger.LogError($"{apiName} - TimeOut420 :: {result} ");
                    await context.Response.WriteAsync(result);
                }
            }
        }
    }
}
