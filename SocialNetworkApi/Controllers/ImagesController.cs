using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public ImagesController(IImagesService imagesService)
        {
            _imagesService = imagesService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadImage(
            [AllowedExtensions, MaxFileSize((int)10E6)] IFormFile image)
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
            var result = await _imagesService.DeleteByIdAsync(imageId);
            if (result)
            {
                return Ok();
            }

            var error = new ApiError("Image with such Id was not found.", HttpStatusCode.NotFound);
            return NotFound(error);
        }
    }
}
