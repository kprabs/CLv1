using AuthService;
using CoreLib.Application.Common.Enums;
using CoreLib.Application.Common.Interfaces;
using CoreLib.Application.Common.Utility;
using System.Collections.Concurrent;
using static AuthService.AuthorizationServiceNewClient;

namespace CoreLib.Application.Common.AuthServices
{
    public class AuthorizationManager : IAuthorizationManager
    {
        private readonly string TenantCode = ClassifiedSegmentEnum.Tenant.GetTypeTableCode() ?? String.Empty;
        private readonly string ClientCode = ClassifiedSegmentEnum.Client.GetTypeTableCode() ?? String.Empty;
        private readonly string AccountCode = ClassifiedSegmentEnum.Account.GetTypeTableCode() ?? String.Empty;
        private readonly string SubAccountCode = ClassifiedSegmentEnum.SubAccount.GetTypeTableCode() ?? String.Empty;
        private readonly ConcurrentDictionary<string, string[]> _accessTypes = new ConcurrentDictionary<string, string[]>();

        public async Task<IList<string>> GetAllTradingPartnersForUser(string accessType, int userId)
        {
            using (AuthorizationServiceNewClient client = new(EndpointConfiguration.BasicHttpsBinding_IAuthorizationServiceNew))
            {
                GetAllTradingPartnersForUserRequest request = new()
                {
                    accessType = accessType,
                    systemCode = TypeCodeValue.ApplicationCode.Value,
                    userId = userId
                };
                var result = await client.GetAllTradingPartnersForUserAsync(request);
                return result.GetAllTradingPartnersForUserResult;
            }
        }

        public async Task<InstanceHierarchyDTO> GetInstanceHierarchy(InstanceHeaderDTO item)
        {
            using (AuthorizationServiceNewClient client = new(EndpointConfiguration.BasicHttpsBinding_IAuthorizationServiceNew))
            {
                GetInstanceHierarchyRequest request = new() { item = item };
                var result = await client.GetInstanceHierarchyAsync(request);
                return result.GetInstanceHierarchyResult;
            }
        }

        public async Task<IList<InstanceHierarchyDTO>> GetInstanceHierarchys(IList<InstanceHeaderDTO> items)
        {
            using (AuthorizationServiceNewClient client = new(EndpointConfiguration.BasicHttpsBinding_IAuthorizationServiceNew))
            {
                GetInstanceHierarchysRequest request = new() { items = items.ToList() };
                var result = await client.GetInstanceHierarchysAsync(request);
                return result.GetInstanceHierarchysResult;
            }
        }

        public async Task<IList<UserAuthorizationAccessTermDto>> GetUserTermsAndCondition(int userId)
        {
            var result = await GetUserAuthorizationInfo(userId);
            return result.AccessTerms.ToList();
        }

        public async Task<bool> IsUserTenantAdmin(int userId)
        {
            using (AuthorizationServiceNewClient client = new(EndpointConfiguration.BasicHttpsBinding_IAuthorizationServiceNew))
            {
                IsUserTenantAdminRequest request = new() { loginSystemUserId = userId };

                var result = await client.IsUserTenantAdminAsync(request);
                return result.IsUserTenantAdminResult;
            }
        }

        public async Task<IList<string>> GetAllTradingPartnersForUser(string systemCode, IList<string> accessTypes, int userId)
        {
            using (AuthorizationServiceNewClient client = new(EndpointConfiguration.BasicHttpsBinding_IAuthorizationServiceNew))
            {
                GetAllTradingPartnersForUserMultipleAccessRequest request = new()
                {
                    systemCode = systemCode,
                    accessTypes = accessTypes.ToList(),
                    userId = userId
                };
                var result = await client.GetAllTradingPartnersForUserMultipleAccessAsync(request);
                return result.GetAllTradingPartnersForUserMultipleAccessResult;
            }
        }

        public async Task<IList<string>> GetAllAccessibleInstancesForUser(int userId, ClassifiedSegmentEnum level, string accessType)
        {
            var result = await GetUserAuthorizationInfo(userId, TypeCodeValue.ApplicationCode.Value);
            return result.AccessDetails.Where(x => accessType == x.AccessTypeCode
                    && x.ClassifiedSegmentCode == level.GetTypeTableCode()).Select(x => x.ClassifiedAreaSegmentNKey).Distinct().ToList();
        }

        public async Task<IList<string>> GetAllAccountsForUserMultipleAccess(int userId, IList<string> accessTypes, string systemCode)
        {
            using (AuthorizationServiceNewClient client = new(EndpointConfiguration.BasicHttpsBinding_IAuthorizationServiceNew))
            {
                GetAllAccountsForUserMultipleAccessRequest request = new()
                {
                    accessTypes = accessTypes.ToList(),
                    userId = userId,
                    systemCode = systemCode
                };
                var result = await client.GetAllAccountsForUserMultipleAccessAsync(request);
                return result.GetAllAccountsForUserMultipleAccessResult;
            }
        }

