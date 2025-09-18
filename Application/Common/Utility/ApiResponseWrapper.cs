using CoreLib.Entities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Globalization;

namespace CoreLib.Application.Common.Utility
{
    public static class ApiResponseWrapper
    {
        /// <summary>
        /// The Response Wrapper method handles customizations and generate Formatted Response.
        /// </summary>
        /// <param name="result">
        /// The Result
        /// </param>
        /// <param name="baseMessage">
        /// baseMessage from Enterprise endpoint response
        /// </param>
        /// <returns>
        /// ApiResponse Object
        /// </returns>
        public static ApiResponse<T> ResponseWrapper<T>(T? result, object? baseMessageGeneric)
        {
            ApiResponse<T> response = new();
            List<string> _errors = [];
            string strBaseMessage = JsonConvert.SerializeObject(baseMessageGeneric);
            BaseMessage? baseMessage = JsonConvert.DeserializeObject<BaseMessage>(strBaseMessage);

            response.data = result;
#pragma warning disable S2955
            response.statusCode = (result != null && Convert.ToInt32(baseMessage?.ReturnCode, CultureInfo.InvariantCulture) == StatusCodes.Status200OK)
                ? StatusCodes.Status200OK : Convert.ToInt32(baseMessage?.ReturnCode, CultureInfo.InvariantCulture);
#pragma warning restore S2955
            response.status = Convert.ToInt32(baseMessage?.ReturnCode, CultureInfo.InvariantCulture) == StatusCodes.Status200OK;
            if (Convert.ToInt32(baseMessage?.ReturnCode, CultureInfo.InvariantCulture) != StatusCodes.Status200OK &&
                !string.IsNullOrWhiteSpace(baseMessage?.ReturnCodeDescription))
            {
                _errors.Add(baseMessage.ReturnCodeDescription);
            }
            response.errors = _errors;
            response.timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            response.id = $"{DateTime.Now:yyyyMMddHHmmssfff}_{Guid.NewGuid()}";

            return response;
        }
    }
}
