using AutoMapper;
using CoreLib.Application.Common.Interfaces;
using CoreLib.Application.Common.Models;
using CoreLib.Entities;

namespace CoreLib.Application.Common.APIAuditHelper.PCPServices
{
    public class ApiAuditPcpDataHandler(IGroupPortalRepository groupPortalRepository, IMapper mapper) : IApiAuditPcpDataHandler
    {
        public async Task<APIAuditPCPModel?> GetApiAuditPcp(string MemberID)
        {
            var apiAuditPCP = await groupPortalRepository.GetAPIAuditPCP(MemberID);
            return apiAuditPCP == null ? null : mapper.Map<APIAuditPCPModel>(apiAuditPCP);
        }

        private async Task<bool> UpdateApiAuditPcpInfo(APIAuditPCPModel model)
        {
            // convert the domain model to DB model
            var objAPIAuditPCPapiAuditPCP = mapper.Map<APIAuditPCP>(model);
            _ = Task.Run(() => groupPortalRepository.UpdateAPIAuditPCP(objAPIAuditPCPapiAuditPCP)).ConfigureAwait(false);

            return true;
        }

        public Task<bool> UpsertApiPcpAudit(APIAuditPCPModel model)
        {
            ArgumentNullException.ThrowIfNull(model);
            return UpdateApiAuditPcpInfo(model);
        }
    }
}
