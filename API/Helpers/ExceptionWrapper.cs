using CoreLib.Application.Common.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace CoreLib.API.Helpers
{
    public static class ExceptionWrapper
    {
        /// <summary>
        /// The Response wrapper method that generates formatted response for validation and exception responses
        /// </summary>
        /// <param name="result">
        /// Exception/Validation object
        /// </param>
        /// <param name="context">
        /// The HTTP Context
        /// </param>
        /// /// <param name="hostEnvironment">
        /// The Host Environemnt Information
        /// </param>
        /// <returns>
        /// Formatted response object
        /// Response body already formatted
        /// </returns>
        public static ApiResponse<T>? Wrap<T>(object? result, HttpContext context, string hostEnvironment, ILogger logger)
        {
            string strResult = JsonConvert.SerializeObject(result);
            logger.LogError("ExceptionWrapper: " + strResult);
            List<string> _errors = [];
            ApiResponse<T>? response = new();
            try
            {
                var exceptionObject = JsonConvert.DeserializeObject<ErrorSchema>(strResult);
                if (exceptionObject == null || context == null)
                {
                    return response;
                }
                if (exceptionObject.Errors != null && context.Response.StatusCode == StatusCodes.Status400BadRequest)
                {
                    if (exceptionObject.Errors is List<string> errList)
                    {
                        foreach (var error in errList)
                        {
                            _errors.Add(error);
                        }
                    }
                    else if (exceptionObject.Errors is Dictionary<string, object> errDic)
                    {
                        foreach (var error in errDic)
                        {
                            if (error.Value is List<string> dictValue)
                            {
                                _errors.Add(dictValue.FirstOrDefault());
                            }
                        }
                    }
                    else
                    {
                        //SC: nothing to handle
                    }
                }
                else if (context.Response.StatusCode == StatusCodes.Status500InternalServerError)
                {
                    if (!string.IsNullOrWhiteSpace(exceptionObject.Title))
                    {
                        _errors.Add(exceptionObject.Title);
                    }

                    //Add detailed exception for Development environment
                    if (hostEnvironment?.ToUpperInvariant() == "DEVELOPMENT" && !string.IsNullOrEmpty(exceptionObject.Detail))
                    {
                        _errors.Add("Source | " + exceptionObject.Source);
                        _errors.Add(exceptionObject.Detail.Trim());
                    }
                }
                else
                {
                    response.errors = _errors;
                }

                response.errors = _errors;
                response.data = (T?)exceptionObject.Data;
                response.timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                response.id = $"{DateTime.Now:yyyyMMddHHmmssfff}_{Guid.NewGuid()}";
                response.statusCode = context.Response.StatusCode;
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ExceptionWrapper: Error while wrapping exception response");
                response = JsonConvert.DeserializeObject<ApiResponse<T>>(strResult);
            }
            return response;
        }
    }

    /// <summary>
    /// Exception Schema 
    /// </summary>
    /// <returns>
    /// It gives title, status, Dictionary of errors
    /// </returns>
    public class ExceptionSchema
    {
        public string? Title { get; set; }
        public string? Status { get; set; }
        public List<object>? Errors { get; set; }
    }

    /// <summary>
    /// Error Schema 
    /// </summary>
    /// /// <returns>
    /// It gives details of errors
    /// </returns>
    public class ErrorSchema
    {
        public string? Title { get; set; }
        public string? Status { get; set; }
        public string? Detail { get; set; }
        public string? Source { get; set; }
        private List<string> errorsList = [];
        private Dictionary<string, List<string>> errorDictionary = [];
        private List<string> dataList = [];
        private Dictionary<string, object> dataDictionary = [];
        public object? Errors
        {
            get
            {
                return errorDictionary.Count > 0 ? errorDictionary : (errorsList.Count > 0 ? errorsList : null);
            }
            set
            {
                if (value == null)
                {
                    errorsList = [];
                    errorDictionary = [];
                    return;
                }
                var token = value as JToken ?? JToken.FromObject(value);
                if (token.Type == JTokenType.Array)
                {
                    errorsList = token.ToObject<List<string>>() ?? [];
                }
                else if (token.Type == JTokenType.Object)
                {
                    errorDictionary = token.ToObject<Dictionary<string, List<string>>>() ?? [];
                }
                else
                {
                    errorsList = [];
                    errorDictionary = [];
                }
            }
        }

        public int? StatusCode { get; set; }
        public object? Data
        {
            get
            {
                return dataDictionary.Count > 0 ? dataDictionary : (dataList.Count > 0 ? dataList : null);
            }
            set
            {
                if (value == null)
                {
                    dataList = [];
                    dataDictionary = [];
                    return;
                }
                var token = value as JToken ?? JToken.FromObject(value);
                if (token.Type == JTokenType.Array)
                {
                    dataList = token.ToObject<List<string>>() ?? [];
                }
                else if (token.Type == JTokenType.Object)
                {
                    dataDictionary = token.ToObject<Dictionary<string, object>>() ?? [];
                }
                else
                {
                    dataList = [];
                    dataDictionary = [];
                }

            }
        }
    }
}
