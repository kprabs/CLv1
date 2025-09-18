using CoreLib.Application.Common.Constants;
using CoreLib.Application.Common.Utility;
using CoreLib.Entities;
using Swashbuckle.AspNetCore.Filters;
using System.Globalization;

namespace CoreLib.API.SwaggerExamples
{
    public class RequestTimeoutSwaggerResponse : IMultipleExamplesProvider<ApiResponse<object>>
    {
        public IEnumerable<SwaggerExample<ApiResponse<object>>> GetExamples()
        {
            BaseMessage baseMessage = new()
            {
                ReturnCode = CustomHTTPCode.TimeoutStatusCode.ToString(CultureInfo.InvariantCulture),
            };

            var response = ApiResponseWrapper.ResponseWrapper((object?)null, baseMessage);
            yield return SwaggerExample.Create("Request Timeout", response);
        }
    }
}
