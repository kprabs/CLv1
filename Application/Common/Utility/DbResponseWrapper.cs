using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace CoreLib.Application.Common.Utility
{
    public static class DbResponseWrapper
    {
        public static ApiResponse<T> ResponseWrapper<T>(T? result)
        {
            var isNullOrEmpty = EqualityComparer<T>.Default.Equals(result, default(T));
            var response = new ApiResponse<T>();
            var errorList = new List<string>();
            response.data = result;
            response.statusCode = (!isNullOrEmpty) ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest;
            response.status = response.statusCode == StatusCodes.Status200OK;
            if (isNullOrEmpty)
            {
                errorList.Add("No Result Found");
            }
            response.errors = errorList;
            response.timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            response.id = $"{DateTime.Now:yyyyMMddHHmmssfff}_{Guid.NewGuid()}";

            return response;
        }
    }
}
