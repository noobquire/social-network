using System.Collections.Generic;
using System.Threading.Tasks;
using SocialNetworkApi.Services.Models;
using SocialNetworkApi.Services.Models.Dtos;

namespace SocialNetworkApi.Services.Interfaces
{
    public interface IChatsService
    {
        Task<ChatDto> CreateGroupAsync(NewChatModel newChat);
        Task<ChatDto> CreatePersonalAsync(string userId);
        Task<ChatDto> GetByIdAsync(string chatId);
        Task<IEnumerable<ChatDto>> GetUserChats();
        Task<bool> LeaveChatAsync(string chatId);
        Task<bool> EditChatAsync(string chatId, ChatDto chatData);
    }
}