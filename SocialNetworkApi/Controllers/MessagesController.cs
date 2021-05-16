﻿using System;
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
        public async Task<IActionResult> SendGroupMessage([Required][ValidateGuid][FromRoute] string chatId,
            [Required][FromBody] MessageDataModel messageData)
        {
            // TODO: Validate if user is chat participant
            try
            {
                var message = await _messagesService.SendGroupMessageAsync(chatId, messageData);
                return CreatedAtAction(nameof(GetMessageById), new { messageId = message.Id, chatId }, message);
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
            // TODO: Validate if user is chat participant
            var message = await _messagesService.GetMessageByIdAsync(messageId);
            if (message == null)
            {
                return NotFound("Message not found");
            }

            return Ok(message);
        }

        [HttpGet("/api/chats/{chatId}/messages")]
        public async Task<IActionResult> GetGroupMessages([Required][ValidateGuid][FromRoute] string chatId)
        {
            // TODO: Validate if user is chat participant
            var messages = await _messagesService.GetGroupMessagesAsync(chatId);

            return Ok(messages);
        }

        [HttpGet("/api/users/{userId}/messages")]
        public async Task<IActionResult> GetPersonalMessages([Required][ValidateGuid][FromRoute] string userId)
        {
            var messages = await _messagesService.GetPersonalMessagesAsync(userId);

            return Ok(messages);
        }

        [HttpDelete]
        [Route("/api/chats/{chatId}/messages/{messageId}")]
        [Route("/api/users/{userId}/messages/{messageId}")]
        public async Task<IActionResult> DeleteMessage([Required][ValidateGuid][FromRoute] string messageId)
        {
            // TODO: Validate if user is message author, or is chat admin, or is social network admin
            var message = await _messagesService.GetMessageByIdAsync(messageId);
            if (message == null)
            {
                return NotFound(new ApiError("Message with specified Id was not found", HttpStatusCode.NotFound));
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
            // TODO: Validate if user is message author
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
