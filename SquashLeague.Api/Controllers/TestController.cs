using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SquashLeague.Api.Extensions;

namespace SquashLeague.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("testaction")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Authorize(Policy = "IsAdmin")]
        public ActionResult TestAction()
        {
            var xxx = HttpContext.GetUserId();

            return Ok("test");
        }
    }
}
