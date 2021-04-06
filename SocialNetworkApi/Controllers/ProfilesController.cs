using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SocialNetworkApi.Models;
using SocialNetworkApi.Services.Interfaces;
using SocialNetworkApi.Services.Models.Dtos;

namespace SocialNetworkApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        [Authorize]
        public async Task<IActionResult> GetById([FromRoute] string profileId)
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
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] ProfileDto profile)
        {
            var storedProfile = await _profilesService.GetByIdAsync(profile.Id);
            var authResult = await _authorizationService.AuthorizeAsync(User, storedProfile, "SameUserPolicy");

            if (!authResult.Succeeded)
            {
                var authError = new ApiError("You are not permitted to update this profile.", HttpStatusCode.BadRequest);
                return Unauthorized(authError);
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
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _profilesService.GetAllAsync());
        }
    }
}
