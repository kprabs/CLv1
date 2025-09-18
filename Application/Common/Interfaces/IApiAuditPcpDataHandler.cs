using CoreLib.Application.Common.Models;

namespace CoreLib.Application.Common.Interfaces
{
    public interface IApiAuditPcpDataHandler
    {
        Task<bool> UpsertApiPcpAudit(APIAuditPCPModel model);
        Task<APIAuditPCPModel> GetApiAuditPcp(string MemberID);
    }
}
