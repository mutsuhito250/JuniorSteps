using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class SimpleAuthFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.Session;
        var isLoggedIn = session.GetString("admin_pass") == "baba123";

        var path = context.HttpContext.Request.Path;

        if (!isLoggedIn && path == "/secret-url")
        {
            context.Result = new RedirectToActionResult("Login", "Access", null);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}
