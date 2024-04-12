using main_project.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace main_project.Middleware;
public class Guest : ActionFilterAttribute
{
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (AuthService.IsAuthenticated(context.HttpContext))
        {
            context.Result = new RedirectToRouteResult(
                new RouteValueDictionary {
                    { "controller", "Home" },
                    { "action", "Index" }
                }
            );
        }
    }
}