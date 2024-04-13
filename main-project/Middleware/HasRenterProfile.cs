using main_project.Models.ViewModels;
using main_project.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace main_project.Middleware;
public class HasRenterProfile : ActionFilterAttribute
{
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (!AccountService.Instance().HasRenterProfile(context.HttpContext))
        {
            context.Result = new RedirectToRouteResult(
                new RouteValueDictionary {
                    { "controller", "Account" },
                    { "action", "Index" }
                }
            );
        }
    }
}