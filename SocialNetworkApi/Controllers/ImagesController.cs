using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SocialNetworkApi.Models;
using SocialNetworkApi.Services.Interfaces;
using SocialNetworkApi.Services.Validation;

namespace SocialNetworkApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImagesService _imagesService;
        private readonly IAuthorizationService _authorizationService;

        public ImagesController(IImagesService imagesService, IAuthorizationService authorizationService)
        {
            _imagesService = imagesService;
            _authorizationService = authorizationService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadImage(
            [AllowedExtensions, MaxFileSize((int)10E6), Required] IFormFile image)
        {
            var createdImage = await _imagesService.UploadAsync(image);
            return CreatedAtAction("GetImageById", new { imageId = createdImage.Id }, createdImage);
        }

        [HttpGet("{imageId}")]
        [Authorize]
        public async Task<IActionResult> GetImageById([FromRoute] string imageId)
        {
            var image = await _imagesService.GetByIdAsync(imageId);
            if (image == null)
            {
                var error = new ApiError("Image with such Id was not found.", HttpStatusCode.NotFound);
                return NotFound(error);
            }

            return Ok(image);
        }

        [HttpGet("{imageId}/header")]
        [Authorize]
        public async Task<IActionResult> GetImageHeaderById([FromRoute] string imageId)
        {
            var imageHeader = await _imagesService.GetHeaderByIdAsync(imageId);
            if (imageHeader == null)
            {
                var error = new ApiError("Image with such Id was not found.", HttpStatusCode.NotFound);
                return NotFound(error);
            }

            return Ok(imageHeader);
        }

        [HttpDelete("{imageId}")]
        [Authorize]
        public async Task<IActionResult> DeleteImageById([FromRoute] string imageId)
        {
            var image = await _imagesService.GetByIdAsync(imageId);
            if(image == null)
            {
                var notFoundError = new ApiError("Image with such Id was not found.", HttpStatusCode.NotFound);
                return NotFound(notFoundError);
            }
            var authResult = await _authorizationService.AuthorizeAsync(User, image, "SameOrAdminUser");

            if (!authResult.Succeeded)
            {
                var authError = new ApiError("You are not permitted to delete this image.", HttpStatusCode.Unauthorized);
                return Unauthorized(authError);
            }

            var result = await _imagesService.DeleteByIdAsync(imageId);
            if (result)
            {
                return Ok();
            }

            var error = new ApiError("Error deleting image.", HttpStatusCode.InternalServerError);

            return StatusCode(500, error);
        }

        [HttpGet]
        [Route("/api/users/{userId}/images")]
        [Authorize]
        public async Task<IActionResult> GetUserImages([FromRoute] string userId)
        {
            return Ok(await _imagesService.GetByUserAsync(userId));
        }

        [HttpGet]
        [Route("/api/users/{userId}/images/headers")]
        [Authorize]
        public async Task<IActionResult> GetUserImageHeaders([FromRoute] string userId)
        {
            return Ok(await _imagesService.GetHeadersByUserAsync(userId));
        }
    }
}
