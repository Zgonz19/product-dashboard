namespace ProductAPI.Middleware
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAttribute : Attribute 
    {
    }

    public class ApiKeyMiddleware
    {
        private const string ApiKeyHeaderName = "x-api-key";
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;

        public ApiKeyMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _config = config;
        }

        public async Task Invoke(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var requiresApiKey = endpoint?.Metadata.GetMetadata<ApiKeyAttribute>() != null;

            if (!requiresApiKey)
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("API Key is missing");
                return;
            }

            var configuredKey = _config.GetValue<string>("ApiKey");
            if (!string.Equals(extractedApiKey, configuredKey, StringComparison.Ordinal))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("API Key is invalid");
                return;
            }

            await _next(context);
        }
    }
}
