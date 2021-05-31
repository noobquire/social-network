using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SocialNetworkApi.Models;
using SocialNetworkApi.Services.Exceptions;
using SocialNetworkApi.Services.Interfaces;
using SocialNetworkApi.Services.Models;
using SocialNetworkApi.Services.Validation;

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
            catch (ItemNotFoundException)
            {
                return BadRequest(new ApiError("Participant list contains invalid user Id", HttpStatusCode.BadRequest));
            }
        }

        [HttpGet("{chatId}")]
        [Authorize]
        public async Task<IActionResult> GetChatById([FromRoute][ValidateGuid] string chatId)
        {
            var chat = await _chatsService.GetByIdAsync(chatId);

            if (chat == null)
            {
                return NotFound(new ApiError("Chat with such Id was not found", HttpStatusCode.NotFound));
            }

            return Ok(chat);
        }

        [HttpDelete("{chatId}")]
        [Authorize]
        public async Task<IActionResult> LeaveChat([FromRoute][ValidateGuid] string chatId)
        {
            var result = await _chatsService.LeaveChatAsync(chatId);

            if (!result)
            {
                return NotFound(new ApiError("Chat not found", HttpStatusCode.NotFound));
            }

            return Ok();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserChats()
        {
            var chats = await _chatsService.GetUserChats();

            return Ok(chats);
        }
    }
}