        public async Task<IList<BillingGroupDTO>> GetAllBillingGroupsByUserANDAcessType(string systemCode, string accessType, int userId)
        {
            using (AuthorizationServiceNewClient client = new(EndpointConfiguration.BasicHttpsBinding_IAuthorizationServiceNew))
            {
                GetAllBillingGroupsByUserANDAcessTypeRequest request = new()
                {
                    accesType = accessType,
                    loginsystemUserId = userId,
                    systemCode = systemCode
                };
                var result = await client.GetAllBillingGroupsByUserANDAcessTypeAsync(request);
                return result.GetAllBillingGroupsByUserANDAcessTypeResult;
            }
        }

        public async Task<IList<InstanceHeaderDTO>> GetAllSegmentInstances(string ClassifiedAreaCode, string ClassifiedSegmentCode)
        {
            using (AuthorizationServiceNewClient client = new(EndpointConfiguration.BasicHttpsBinding_IAuthorizationServiceNew))
            {
                GetAllSegmentInstancesRequest request = new()
                {
                    classifiedAreaCode = ClassifiedAreaCode,
                    classifiedSegmentCode = ClassifiedSegmentCode
                };
                var result = await client.GetAllSegmentInstancesAsync(request);
                return result.GetAllSegmentInstancesResult;
            }
        }

        public async Task<bool> CanCurrentUserAccess(string accessTypes, int userId) { return await CheckAccess(userId, accessTypes); }
        public async Task<bool> CanCurrentUserAccessTenant(string accessTypes, string tenantNKey, int userId) { return await CheckAccess(userId, accessTypes, TenantCode, tenantNKey); }
        public async Task<bool> CanCurrentUserAccessClient(string accessTypes, String clientCID, int userId) { return await CheckAccess(userId, accessTypes, ClientCode, clientCID); }
        public async Task<bool> CanCurrentUserAccessAccount(string accessTypes, string accountNKey, int userId) { return await CheckAccess(userId, accessTypes, AccountCode, accountNKey); }
        public async Task<bool> CanCurrentUserAccessSubAccount(string accessTypes, string subAccountNKey, int userId) { return await CheckAccess(userId, accessTypes, SubAccountCode, subAccountNKey); }

        #region private methods
        private async Task<UserAuthorizationAccessDTO> GetUserAuthorizationInfo(int userId)
        {
            using (AuthorizationServiceNewClient client = new AuthorizationServiceNewClient(EndpointConfiguration.BasicHttpsBinding_IAuthorizationServiceNew))
            {
                GetUserAuthorizationDetailsRequest request = new GetUserAuthorizationDetailsRequest { loginSystemUserId = userId, systemCode = TypeCodeValue.ApplicationCode.Value };
                var result = await client.GetUserAuthorizationDetailsAsync(request);
                return result.GetUserAuthorizationDetailsResult;

            }
        }
        private async Task<UserAuthorizationAccessDTO> GetUserAuthorizationInfo(int userId, string systemCode)
        {
            using (AuthorizationServiceNewClient client = new AuthorizationServiceNewClient(EndpointConfiguration.BasicHttpsBinding_IAuthorizationServiceNew))
            {
                GetUserAuthorizationDetailsRequest request = new GetUserAuthorizationDetailsRequest { loginSystemUserId = userId, systemCode = systemCode };

                var result = await client.GetUserAuthorizationDetailsAsync(request);
                return result.GetUserAuthorizationDetailsResult;

            }

        }
        private async Task<bool> CheckAccess(int userId, string accessTypes, string segmentCode, string codeValue)
        {
            return await CheckAccess(userId, accessTypes, (accessDetail, accessType) => accessDetail.AccessTypeCode == accessType && accessDetail.ClassifiedSegmentCode == segmentCode && accessDetail.ClassifiedAreaSegmentNKey == codeValue);
        }
        private async Task<bool> CheckAccess(int userId, string accessTypes)
        {
            return await CheckAccess(userId, accessTypes, (accessDetail, accessType) => accessDetail.AccessTypeCode == accessType);
        }
        private async Task<bool> CheckAccess(int userId, string accessTypes, Func<UserAuthorizationAccessDetailDTO, string, bool> filter)
        {
            var userPermInfo = await GetUserAuthorizationInfo(userId);
            var accessTypeList = _accessTypes.GetOrAdd(accessTypes, (k) => k.Split(',').Select(x => x.Trim()).ToArray()).ToList();
            return accessTypeList.Exists(accessType => userPermInfo.AccessDetails.Exists(accessDetail => filter(accessDetail, accessType)));
        }
        #endregion
    }
}