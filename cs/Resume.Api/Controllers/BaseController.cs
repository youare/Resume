using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Resume.Api.Middlewares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Resume.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [ServiceFilter(typeof(AuditLog))]
    [ServiceFilter(typeof(DomainEventActionFilter))]
    public class BaseController:ControllerBase
    {
        public string CurrentUser => GetCurrentUser(HttpContext);
        public static string GetCurrentUser(HttpContext httpContext)
        {
            var result = "AnonymouseUser";
            if(!(httpContext.User is null))
            {
                if (!string.IsNullOrEmpty(httpContext.User.Identity?.Name)) return httpContext.User.Identity.Name;
                var names = httpContext.User.Claims.Where(x => x.Type == "name");
                if (names.Any()) return names.First().Value;
            }
            return result;
        }
    }
}
