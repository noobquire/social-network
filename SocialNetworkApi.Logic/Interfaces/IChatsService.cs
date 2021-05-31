using System.Threading.Tasks;
using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Services.Models;
using SocialNetworkApi.Services.Models.Dtos;

namespace SocialNetworkApi.Services.Interfaces
{
    public interface IChatsService
    {
        Task<ChatDto> CreateGroupAsync(NewChatModel newChat);
        Task<ChatDto> CreatePersonalAsync(string userId);
        Task<ChatDto> GetByIdAsync(string chatId);
        Task<PagedResponse<ChatDto>> GetUserChats(PaginationFilter filter);
        Task<bool> LeaveChatAsync(string chatId);
        Task<bool> IsAdmin(string userId, string chatId);
    }
}