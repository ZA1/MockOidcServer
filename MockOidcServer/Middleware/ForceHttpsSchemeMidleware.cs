namespace MockOidcServer.Middleware;

public class ForceHttpsSchemeMiddleware(RequestDelegate next, IConfiguration configuration)
{
    private readonly bool _force = configuration.GetValue<bool>("ForceHttps");

    public async Task InvokeAsync(HttpContext context)
    {
        if (_force)
        {
            // Force the scheme to HTTPS
            context.Request.Scheme = "https";
        }

        // Continue to the next middleware in the pipeline
        await next(context);
    }
}

public static class ForceHttpsSchemeMiddlewareExtensions
{
    public static IApplicationBuilder UseForceHttpsScheme(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ForceHttpsSchemeMiddleware>();
    }
}