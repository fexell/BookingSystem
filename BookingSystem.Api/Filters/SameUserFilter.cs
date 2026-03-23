

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace BookingSystem.Api.Filters;

// Om användaren försöker komma åt en resurs som inte tillhör dom, så ska dom inte få göra det
public class SameUserFilter : IActionFilter {
    public void OnActionExecuting(ActionExecutingContext context) {
        var routeUserId = context.RouteData.Values[ "userId" ]?.ToString();
        var tokenUserId = context.HttpContext.User.FindFirstValue( ClaimTypes.NameIdentifier );

        if(routeUserId != tokenUserId) {
            context.Result = new ForbidResult();
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}