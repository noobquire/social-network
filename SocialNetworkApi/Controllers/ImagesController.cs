using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Models;
using SocialNetworkApi.Services.Exceptions;
using SocialNetworkApi.Services.Interfaces;
using SocialNetworkApi.Services.Models;
using SocialNetworkApi.Services.Models.Dtos;
using SocialNetworkApi.Services.Validation;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;

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
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ImageHeaderDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadImage(
            [AllowedExtensions, MaxFileSize((int)10E6), Required] IFormFile image)
        {
            var createdImage = await _imagesService.UploadAsync(image);
            return CreatedAtAction("GetImageById", new { imageId = createdImage.Id }, createdImage);
        }

        [HttpGet("{imageId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ImageDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiError))]
        public async Task<IActionResult> GetImageById([FromRoute][ValidateGuid] string imageId)
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ImageHeaderDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiError))]
        public async Task<IActionResult> GetImageHeaderById([FromRoute][ValidateGuid] string imageId)
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ApiError))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiError))]
        public async Task<IActionResult> DeleteImageById([FromRoute][ValidateGuid] string imageId)
        {
            var image = await _imagesService.GetByIdAsync(imageId);
            if (image == null)
            {
                var notFoundError = new ApiError("Image with such Id was not found.", HttpStatusCode.NotFound);
                return NotFound(notFoundError);
            }
            var authResult = await _authorizationService.AuthorizeAsync(User, image, "SameOrAdminUser");

            if (!authResult.Succeeded)
            {
                var authError = new ApiError("You are not permitted to delete this image.", HttpStatusCode.Unauthorized);
                return StatusCode(StatusCodes.Status403Forbidden, authError);
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResponse<ImageDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiError))]
        public async Task<IActionResult> GetUserImages([FromRoute][ValidateGuid] string userId, [FromQuery] PaginationFilter filter)
        {
            try
            {
                var images = await _imagesService.GetByUserAsync(userId, filter);
                return Ok(images);
            }
            catch (ItemNotFoundException)
            {
                var notFoundError = new ApiError("User with such Id was not found.", HttpStatusCode.NotFound);
                return NotFound(notFoundError);
            }
        }

        [HttpGet]
        [Route("/api/users/{userId}/images/headers")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResponse<ImageHeaderDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiError))]
        public async Task<IActionResult> GetUserImageHeaders([FromRoute][ValidateGuid] string userId, [FromQuery] PaginationFilter filter)
        {
            try
            {
                var headers = await _imagesService.GetHeadersByUserAsync(userId, filter);
                return Ok(headers);
            }
            catch (ItemNotFoundException)
            {
                var notFoundError = new ApiError("User with such Id was not found.", HttpStatusCode.NotFound);
                return NotFound(notFoundError);
            }
        }
    }
}
