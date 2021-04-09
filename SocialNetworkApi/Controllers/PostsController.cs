using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetworkApi.Models;
using SocialNetworkApi.Services.Interfaces;
using SocialNetworkApi.Services.Models;

namespace SocialNetworkApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostsService _postsService;
        private readonly IAuthorizationService _authorizationService;

        public PostsController(IPostsService postsService, IAuthorizationService authorizationService)
        {
            _postsService = postsService;
            _authorizationService = authorizationService;
        }

        [HttpPost]
        [Route("api/profiles/{profileId}/posts")]
        [Authorize]
        public async Task<IActionResult> CreatePost([FromRoute] string profileId, [FromBody, Required] PostDataModel data)
        {
            // TODO: Authorize if user can write posts in this profile
            var post = await _postsService.CreateAsync(profileId, data);
            return CreatedAtAction("GetPostById", new {postId = post.Id}, post);
        }

        [HttpGet]
        [Route("api/profiles/{profileId}/posts/{postId}")]
        [Authorize]
        public async Task<IActionResult> GetPostById([FromRoute] string postId)
        {
            var post = await _postsService.GetByIdAsync(postId);
            if (post == null)
            {
                var error = new ApiError("Post with such Id was not found.", HttpStatusCode.NotFound);
                return NotFound(error);
            }

            return Ok(post);
        }

        [HttpGet]
        [Route("api/profiles/{profileId}/posts")]
        [Authorize]
        public async Task<IActionResult> GetPostsByProfile([FromRoute] string profileId)
        {
            var posts = await _postsService.GetByProfileAsync(profileId);

            return Ok(posts);
        }

        [HttpDelete]
        [Route("api/profiles/{profileId}/posts/{postId}")]
        [Authorize]
        public async Task<IActionResult> DeletePost([FromRoute] string postId)
        {
            var post = _postsService.GetByIdAsync(postId);

            if (post == null)
            {
                var notFoundError = new ApiError("Image with such Id was not found.", HttpStatusCode.NotFound);
                return NotFound(notFoundError);
            }
            var authResult = await _authorizationService.AuthorizeAsync(User, post, "SameUserPolicy");

            if (!authResult.Succeeded)
            {
                var authError = new ApiError("You are not permitted to delete this post.", HttpStatusCode.BadRequest);
                return Unauthorized(authError);
            }

            await _postsService.DeleteByIdAsync(postId);

            return Ok();
        }

        [HttpPut]
        [Route("api/profiles/{profileId}/posts/{postId}")]
        [Authorize]
        public async Task<IActionResult> UpdatePost(
            [FromRoute] string postId,
            [FromBody, Required] PostDataModel data)
        {
            var post = _postsService.GetByIdAsync(postId);

            if (post == null)
            {
                var notFoundError = new ApiError("Image with such Id was not found.", HttpStatusCode.NotFound);
                return NotFound(notFoundError);
            }
            var authResult = await _authorizationService.AuthorizeAsync(User, post, "SameUserPolicy");

            if (!authResult.Succeeded)
            {
                var authError = new ApiError("You are not permitted to delete this post.", HttpStatusCode.BadRequest);
                return Unauthorized(authError);
            }

            await _postsService.UpdateAsync(postId, data);

            return Ok();
        }
    }
}
