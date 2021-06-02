using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SocialNetworkApi.Data.Interfaces;
using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Services.Exceptions;
using SocialNetworkApi.Services.Extensions;
using SocialNetworkApi.Services.Interfaces;
using SocialNetworkApi.Services.Models;
using SocialNetworkApi.Services.Models.Dtos;

namespace SocialNetworkApi.Services.Implementations
{
    public class MessagesService : IMessagesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IChatsService _chatsService;
        private readonly UserManager<User> _userManager;

        public MessagesService(IUnitOfWork unitOfWork, UserManager<User> userManager, IHttpContextAccessor contextAccessor, IChatsService chatsService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _chatsService = chatsService;
        }

        public async Task<MessageDto> SendGroupMessageAsync(string chatId, MessageDataModel messageData)
        {
            var chat = await _unitOfWork.Chats.GetByIdAsync(chatId);

            if (chat == null)
            {
                throw new ItemNotFoundException("Specified chat was not found");
            }

            if (chat.Type != ChatType.Group)
            {
                throw new InvalidOperationException("Specified chat is not a group");
            }

            return await SendMessageInternalAsync(chatId, messageData);
        }

        private async Task<MessageDto> SendMessageInternalAsync(string chatId, MessageDataModel messageData)
        {
            var principal = _contextAccessor.HttpContext.User;
            var ownerUser = await _userManager.GetUserAsync(principal);
            var chat = await _unitOfWork.Chats.GetByIdAsync(chatId);

            if (messageData.ReplyToId != null)
            {
                var reply = await _unitOfWork.Messages.GetByIdAsync(messageData.ReplyToId);
                if (reply == null || reply.ChatId.ToString() != chatId)
                {
                    throw new ItemNotFoundException("Message reply target was not found in this chat");
                }
            }

            var message = new Message
            {
                AuthorId = ownerUser.Id,
                ChatId = chat.Id,
                TimePublished = DateTime.UtcNow,
                ReplyToId = messageData.ReplyToId == null ? null : new Guid?(new Guid(messageData.ReplyToId)),
                Text = messageData.Text
            };

            await _unitOfWork.Messages.CreateAsync(message);

            return message.ToDto();
        }

        public async Task<MessageDto> SendPersonalMessageAsync(string userId, MessageDataModel messageData)
        {
            var otherUser = await _unitOfWork.Users.GetByIdAsync(userId);
            if (otherUser == null)
            {
                throw new ItemNotFoundException("User not found");
            }

            // Find chat with user, or create new one
            var chatWithUser = await GetChatWithUser(userId)
                               ?? await _chatsService.CreatePersonalAsync(userId);

            return await SendMessageInternalAsync(chatWithUser.Id, messageData);
        }

        private async Task<ChatDto> GetChatWithUser(string userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                throw new ItemNotFoundException("User not found");
            }
            return (await _unitOfWork.Chats.QueryAsync(c =>
                    c.Participants
                        .Any(p =>
                            p.UserId.ToString() == userId)
                            && c.Type == ChatType.Personal))
                .FirstOrDefault()?.ToDto();
        }

        public async Task<bool> DeleteMessageAsync(string messageId)
        {
            var message = await _unitOfWork.Messages.GetByIdAsync(messageId);

            if (message == null)
            {
                return false;
            }

            await _unitOfWork.Messages.DeleteByIdAsync(messageId);
            return true;
        }

        public async Task<MessageDto> EditMessageAsync(string messageId, MessageDataModel messageData)
        {
            var message = await _unitOfWork.Messages.GetByIdAsync(messageId);

            if (message == null)
            {
                throw new ItemNotFoundException("Message not found");
            }

            if (messageData.ReplyToId != null)
            {
                var reply = await _unitOfWork.Messages.GetByIdAsync(messageData.ReplyToId);
                if (reply == null || reply.ChatId != message.ChatId)
                {
                    throw new ItemNotFoundException("Message reply target was not found in this chat");
                }

                if (reply.TimePublished > message.TimePublished)
                {
                    throw new InvalidOperationException("Message reply target must be published before this message");
                }
            }

            message.Update(messageData);

            await _unitOfWork.Messages.UpdateAsync(message);
            return message.ToDto();
        }

        public async Task<PagedResponse<MessageDto>> GetGroupMessagesAsync(string chatId, PaginationFilter filter)
        {
            var chat = await _unitOfWork.Chats.GetByIdAsync(chatId);
            if (chat == null)
            {
                throw new ItemNotFoundException("Chat not found");
            }

            var allMessages = (await _unitOfWork.Messages
                    .QueryAsync(m =>
                        m.ChatId.ToString() == chatId))
                .Select(m => m.ToDto())
                .OrderByDescending(m => m.TimePublished)
                .ToArray();
            return PagedResponse<MessageDto>.CreatePagedResponse(allMessages, filter);
        }

        public async Task<PagedResponse<MessageDto>> GetPersonalMessagesAsync(string userId, PaginationFilter filter)
        {
            var chatWithUser = await GetChatWithUser(userId);
            return await GetGroupMessagesAsync(chatWithUser.Id, filter);
        }

        public async Task<MessageDto> GetMessageByIdAsync(string messageId)
        {
            return (await _unitOfWork.Messages.GetByIdAsync(messageId))?.ToDto();
        }
    }
}
