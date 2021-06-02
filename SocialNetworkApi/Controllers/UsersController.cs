using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Models;
using SocialNetworkApi.Services.Exceptions;
using SocialNetworkApi.Services.Interfaces;
using SocialNetworkApi.Services.Models;
using SocialNetworkApi.Services.Models.Dtos;
using SocialNetworkApi.Services.Validation;

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

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(JwtToken))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ApiError))]
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
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiError))]
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

        [HttpDelete("{userId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiError))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiError))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiError))]
        public async Task<IActionResult> DeleteById([FromRoute][ValidateGuid] string userId)
        {
            var user = await _usersService.GetByIdAsync(userId);

            if(user == null)
            {
                var error = new ApiError("User with such Id was not found.", HttpStatusCode.NotFound);
                return NotFound(error);
            }

            var authResult = await _authorizationService.AuthorizeAsync(User, user, "SameOrAdminUser");

            if(!authResult.Succeeded)
            {
                var authError = new ApiError("You are not permitted to delete this user.", HttpStatusCode.BadRequest);
                return StatusCode(StatusCodes.Status403Forbidden, authError);
            }

            await _usersService.DeleteByIdAsync(userId);
            return Ok();
        }

        [HttpGet("{userId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiError))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiError))]
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
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResponse<UserDto>))]
        public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter)
        {
            var users = await _usersService.GetAllAsync(filter);

            return Ok(users);
        }

        [HttpGet("{userId}/reinstate")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResponse<UserDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiError))]
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
