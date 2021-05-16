using System;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SocialNetworkApi.Models;
using SocialNetworkApi.Services.Exceptions;
using SocialNetworkApi.Services.Interfaces;
using SocialNetworkApi.Services.Models;
using SocialNetworkApi.Services.Validation;

namespace SocialNetworkApi.Controllers
{
    [ApiController]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IMessagesService _messagesService;
        private readonly IChatsService _chatsService;
        private readonly IAuthorizationService _authorizationService;

        public MessagesController(IMessagesService messagesService, IAuthorizationService authorizationService, IChatsService chatsService)
        {
            _messagesService = messagesService;
            _authorizationService = authorizationService;
            _chatsService = chatsService;
        }

        [HttpPost("/api/chats/{chatId}/messages")]
        public async Task<IActionResult> SendGroupMessage([Required][ValidateGuid][FromRoute] string chatId,
            [Required][FromBody] MessageDataModel messageData)
        {
            var chat = await _chatsService.GetByIdAsync(chatId);
            var authResult = await _authorizationService.AuthorizeAsync(User, chat, "ChatParticipant");

            if (!authResult.Succeeded)
            {
                return BadRequest(new ApiError("Chat not found", HttpStatusCode.BadRequest));
            }

            try
            {
                var message = await _messagesService.SendGroupMessageAsync(chatId, messageData);
                return CreatedAtAction(nameof(GetMessageById), new { messageId = message.Id, chatId }, message);
            }
            catch (ItemNotFoundException)
            {
                return BadRequest(new ApiError("Chat not found", HttpStatusCode.BadRequest));
            }
            catch (InvalidOperationException)
            {
                return BadRequest(new ApiError("Chat is not a group", HttpStatusCode.BadRequest));
            }
        }

        [HttpPost("/api/users/{userId}/messages")]
        public async Task<IActionResult> SendPersonalMessage([Required][ValidateGuid][FromRoute] string userId,
            [Required][FromBody] MessageDataModel messageData)
        {
            try
            {
                var message = await _messagesService.SendPersonalMessageAsync(userId, messageData);
                return CreatedAtAction(nameof(GetMessageById), new { messageId = message.Id, userId }, message);
            }
            catch (ItemNotFoundException e)
            {
                return BadRequest(new ApiError(e.Message, HttpStatusCode.BadRequest));
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(new ApiError(e.Message, HttpStatusCode.BadRequest));
            }
        }

        [HttpGet]
        [Route("/api/chats/{chatId}/messages/{messageId}")]
        [Route("/api/users/{userId}/messages/{messageId}")]
        public async Task<IActionResult> GetMessageById([Required][ValidateGuid][FromRoute] string messageId)
        {
            var message = await _messagesService.GetMessageByIdAsync(messageId);

            if (message == null)
            {
                return NotFound(new ApiError("Message not found", HttpStatusCode.NotFound));
            }

            var chat = await _chatsService.GetByIdAsync(message.ChatId);
            var authResult = await _authorizationService.AuthorizeAsync(User, chat, "ChatParticipant");

            if (!authResult.Succeeded)
            {
                return NotFound(new ApiError("Message not found", HttpStatusCode.NotFound));
            }

            return Ok(message);
        }

        [HttpGet("/api/chats/{chatId}/messages")]
        public async Task<IActionResult> GetGroupMessages([Required][ValidateGuid][FromRoute] string chatId)
        {
            var chat = await _chatsService.GetByIdAsync(chatId);
            var authResult = await _authorizationService.AuthorizeAsync(User, chat, "ChatParticipant");

            if (!authResult.Succeeded)
            {
                return NotFound(new ApiError("Chat not found", HttpStatusCode.NotFound));
            }

            try
            {
                var messages = await _messagesService.GetGroupMessagesAsync(chatId);
                return Ok(messages);
            }
            catch (ItemNotFoundException)
            {
                return NotFound(new ApiError("Chat not found", HttpStatusCode.NotFound));
            }
        }

        [HttpGet("/api/users/{userId}/messages")]
        public async Task<IActionResult> GetPersonalMessages([Required][ValidateGuid][FromRoute] string userId)
        {
            try
            {
                var messages = await _messagesService.GetPersonalMessagesAsync(userId);
                return Ok(messages);
            }
            catch (ItemNotFoundException e)
            {
                return NotFound(new ApiError(e.Message, HttpStatusCode.NotFound));
            }
        }

        [HttpDelete]
        [Route("/api/chats/{chatId}/messages/{messageId}")]
        [Route("/api/users/{userId}/messages/{messageId}")]
        public async Task<IActionResult> DeleteMessage([Required][ValidateGuid][FromRoute] string messageId)
        {
            var message = await _messagesService.GetMessageByIdAsync(messageId);

            if (message == null)
            {
                return NotFound(new ApiError("Message with specified Id was not found", HttpStatusCode.NotFound));
            }

            var chat = await _chatsService.GetByIdAsync(message.ChatId);

            var isChatParticipant = await _authorizationService.AuthorizeAsync(User, chat, "ChatParticipant");

            if (!isChatParticipant.Succeeded)
            {
                return NotFound(new ApiError("Message with specified Id was not found", HttpStatusCode.NotFound));
            }

            var authResult = await _authorizationService.AuthorizeAsync(User, message, "SameOrAdminUser");

            if (!authResult.Succeeded)
            {
                var authError = new ApiError("You are not message author or chat admin", HttpStatusCode.Unauthorized);
                return Unauthorized(authError);
            }

            await _messagesService.DeleteMessageAsync(messageId);

            return Ok();
        }

        [HttpPut]
        [Route("/api/chats/{chatId}/messages/{messageId}")]
        [Route("/api/users/{userId}/messages/{messageId}")]
        public async Task<IActionResult> EditMessage([Required][ValidateGuid][FromRoute] string messageId,
            [Required][FromBody] MessageDataModel messageData)
        {
            var message = await _messagesService.GetMessageByIdAsync(messageId);

            if (message == null)
            {
                return NotFound(new ApiError("Message with specified Id was not found", HttpStatusCode.NotFound));
            }

            var chat = await _chatsService.GetByIdAsync(message.ChatId);

            var isChatParticipant = await _authorizationService.AuthorizeAsync(User, chat, "ChatParticipant");

            if (!isChatParticipant.Succeeded)
            {
                return NotFound(new ApiError("Message with specified Id was not found", HttpStatusCode.NotFound));
            }

            var authResult = await _authorizationService.AuthorizeAsync(User, message, "SameUser");

            if (!authResult.Succeeded)
            {
                var authError = new ApiError("You are not message author", HttpStatusCode.Unauthorized);
                return Unauthorized(authError);
            }

            try
            {
                await _messagesService.EditMessageAsync(messageId, messageData);
            }
            catch (ItemNotFoundException e)
            {
                return BadRequest(new ApiError(e.Message, HttpStatusCode.BadRequest));
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(new ApiError(e.Message, HttpStatusCode.BadRequest));
            }

            return Ok(message);
        }
    }
}