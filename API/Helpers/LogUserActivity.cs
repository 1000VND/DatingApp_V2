using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //Đợi cho API trả ra kết quả xong mới thực hiện các hành động tiếp theo
            var resultContext = await next();

            if(!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            var userId = resultContext.HttpContext.User.GetUserId();

            var repo = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
            
            var user = await repo.GetUserByIdAsync(userId);

            user.LastActive = DateTime.Now;

            await repo.SaveAllAsync();
        }
    }
}
