using Microsoft.AspNetCore.Http;

namespace Ambev.DeveloperEvaluation.Application.Messaging
{
    public sealed class CorrelationMiddleware(RequestDelegate next, HttpCorrelationProvider provider)
    {
        private const string Header = "X-Correlation-Id";
        public async Task Invoke(HttpContext ctx)
        {
            var id = ctx.Request.Headers.TryGetValue(Header, out var v) && Guid.TryParse(v, out var g) ? g : Guid.NewGuid();
            provider.Set(id);
            ctx.Response.Headers[Header] = id.ToString();
            await next(ctx);
        }
    }
}
