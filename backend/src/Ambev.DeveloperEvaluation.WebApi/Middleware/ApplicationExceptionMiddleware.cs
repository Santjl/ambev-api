using Ambev.DeveloperEvaluation.WebApi.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Middleware
{
    public class ApplicationExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ApplicationExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ApplicationException ex)
            {
                await HandleApplicationExceptionAsync(context, ex);
            }
        }

        private static Task HandleApplicationExceptionAsync(HttpContext context, ApplicationException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            var response = new ApiResponse
            {
                Success = false,
                Message = string.IsNullOrWhiteSpace(ex.Message) ? "An error as occurred." : ex.Message
            };

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
