using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SocialNetworkApi.Models;
using SocialNetworkApi.Services.Attributes;
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
        private readonly IAuthorizationService _authorizationService;

        public UsersController(IUsersService usersService, IAuthorizationService authorizationService)
        {
            _usersService = usersService;
            _authorizationService = authorizationService;
        }

        // /api/users/login
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
                return CreatedAtAction(nameof(GetById), new { userId = result.Id }, result);
            }
            catch (RegisterUserException ex)
            {
                var error = new ApiError(ex.Message, HttpStatusCode.BadRequest);
                return BadRequest(error);
            }
        }

        // DELETE /api/users/user-id?
        [HttpDelete("{userId}")]
        [Authorize]
        public async Task<IActionResult> DeleteById([FromRoute][ValidateGuid] string userId)
        {
            var user = await _usersService.GetByIdAsync(userId);
            var authResult = await _authorizationService.AuthorizeAsync(User, user, "SameOrAdminUser");

            if(!authResult.Succeeded)
            {
                var authError = new ApiError("You are not permitted to delete this user.", HttpStatusCode.BadRequest);
                return Unauthorized(authError);
            }

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
        public async Task<IActionResult> GetById([FromRoute][ValidateGuid] string userId)
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
        public async Task<IActionResult> GetAll([FromQuery][ValidateGuid] bool withDeleted = false)
        {
            var users = await _usersService.GetAllAsync(withDeleted);

            return Ok(users);
        }

        [HttpGet("{userId}/reinstate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reinstate([FromRoute][ValidateGuid] string userId)
        {
            var result = await _usersService.ReinstateAsync(userId);
            if (!result)
            {
                var error = new ApiError("Unable to reinstate user with such Id.", HttpStatusCode.NotFound);
                return NotFound(error);
            }

            return Ok();
        }
    }
}
