using CoreLib.Application.Common.Utility;
using CoreLib.Entities;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Filters;
using System.Globalization;

namespace CoreLib.API.SwaggerExamples
{
    public class NotFoundSwaggerResponse : IMultipleExamplesProvider<ApiResponse<object>>
    {
        public IEnumerable<SwaggerExample<ApiResponse<object>>> GetExamples()
        {
            BaseMessage baseMessage = new()
            {
                ReturnCode = StatusCodes.Status404NotFound.ToString(CultureInfo.InvariantCulture),
                ReturnCodeDescription = "Not Found"
            };

            var response = ApiResponseWrapper.ResponseWrapper<object>(null, baseMessage);
            yield return SwaggerExample.Create("Not Found", response);
        }
    }
}
