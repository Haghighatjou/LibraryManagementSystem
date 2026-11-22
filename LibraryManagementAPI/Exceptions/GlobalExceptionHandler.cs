using LibraryManagement.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace LibraryManagementAPI.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            this._logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            int statusCode;
            switch(exception)
            {
                case MemberNotFoundException:
                case BookNotFoundException:
                    statusCode = 404;
                    break;

                case MemberInactiveException:
                case BookNotAvailableException:
                    statusCode = 400;
                    break;

                case LastAdminCannotBeDemotedException:
                    statusCode = 409;
                    break;

                default:
                    statusCode = 500;
                    _logger.LogError(exception, "An unexpected error occurred.");
                    break;
            };
            
            httpContext.Response.StatusCode= statusCode;

            await httpContext.Response.WriteAsJsonAsync(
                new
                {
                    Message = exception.Message
                },
                cancellationToken);

            return true;
        }
    }
}
