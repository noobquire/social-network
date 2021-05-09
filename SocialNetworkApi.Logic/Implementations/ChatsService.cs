using System;
using System.Collections.Generic;
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
    public class ChatsService : IChatsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;

        public ChatsService(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
            _userManager = userManager;
        }

        public async Task<ChatDto> CreateGroupAsync(NewChatModel newChat)
        {
            var chat = new Chat
            {
                Name = newChat.Name,
                Type = ChatType.Group
            };

            await _unitOfWork.Chats.CreateAsync(chat);
            var principal = _contextAccessor.HttpContext.User;
            var ownerUser = await _userManager.GetUserAsync(principal);

            var ownerParticipant = new UserChat()
            {
                ChatId = chat.Id,
                UserId = ownerUser.Id,
                IsAdmin = true
            };

            chat.Participants.Add(ownerParticipant);
            ownerUser.Chats.Add(ownerParticipant);

            foreach (var participantId in newChat.ParticipantIds)
            {
                var user = await _userManager.FindByIdAsync(participantId);
                var chatParticipant = new UserChat()
                {
                    ChatId = chat.Id,
                    UserId = new Guid(participantId),
                };

                chat.Participants.Add(chatParticipant);
                user.Chats.Add(chatParticipant);
            }

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (InvalidOperationException ex)
            {
                if (!ex.Message.Contains("already being tracked"))
                {
                    throw;
                }

                throw new DuplicateChatParticipantException("Participant is already in this chat");
            }

            return chat.ToDto();
        }

        public Task<ChatDto> CreatePersonalAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<ChatDto> GetByIdAsync(string chatId)
        {
            return (await _unitOfWork.Chats.GetByIdAsync(chatId)).ToDto();
        }

        public Task<IEnumerable<ChatDto>> GetUserChats()
        {
            throw new NotImplementedException();
        }

        public Task<bool> LeaveChatAsync(string chatId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EditChatAsync(string chatId, ChatDto chatData)
        {
            throw new NotImplementedException();
        }
    }
}
