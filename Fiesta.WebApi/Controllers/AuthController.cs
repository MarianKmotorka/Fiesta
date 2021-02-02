using System.Threading;
using System.Threading.Tasks;
using Fiesta.Application.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Fiesta.WebApi.Controllers
{
    [Route("api/auth")]
    public class AuthController : BaseController
    {
        [HttpGet]
        public async Task<ActionResult<string>> Get([FromQuery] Greeting.Command request, CancellationToken ct)
        {
            var response = await Mediator.Send(request, ct);
            return Ok(response);
        }
    }
}
