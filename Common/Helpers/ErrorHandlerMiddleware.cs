using System.Net;
using System.Text.Json;

namespace ContactAPI.Common.Helpers
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);

                // Check for 404 status code
                if (context.Response.StatusCode == (int)HttpStatusCode.NotFound)
                {
                    var response = context.Response;
                    response.ContentType = "application/json";
                    var result = JsonSerializer.Serialize(new
                    {
                        status = (int)HttpStatusCode.NotFound,
                        message = "Resource not found"
                    });
                    await response.WriteAsync(result);
                }

                if (context.Response.StatusCode == (int)HttpStatusCode.BadRequest)
                {
                    var response = context.Response;
                    response.ContentType = "application/json";
                    var result = JsonSerializer.Serialize(new
                    {
                        status = (int)HttpStatusCode.NotFound,
                        message = "Bad Request"
                    });
                    await response.WriteAsync(result);
                }
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                int statusCode;

                switch (error)
                {
                    case AppException e:
                        // custom application error
                        statusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case KeyNotFoundException e:
                        // not found error
                        statusCode = (int)HttpStatusCode.NotFound;
                        break;
                    default:
                        // unhandled error
                        _logger.LogError(error, error.Message);
                        statusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                response.StatusCode = statusCode;
                var result = JsonSerializer.Serialize(new
                {
                    status = statusCode,
                    message = error?.Message
                });
                await response.WriteAsync(result);

            }
        }
    }
}
