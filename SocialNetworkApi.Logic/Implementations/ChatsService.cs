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
using SocialNetworkApi.Services.Validation;

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
                var validator = new ValidateGuidAttribute();
                if(!validator.IsValid(participantId))
                {
                    throw new ItemNotFoundException("Specified Id is not a valid GUID");
                }

                var user = await _userManager.FindByIdAsync(participantId);

                if (user == null)
                {
                    throw new ItemNotFoundException("User with specified Id was not found");
                }

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
            catch (InvalidOperationException ex) when (ex.Message.Contains("already being tracked"))
            {
                throw new DuplicateChatParticipantException("Participant is already in this chat");
            }

            return chat.ToDto();
        }

        public async Task<ChatDto> CreatePersonalAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var principal = _contextAccessor.HttpContext.User;
            var ownerUser = await _userManager.GetUserAsync(principal);
            var existingChat = (await _unitOfWork.Chats
                .QueryAsync(c => c.Type == ChatType.Personal && 
                                 c.Participants.Any(uc => uc.UserId.ToString() == userId) 
                                 && c.Participants.Any(uc => uc.UserId == ownerUser.Id)))
                .FirstOrDefault();
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
            chat.Participants.Add(secondParticipant);

            await _unitOfWork.SaveChangesAsync();

            return chat.ToDto();
        }

        public async Task<ChatDto> GetByIdAsync(string chatId)
        {
            return (await _unitOfWork.Chats.GetByIdAsync(chatId))?.ToDto();
        }

        public async Task<PagedResponse<ChatDto>> GetUserChats(PaginationFilter filter)
        {
            var principal = _contextAccessor.HttpContext.User;
            var currentUser = await _userManager.GetUserAsync(principal);
            var allChats = (await _unitOfWork.Chats
                    .QueryAsync(c => c.Participants
                        .Any(p => p.UserId == currentUser.Id)))
                .Select(c => c.ToDto()).ToArray();
            return PagedResponse<ChatDto>.CreatePagedResponse(allChats, filter);
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

        public async Task<bool> IsAdmin(string userId, string chatId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var chat = await _unitOfWork.Chats.GetByIdAsync(chatId);

            return chat.Participants.Any(uc => uc.UserId == user.Id && uc.IsAdmin);
        }
    }
}
