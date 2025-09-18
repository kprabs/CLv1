using CoreLib.Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace CoreLib.API.Filters
{
    /// <summary>
    /// The API Exception filter attribute is to handle multiple errors
    /// </summary>
    /// <param name="hostEnvironment">
    /// Host Environment
    /// </param>
    /// <returns>
    /// ApiResponse Object
    /// </returns>
    /// 
#pragma warning disable S3993
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
#pragma warning restore S3993
    {
        private readonly ILogger<ApiExceptionFilterAttribute> _logger;
        private readonly Dictionary<Type, Action<ExceptionContext>> _exceptionHandlers;

        public ApiExceptionFilterAttribute(ILogger<ApiExceptionFilterAttribute> logger)
        {
            _logger = logger;
            /// Register known exception types and handlers.
            _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
            {
                { typeof(ValidationException), HandleValidationException },
                { typeof(NotFoundException), HandleNotFoundException },
                { typeof(UnauthorizedAccessException), HandleUnauthorizedAccessException },
                { typeof(ForbiddenAccessException), HandleForbiddenAccessException },
            };
        }

        public override void OnException(ExceptionContext context)
        {
            HandleException(context);
            LogException(context);
            base.OnException(context);
        }

        private void LogException(ExceptionContext context)
        {
            var exceptionLog = new ExceptionLog(context);
            _logger.Log(LogLevel.Error, new EventId(1, "Exception"), exceptionLog, context.Exception, ExceptionLog.Callback);
        }


        /// <summary>
        /// handles ModelstateExceptions otherwise it handlesunknowException
        /// </summary>
        private void HandleException(ExceptionContext context)
        {
            Type type = context.Exception.GetType();
            if (_exceptionHandlers.ContainsKey(type))
            {
                _exceptionHandlers[type].Invoke(context);
                return;
            }

            if (!context.ModelState.IsValid)
            {
                HandleInvalidModelStateException(context);
                return;
            }

            HandleUnknownException(context);
        }
        /// <summary>
        /// handles unknow Exceptions
        /// </summary>
        private static void HandleUnknownException(ExceptionContext context)
        {

            ProblemDetails details = new()
            {
                Status = StatusCodes.Status500InternalServerError
            };
            /// If Host Environment is Development env
            //if (_hostEnvironment.IsDevelopment())
            //{
            details.Detail = context.Exception.StackTrace;
            details.Title = context.Exception.Message;
            details.Extensions.Add("Source", context.Exception.Source);
            //}
            //else
            //{
            //    /// If Host Environment is Development env
            //    details.Title = "An error occurred while processing your request.";
            //    details.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
            //}

            context.Result = new ObjectResult(details)
            {
                /// Returns 500 Internal Error
                StatusCode = StatusCodes.Status500InternalServerError
            };

            context.ExceptionHandled = true;
        }
        /// <summary>
        /// handles validation Exceptions
        /// </summary>
        private static void HandleValidationException(ExceptionContext context)
        {
            ValidateProblemDetails(context);
        }
        /// <summary>
        /// handles Invalid MOdelState Exception
        /// </summary>
        private static void HandleInvalidModelStateException(ExceptionContext context)
        {
            ValidateProblemDetails(context);
        }
        /// <summary>
        /// Method to validate modelstate /Errors
        /// </summary>

        private static void ValidateProblemDetails(ExceptionContext context)
        {
            ValidationProblemDetails? details = null;
            var exception = (ValidationException)context.Exception;
            if (context.ModelState != null)
            {
                /// Captures details based on ModelState and Errors
                details = new ValidationProblemDetails(context.ModelState)
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
                };
            }
            else
            {
                if (context.ExceptionHandled)
                {
                    details = new ValidationProblemDetails(exception.Errors)
                    {
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
                    };
                }
            }

            context.Result = new BadRequestObjectResult(details);
            context.ExceptionHandled = true;
        }

        /// <summary>
        /// Method to validate modelstate /ErrorsHandles NOtFoundException
        /// </summary>
        private void HandleNotFoundException(ExceptionContext context)
        {
            /// Captures details of Exception
            var exception = (NotFoundException)context.Exception;

            var details = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "The specified resource was not found.",
                Detail = exception.ToString()
            };

            context.Result = new NotFoundObjectResult(details);

            context.ExceptionHandled = true;
        }
        /// <summary>
        /// Method to validate unauthorizedAccessException
        /// </summary>
        private void HandleUnauthorizedAccessException(ExceptionContext context)
        {
            var details = new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
            };

            context.Result = new ObjectResult(details)
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };

            context.ExceptionHandled = true;
        }
        /// <summary>
        /// Method to handle Forbidden Access Exception
        /// </summary>
        private void HandleForbiddenAccessException(ExceptionContext context)
        {
            var details = new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Title = "Forbidden",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
            };

            context.Result = new ObjectResult(details)
            {
                StatusCode = StatusCodes.Status403Forbidden
            };

            context.ExceptionHandled = true;
        }
    }
}
