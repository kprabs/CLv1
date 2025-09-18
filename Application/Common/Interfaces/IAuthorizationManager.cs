using AuthService;
using CoreLib.Application.Common.Enums;

namespace CoreLib.Application.Common.Interfaces
{
    public interface IAuthorizationManager
    {
        Task<IList<string>> GetAllTradingPartnersForUser(string accessType, int userId);
        Task<InstanceHierarchyDTO> GetInstanceHierarchy(InstanceHeaderDTO item);
        Task<IList<InstanceHierarchyDTO>> GetInstanceHierarchys(IList<InstanceHeaderDTO> items);
        Task<IList<UserAuthorizationAccessTermDto>> GetUserTermsAndCondition(int userId);
        Task<bool> IsUserTenantAdmin(int userId);
        Task<IList<string>> GetAllTradingPartnersForUser(string systemCode, IList<string> accessTypes, int userId);
        Task<IList<string>> GetAllAccessibleInstancesForUser(int userId, ClassifiedSegmentEnum level, string accessType);
        Task<IList<string>> GetAllAccountsForUserMultipleAccess(int userId, IList<string> accessTypes, string systemCode);
        Task<IList<BillingGroupDTO>> GetAllBillingGroupsByUserANDAcessType(string systemCode, string accessType, int userId);
        Task<IList<InstanceHeaderDTO>> GetAllSegmentInstances(string ClassifiedAreaCode, string ClassifiedSegmentCode);
        Task<bool> CanCurrentUserAccess(string accessTypes, int userId);
        Task<bool> CanCurrentUserAccessTenant(string accessTypes, string tenantNKey, int userId);
        Task<bool> CanCurrentUserAccessClient(string accessTypes, string clientCID, int userId);
        Task<bool> CanCurrentUserAccessAccount(string accessTypes, string accountNKey, int userId);

    }
}
