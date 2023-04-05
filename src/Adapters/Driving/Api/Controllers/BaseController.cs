using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize(Roles = "Administrator,WM")]
    public class BaseController : ControllerBase
    {
        protected string GetUserName()
        {
            var claimsIdentity = HttpContext?.User?.Identity == null ? throw new NullReferenceException() : (ClaimsIdentity)HttpContext.User.Identity;

            return claimsIdentity.FindFirst(p => p.Type == "preferred_username")?.Value ?? "";
        }
    }
}
