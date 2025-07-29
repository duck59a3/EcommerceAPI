namespace MyWebApi.Middleware
{
    public class FileUploadMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly long _maxFileSize;

        public FileUploadMiddleware(RequestDelegate next, long maxFileSize = 10 * 1024 * 1024) // 10MB default
        {
            _next = next;
            _maxFileSize = maxFileSize;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.ContentType != null &&
                context.Request.ContentType.Contains("multipart/form-data"))
            {
                if (context.Request.ContentLength > _maxFileSize)
                {
                    context.Response.StatusCode = 413; // Payload Too Large
                    await context.Response.WriteAsync("File size exceeds the maximum allowed limit.");
                    return;
                }
            }

            await _next(context);
        }
    }
    public static class FileUploadMiddlewareExtensions
    {
        public static IApplicationBuilder UseFileUploadMiddleware(this IApplicationBuilder builder, long maxFileSize = 10 * 1024 * 1024)
        {
            return builder.UseMiddleware<FileUploadMiddleware>(maxFileSize);
        }
    }
}
