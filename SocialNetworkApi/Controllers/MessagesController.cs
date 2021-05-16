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

        public MessagesController(IMessagesService messagesService)
        {
            _messagesService = messagesService;
        }

        [HttpPost("/api/chats/{chatId}/messages")]
        public async Task<IActionResult> SendMessage([Required][ValidateGuid][FromRoute] string chatId,
            [Required][FromBody] MessageDataModel messageData)
        {
            try
            {
                var message = await _messagesService.SendMessageAsync(chatId, messageData);
                return CreatedAtAction(nameof(GetMessageById), new { messageId = message.Id, chatId }, message);
            }
            catch (ItemNotFoundException e)
            {
                return BadRequest(new ApiError(e.Message, HttpStatusCode.BadRequest));
            }
        }

        [HttpGet("/api/chats/{chatId}/messages/{messageId}")]
        public async Task<IActionResult> GetMessageById([Required][ValidateGuid][FromRoute] string messageId)
        {
            var message = await _messagesService.GetMessageByIdAsync(messageId);
            if (message == null)
            {
                return NotFound("Message not found");
            }

            return Ok(message);
        }

        [HttpGet("/api/chats/{chatId}/messages")]
        public async Task<IActionResult> GetChatMessages([Required][ValidateGuid][FromRoute] string chatId)
        {
            var messages = await _messagesService.GetChatMessagesAsync(chatId);

            return Ok(messages);
        }

        [HttpDelete("/api/chats/{chatId}/messages/{messageId}")]
        public async Task<IActionResult> DeleteMessage([Required][ValidateGuid][FromRoute] string messageId)
        {
            var message = await _messagesService.GetMessageByIdAsync(messageId);
            if (message == null)
            {
                return NotFound(new ApiError("Message with specified Id was not found", HttpStatusCode.NotFound));
            }

            await _messagesService.DeleteMessageAsync(messageId);

            return Ok();
        }

        [HttpPut("/api/chats/{chatId}/messages/{messageId}")]
        public async Task<IActionResult> EditMessage([Required][ValidateGuid][FromRoute] string messageId,
            [Required][FromBody] MessageDataModel messageData)
        {
            var message = await _messagesService.GetMessageByIdAsync(messageId);
            if (message == null)
            {
                return NotFound(new ApiError("Message with specified Id was not found", HttpStatusCode.NotFound));
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
