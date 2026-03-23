

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BookingSystem.Api.Filters;

// Kollar om användaren är inloggad
public class NotLoggedInFilter : IActionFilter {
    public void OnActionExecuting(ActionExecutingContext context) {
        if(context.HttpContext.User.Identity?.IsAuthenticated == true)
            context.Result = new ConflictObjectResult( "User is already logged in" );
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}