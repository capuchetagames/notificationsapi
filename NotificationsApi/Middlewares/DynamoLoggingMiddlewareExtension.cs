namespace NotificationsApi.Middlewares;

public static class DynamoLoggingMiddlewareExtension
{
    public static IApplicationBuilder UseDynamoLogging(this IApplicationBuilder app)
        => app.UseMiddleware<DynamoLoggingMiddleware>();
    
}