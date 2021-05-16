using System.Collections.Generic;
using System.Threading.Tasks;
using SocialNetworkApi.Services.Models;
using SocialNetworkApi.Services.Models.Dtos;

namespace SocialNetworkApi.Services.Interfaces
{
    public interface IMessagesService
    {
        Task<MessageDto> SendMessage(string chatId, MessageDataModel messageData);
        Task<bool> DeleteMessage(string messageId);
        Task<bool> EditMessage(string messageId, MessageDataModel messageData);
        Task<IEnumerable<MessageDto>> GetChatMessages(string chatId);
    }
}
