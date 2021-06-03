using SocialNetworkApi.Data.Models;
using SocialNetworkApi.Services.Models;
using SocialNetworkApi.Services.Models.Dtos;
using System.Threading.Tasks;

namespace SocialNetworkApi.Services.Interfaces
{
    public interface IMessagesService
    {
        Task<MessageDto> SendGroupMessageAsync(string chatId, MessageDataModel messageData);
        Task<MessageDto> SendPersonalMessageAsync(string userId, MessageDataModel messageData);
        Task<bool> DeleteMessageAsync(string messageId);
        Task<MessageDto> EditMessageAsync(string messageId, MessageDataModel messageData);
        Task<PagedResponse<MessageDto>> GetGroupMessagesAsync(string chatId, PaginationFilter filter);
        Task<PagedResponse<MessageDto>> GetPersonalMessagesAsync(string userId, PaginationFilter filter);
        Task<MessageDto> GetMessageByIdAsync(string messageId);
    }
}
