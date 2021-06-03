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
    [Authorize]
    public class ChatsController : ControllerBase
    {
        private readonly IChatsService _chatsService;

        public ChatsController(IChatsService chatsService)
        {
            _chatsService = chatsService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ChatDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiError))]
        public async Task<IActionResult> CreateGroupChat([FromBody][Required] NewChatModel newChat)
        {
            try
            {
                var chat = await _chatsService.CreateGroupAsync(newChat);
                return CreatedAtAction(nameof(GetChatById), new { chatId = chat.Id }, chat);
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChatDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiError))]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiError))]
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResponse<ChatDto>))]
        public async Task<IActionResult> GetUserChats([FromQuery] PaginationFilter filter)
        {
            var chats = await _chatsService.GetUserChats(filter);

            return Ok(chats);
        }
    }
}
