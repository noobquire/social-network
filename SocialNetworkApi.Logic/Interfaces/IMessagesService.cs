using System.Collections.Generic;
using System.Threading.Tasks;
using SocialNetworkApi.Services.Models;
using SocialNetworkApi.Services.Models.Dtos;

namespace SocialNetworkApi.Services.Interfaces
{
    public interface IMessagesService
    {
        Task<MessageDto> SendMessageAsync(string chatId, MessageDataModel messageData);
        Task<bool> DeleteMessageAsync(string messageId);
        Task<MessageDto> EditMessageAsync(string messageId, MessageDataModel messageData);
        Task<IEnumerable<MessageDto>> GetChatMessagesAsync(string chatId);
        Task<MessageDto> GetMessageByIdAsync(string messageId);
    }
}
