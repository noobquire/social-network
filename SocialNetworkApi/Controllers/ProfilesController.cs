using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Models;
using SocialNetworkApi.Services.Interfaces;
using SocialNetworkApi.Services.Models;
using SocialNetworkApi.Services.Models.Dtos;
using SocialNetworkApi.Services.Validation;
using System.Net;
using System.Threading.Tasks;

namespace SocialNetworkApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfilesController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IProfilesService _profilesService;
        public ProfilesController(IProfilesService profilesService, IAuthorizationService authorizationService)
        {
            _profilesService = profilesService;
            _authorizationService = authorizationService;
        }

        [HttpGet("{profileId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProfileDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiError))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiError))]
        public async Task<IActionResult> GetById([FromRoute][ValidateGuid] string profileId)
        {
            var profile = await _profilesService.GetByIdAsync(profileId);
            if (profile == null)
            {
                var error = new ApiError("Profile with such Id was not found.", HttpStatusCode.NotFound);
                return NotFound(error);
            }

            return Ok(profile);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProfileDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiError))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiError))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiError))]
        public async Task<IActionResult> UpdateProfile([FromBody][ValidateGuid] ProfileDto profile)
        {
            var storedProfile = await _profilesService.GetByIdAsync(profile.Id);
            var authResult = await _authorizationService.AuthorizeAsync(User, storedProfile, "SameUser");

            if (!authResult.Succeeded)
            {
                var authError = new ApiError("You are not permitted to update this profile.", HttpStatusCode.BadRequest);
                return StatusCode(StatusCodes.Status403Forbidden, authError);
            }

            var result = await _profilesService.UpdateAsync(profile);
            if (result)
            {
                return Ok();
            }

            var error = new ApiError("Profile with such Id was not found.", HttpStatusCode.NotFound);
            return NotFound(error);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResponse<ProfileDto>))]
        public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter)
        {
            return Ok(await _profilesService.GetAllAsync(filter));
        }
    }
}
