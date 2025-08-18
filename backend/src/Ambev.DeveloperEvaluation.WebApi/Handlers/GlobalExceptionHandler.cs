using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.WebApi.Common;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Ambev.DeveloperEvaluation.WebApi.Filters
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
            => _logger = logger;

        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken _)
        {
            if (context.Response.HasStarted) return false;

            var (status, response) = MapException(exception);

            if ((int)status >= 500)
                _logger.LogError(exception, "Unhandled exception");
            else
                _logger.LogWarning(exception, "Handled exception: {ExceptionType}", exception.GetType().Name);

            context.Response.StatusCode = (int)status;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(response);
            return true;
        }

        private static (HttpStatusCode status, ApiResponse body) MapException(Exception ex)
        {
            switch (ex)
            {
                case ValidationException vex:
                    return (HttpStatusCode.BadRequest, new ApiResponse
                    {
                        Success = false,
                        Message = "Validation Failed",
                        Errors = vex.Errors.Select(e => new ValidationErrorDetail
                        {
                            Detail = e.ErrorMessage,
                            Error = e.ErrorCode
                        })
                    });

                case UnauthorizedAccessException uex:
                    return (HttpStatusCode.Unauthorized, new ApiResponse
                    {
                        Success = false,
                        Message = string.IsNullOrWhiteSpace(uex.Message) ? "Unauthorized." : uex.Message
                    });

                case KeyNotFoundException knf:
                    return (HttpStatusCode.NotFound, new ApiResponse
                    {
                        Success = false,
                        Message = string.IsNullOrWhiteSpace(knf.Message) ? "Resource not found." : knf.Message
                    });

                case ApplicationException aex:
                    return (HttpStatusCode.BadRequest, new ApiResponse
                    {
                        Success = false,
                        Message = string.IsNullOrWhiteSpace(aex.Message) ? "An error has occurred." : aex.Message
                    });

                default:
                    return (HttpStatusCode.InternalServerError, new ApiResponse
                    {
                        Success = false,
                        Message = "Internal server error: " + ex.Message
                    });
            }
        }
    }
}