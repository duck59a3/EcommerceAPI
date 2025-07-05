using Microsoft.AspNetCore.Mvc;
using MyWebApi.Logs;
using System.Net;
using System.Text.Json;

namespace MyWebApi.Middleware
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            //bien
            string message = "Xin lỗi, có lỗi hệ thống. Vui lòng thử lại";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string title = "Lỗi hệ thống";
            try
            {
                await next(context);
                //check exception nhiều request // 429 
                 if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    title = "Lỗi";
                    message = "Quá nhiều yêu cầu ";
                    statusCode = (int)StatusCodes.Status429TooManyRequests;
                    await ModifyHeader(context, statusCode, title, message);
                }
                // nếu response là Unauthorized (401)
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    title = "Thông báo";
                    message = "Bạn không có quyền truy cập vào tài nguyên này.";
                    statusCode = (int)StatusCodes.Status401Unauthorized;
                    await ModifyHeader(context, statusCode, title, message);
                }
                // nếu response là Forbidden (403)
                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    title = "Hết quyền truy cập";
                    message = "Bạn không có quyền/ không được truy cập vào tài nguyên này.";
                    statusCode = (int)StatusCodes.Status403Forbidden;
                    await ModifyHeader(context, statusCode, title, message);
                }
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here if needed// File/console/debug
                LogException.LogExceptions(ex);
                // if Exception is timeout / 408
                if(ex is TaskCanceledException || ex is TimeoutException)
                {
                    title = "Hết thời gian chờ";
                    message = "Yêu cầu của bạn đã bị hủy do quá thời gian chờ. Vui lòng thử lại sau.";
                    statusCode = StatusCodes.Status408RequestTimeout;
                }
                // nếu bắt được Exception 
                // if không có exceptions nào -> default
                await ModifyHeader(context, statusCode, title, ex.Message);
            }
        }

        private async Task ModifyHeader(HttpContext context, int statusCode, string title, string message)
        {
            //gui tin toi client
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
            {
                Detail = message,
                Status = statusCode,
                Title = title,
            }), CancellationToken.None);
            return;
        }
    }

}
