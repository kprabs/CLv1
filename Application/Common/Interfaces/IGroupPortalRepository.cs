using CoreLib.Application.Common.Models.MessageModel;
using CoreLib.Entities;
using CoreLib.Entities.Messages;
using CoreLib.Infrastructure.Persistence;

namespace CoreLib.Application.Common.Interfaces
{
    public interface IGroupPortalRepository
    {
        public Task SetAutoProvisionLog(AutoProvisionLog autoProvisionLog);
        Task<IEnumerable<SystemUserLoginLog>> GetUserLoginLogAsync(string username);
        Task<bool> SetUserLoginLogAsync(SystemUserLoginLog userNotes);
        public Task<DocumentInstance> GetTermsAndConditionsDetail();
        public Task<bool> DeactiveUserTermsAndContions(string userName);
        public Task<string> GetUserPermissionClientNKeyName(string nKey);
        void UpdateAutoProvisionLog(string RequestID, string statusCode, string remarks);
        AutoProvisionLog? GetAutoProvisionLog(string RequestID);
        public Task SetAPIAudit(APIAudit setAPIAudit);
        public Task<APIAuditPCP?> GetAPIAuditPCP(string memberIdNumber);
        public Task AddAPIAuditPCP(APIAuditPCP setAPIAuditPCP);
        int UpdateAPIAuditPCP(APIAuditPCP setAPIAuditPCP);
        public Task<int> CreateMessage(Message message);
        public Task<int> CreateMessageGroup(MessageGroup group);
        public Task<int> CreateMessageDelivery(MessageDelivery delivery);
        public Task<int> CreateMessageGroupMembers(List<MessageGroupMember> members);
        public Task<Message> UpdateMessage(Message message);

        public Task<bool> DeleteMessage(Message message);

    }
}
