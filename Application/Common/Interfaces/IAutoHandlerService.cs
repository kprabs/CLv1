using CoreLib.Application.Common.Models;

namespace CoreLib.Application.Common.Interfaces
{
    public interface IAutoHandlerService
    {
        void AutoProvisioning(UserFeatureAccessPermissionDTO dto, string requestGUID);
        string? AutoProvisionStatus(string RID);
        bool IsAutoProvisionsConcent(int loginId, string ClassifiedSegmentInstanceId);
    }
}
