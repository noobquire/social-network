using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SocialNetworkApi.Attributes;
using SocialNetworkApi.Models;
using SocialNetworkApi.Services.Exceptions;
using SocialNetworkApi.Services.Interfaces;
using SocialNetworkApi.Services.Models;

namespace SocialNetworkApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly IChatsService _chatsService;

        public ChatsController(IChatsService chatsService)
        {
            _chatsService = chatsService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateGroupChat([FromBody][Required] NewChatModel newChat)
        {
            try
            {
                var chat = await _chatsService.CreateGroupAsync(newChat);
                return CreatedAtAction(nameof(GetChatById), new {chatId = chat.Id}, chat);
            }
            catch (DuplicateChatParticipantException)
            {
                return BadRequest(new ApiError("Participant list contains duplicates", HttpStatusCode.BadRequest));
            }
        }

        [HttpGet("{chatId}")]
        [Authorize]
        public async Task<IActionResult> GetChatById([FromRoute][ValidateGuid] string chatId)
        {
            var chat = await _chatsService.GetByIdAsync(chatId);
            return Ok(chat);
        }
    }
}
