using System.Collections.Generic;
using System.Threading.Tasks;
using SocialNetworkApi.Services.Models;
using SocialNetworkApi.Services.Models.Dtos;

namespace SocialNetworkApi.Services.Interfaces
{
    public interface IMessagesService
    {
        Task<MessageDto> SendGroupMessageAsync(string chatId, MessageDataModel messageData);
        Task<MessageDto> SendPersonalMessageAsync(string userId, MessageDataModel messageData);
        Task<bool> DeleteMessageAsync(string messageId);
        Task<MessageDto> EditMessageAsync(string messageId, MessageDataModel messageData);
        Task<IEnumerable<MessageDto>> GetGroupMessagesAsync(string chatId);
        Task<IEnumerable<MessageDto>> GetPersonalMessagesAsync(string userId);
        Task<MessageDto> GetMessageByIdAsync(string messageId);
    }
}
