using CoreLib.Application.Common.Utility;
using CoreLib.Entities;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Filters;
using System.Globalization;

namespace CoreLib.API.SwaggerExamples
{
    public class ServerErrorSwaggerResponse : IMultipleExamplesProvider<ApiResponse<object>>
    {
        public IEnumerable<SwaggerExample<ApiResponse<object>>> GetExamples()
        {
            BaseMessage baseMessage = new()
            {
                ReturnCode = StatusCodes.Status500InternalServerError.ToString(CultureInfo.InvariantCulture),
                ReturnCodeDescription = "Server Error"
            };

            var response = ApiResponseWrapper.ResponseWrapper((object?)null, baseMessage);
            yield return SwaggerExample.Create("Server Error", response);
        }
    }
}
