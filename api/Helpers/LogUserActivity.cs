using System;
using System.Threading.Tasks;
using api.Extensions;
using api.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace api.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            var userId = resultContext.HttpContext.User.GetUserId();
            var rep = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();
            var user = await rep.GetUserByIdAsync(userId);
            user.LastActive = DateTime.UtcNow;
            await rep.SaveAllAsync();
        }
    }
}