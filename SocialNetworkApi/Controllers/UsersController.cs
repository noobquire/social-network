using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
                if (result.Succeeded)
                {
                    return Ok();
                }

                var message = string.Join(" ", result.Errors.Select(e => e.Description));
                var error = new ApiError(message, HttpStatusCode.BadRequest);
                return BadRequest(error);
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

        [HttpDelete("{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteById([FromRoute]string userId)
        {
            var result = await _usersService.DeleteByIdAsync(userId);
            if (result)
            {
                return Ok();
            }

            var error = new ApiError("User with such Id was not found.", HttpStatusCode.NotFound);
            return NotFound(error);
        }

        [HttpGet("{userId}")]
        [Authorize]
        public async Task<IActionResult> GetById([FromRoute] string userId)
        {
            var user = await _usersService.GetByIdAsync(userId);
            if (user == null)
            {
                var error = new ApiError("User with such Id was not found.", HttpStatusCode.NotFound);
                return NotFound(error);
            }

            return Ok(user);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll([FromQuery]bool withDeleted = false)
        {
            var users = await _usersService.GetAllAsync();

            return Ok(users);
        }

        [HttpGet("{userId}/reinstate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reinstate([FromRoute]string userId)
        {
            var result = await _usersService.Reinstate(userId);
            if (!result)
            {
                var error = new ApiError("Unable to reinstate user with such Id.", HttpStatusCode.NotFound);
            }

            return Ok();
        }
    }
}
