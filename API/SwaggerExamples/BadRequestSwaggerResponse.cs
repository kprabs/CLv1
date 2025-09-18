using CoreLib.Application.Common.Utility;
using CoreLib.Entities;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Filters;
using System.Globalization;

namespace CoreLib.API.SwaggerExamples
{
    public class BadRequestSwaggerResponse : IMultipleExamplesProvider<ApiResponse<object>>
    {
        public IEnumerable<SwaggerExample<ApiResponse<object>>> GetExamples()
        {
            BaseMessage baseMessage = new()
            {
                ReturnCode = StatusCodes.Status400BadRequest.ToString(CultureInfo.InvariantCulture),
                ReturnCodeDescription = "Bad request"
            };

            var response = ApiResponseWrapper.ResponseWrapper((object?)null, baseMessage);
            yield return SwaggerExample.Create("Bad Request", response);
        }
    }
}
