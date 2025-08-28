using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FlixTv.Clients.WebApp.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class RedirectAuthenticatedAttribute : ActionFilterAttribute
    {
        private readonly string _action;
        private readonly string _controller;

        public RedirectAuthenticatedAttribute(string action = "Index", string controller = "Home")
        {
            _action = action;
            _controller = controller;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                context.Result = new RedirectToActionResult(_action, _controller, null);
                return;
            }
            base.OnActionExecuting(context);
        }
    }
}
