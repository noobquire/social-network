using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using SocialNetworkApi.Models;
using SocialNetworkApi.Services.Interfaces;

namespace SocialNetworkApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfilesController : ControllerBase
    {
        private readonly IProfilesService _profilesService;
        public ProfilesController(IProfilesService profilesService)
        {
            _profilesService = profilesService;
        }

        [HttpGet("{profileId}")]
        public async Task<IActionResult> GetById([FromRoute] string profileId)
        {
            var profile = await _profilesService.GetByIdAsync(profileId);
            if(profile == null)
            {
                var error = new ApiError("Profile with such Id was not found.", HttpStatusCode.NotFound);
                return NotFound(error);
            }

            return Ok(profile);
        }
    }
}
