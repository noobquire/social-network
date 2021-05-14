using System;
using System.Collections.Generic;
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

        public async Task<ChatDto> CreatePersonalAsync(string userId)
        {
            var user = await _userManager.FindByEmailAsync(userId);
            var principal = _contextAccessor.HttpContext.User;
            var ownerUser = await _userManager.GetUserAsync(principal);
            var existingChat = await _unitOfWork.Chats
                .QueryAsync(c => c.Type == ChatType.Personal && 
                                 c.Participants.Any(uc => uc.UserId.ToString() == userId) 
                                 && c.Participants.Any(uc => uc.UserId == ownerUser.Id));
            if(existingChat != null)
            {
                throw new ItemAlreadyExistsException("Personal chat with this user already exists");
            }

            var chat = new Chat
            {
                Name = $"{user.FirstName} {user.LastName}",
                Type = ChatType.Personal
            };

            await _unitOfWork.Chats.CreateAsync(chat);
            

            var firstParticipant = new UserChat
            {
                ChatId = chat.Id,
                UserId = ownerUser.Id,
                IsAdmin = false
            };

            var secondParticipant = new UserChat
            {
                ChatId = chat.Id,
                UserId = user.Id,
                IsAdmin = false
            };

            chat.Participants.Add(firstParticipant);
            ownerUser.Chats.Add(firstParticipant);
            chat.Participants.Add(secondParticipant);
            ownerUser.Chats.Add(secondParticipant);

            await _unitOfWork.SaveChangesAsync();

            return chat.ToDto();
        }

        public async Task<ChatDto> GetByIdAsync(string chatId)
        {
            return (await _unitOfWork.Chats.GetByIdAsync(chatId))?.ToDto();
        }

        public async Task<IEnumerable<ChatDto>> GetUserChats()
        {
            var principal = _contextAccessor.HttpContext.User;
            var currentUser = await _userManager.GetUserAsync(principal);
            return (await _unitOfWork.Chats
                    .QueryAsync(c => c.Participants
                        .Any(p => p.UserId == currentUser.Id)))
                .Select(c => c.ToDto());
        }

        public async Task<bool> LeaveChatAsync(string chatId)
        {
            var principal = _contextAccessor.HttpContext.User;
            var currentUser = await _userManager.GetUserAsync(principal);
            var chat = await _unitOfWork.Chats.GetByIdAsync(chatId);
            if (chat == null)
            {
                return false;
            }

            if (chat.Participants.All(p => p.UserId != currentUser.Id))
            {
                return false;
            }

            var userChat = chat.Participants.First(c => c.UserId == currentUser.Id);
            chat.Participants.Remove(userChat);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public Task<bool> EditChatAsync(string chatId, ChatDto chatData)
        {
            throw new NotImplementedException();
        }
    }
}
