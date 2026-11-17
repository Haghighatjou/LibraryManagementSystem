
using LibraryManagement.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    public class ErrorController:ControllerBase
    {
        [AcceptVerbs("GET", "POST", "PUT", "DELETE", "PATCH")]
        [Route("/error")]
        public IActionResult HandleError()
        {
            var exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

            
            if (exception is BookNotFoundException)
            {
                return NotFound(new
                {
                    message = exception.Message
                });
            }

            if(exception is BookNotAvailableException)
            {
                return BadRequest(new
                {
                    message = exception.Message
                });
            }

            if(exception is BookHasLoanHistoryException)
            {
                return BadRequest(new
                {
                    message = exception.Message
                });
            }

            return StatusCode(500, new
            {
                message = "Internal server error."
            });

        }
    }
}
