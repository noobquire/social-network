using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using SocialNetworkApi.Models;
using SocialNetworkApi.Services.Exceptions;
using SocialNetworkApi.Services.Interfaces;
using SocialNetworkApi.Services.Models;

namespace SocialNetworkApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;
        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            var token = await _usersService.LoginAsync(login);
            if (token == null)
            {
                var error = new ApiError("Invalid credentials, please try again.", HttpStatusCode.Unauthorized);
                return Unauthorized(error);
            }

            return Ok(token);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterModel registerModel)
        {
            try
            {
                var result = await _usersService.RegisterAsync(registerModel);
                if (!result.Succeeded)
                {
                    var message = string.Join(" ", result.Errors.Select(e => e.Description));
                    var error = new ApiError(message, HttpStatusCode.BadRequest);
                    return BadRequest(error);
                }
                return Ok();
            }
            catch (UserAlreadyExistsException userExistsEx)
            {
                var error = new ApiError(userExistsEx.Message, HttpStatusCode.BadRequest);
                return BadRequest(error);
            }
            catch (RegisterUserException registerEx)
            {
                var error = new ApiError(registerEx.Message, HttpStatusCode.BadRequest);
                return BadRequest(error);
            }
        }
    }
}
