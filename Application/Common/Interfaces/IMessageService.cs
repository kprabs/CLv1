using CoreLib.Application.Common.Models.MessageModel;
using CoreLib.Entities.Messages;
using System.Threading.Tasks;

namespace CoreLib.Application.Common.Interfaces;

public interface IMessageService
{
    Task<MessagesResult> GetAdminMessages(MessageSearchCriteria criteria);
    Task<MessagesResult> GetUserMessages(   MessageSearchCriteria criteria);

    Task<List<MessageAudienceType>> GetMessageAudienceTypes();
    Task<List<MessageStatus>> GetMessageStatuses();
    Task<List<MessageType>> GetMessageTypes();

    Task<MessageItem?> AddMessage(MessageItemRequest item);
    

    Task<MessageItem> UpdateMessage(MessageItemRequest item);





    Task<bool> DeleteMessage(MessageItemRequest item);


}
