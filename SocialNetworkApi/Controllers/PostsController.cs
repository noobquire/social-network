using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetworkApi.Attributes;
using SocialNetworkApi.Models;
using SocialNetworkApi.Services.Interfaces;
using SocialNetworkApi.Services.Models;

namespace SocialNetworkApi.Controllers
{
    [ApiController]
    [Authorize]
    public class PostsController : ControllerBase
    {
        private readonly IPostsService _postsService;
        private readonly IUsersService _usersService;
        private readonly IAuthorizationService _authorizationService;

        public PostsController(IPostsService postsService, IAuthorizationService authorizationService, IUsersService usersService)
        {
            _postsService = postsService;
            _authorizationService = authorizationService;
            _usersService = usersService;
        }

        [HttpPost]
        [Route("api/profiles/{profileId}/posts")]
        public async Task<IActionResult> CreatePost([FromRoute][ValidateGuid] string profileId,
            [FromBody, Required] PostDataModel data)
        {
            // TODO: Authorize if user can write posts in this profile
            // TODO: Add "profile lockout", so that only this user can write posts
            var post = await _postsService.CreateAsync(profileId, data);
            return CreatedAtAction(nameof(GetPostById), "Posts", new { profileId, postId = post.Id }, post);
        }

        [HttpGet]
        [Route("api/profiles/{profileId}/posts/{postId}")]
        public async Task<IActionResult> GetPostById([FromRoute][ValidateGuid] string postId)
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
        public async Task<IActionResult> GetPostsByProfile([FromRoute][ValidateGuid] string profileId)
        {
            var posts = await _postsService.GetByProfileAsync(profileId);

            return Ok(posts);
        }

        [HttpDelete]
        [Route("api/profiles/{profileId}/posts/{postId}")]
        public async Task<IActionResult> DeletePost([FromRoute][ValidateGuid] string postId)
        {
            var post = await _postsService.GetByIdAsync(postId);

            if (post == null)
            {
                var notFoundError = new ApiError("Post with such Id was not found.", HttpStatusCode.NotFound);
                return NotFound(notFoundError);
            }
            var profile = await _usersService.GetByIdAsync(post.AuthorId);

            var postAuthor = await _authorizationService.AuthorizeAsync(User, post, "SameUser");
            var profileOwner = await _authorizationService.AuthorizeAsync(User, profile, "SameOrAdminUser");

            if (!(postAuthor.Succeeded || profileOwner.Succeeded))
            {
                var authError = new ApiError("You are not permitted to delete this post.", HttpStatusCode.Unauthorized);
                return Unauthorized(authError);
            }

            await _postsService.DeleteByIdAsync(postId);

            return Ok();
        }

        [HttpPut]
        [Route("api/profiles/{profileId}/posts/{postId}")]
        public async Task<IActionResult> UpdatePost(
            [FromRoute][ValidateGuid] string postId,
            [FromBody, Required] PostDataModel data)
        {
            var post = await _postsService.GetByIdAsync(postId);

            if (post == null)
            {
                var notFoundError = new ApiError("Post with such Id was not found.", HttpStatusCode.NotFound);
                return NotFound(notFoundError);
            }

            var postAuthor = await _authorizationService.AuthorizeAsync(User, post, "SameUser");

            if (!postAuthor.Succeeded)
            {
                var authError = new ApiError("You are not permitted to update this post.", HttpStatusCode.Unauthorized);
                return Unauthorized(authError);
            }

            await _postsService.UpdateAsync(postId, data);

            return Ok();
        }
    }
}
