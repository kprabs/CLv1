using AHA.IS.Common.Authorization.DTO.New;
using CoreLib.Application.Common.Models;
using CoreLib.Application.Common.SqlEntities;
using Forgerock.SuperAdmin.Models;
using Microsoft.Extensions.Configuration;
using UserAuthServiceNew1;

namespace CoreLib.Application.Common.Interfaces
{
    public interface IUserAuthorizationService
    {
        List<UserListBySearchCriteriaEntity> GetUserListBySearchCriteria(int systemId, SearchCriteriaDTO searchCriteria);
        AHA.IS.Common.Authorization.DTO.New.UserDetailsDTO GetUserDetails(int userId, string remoteAddress);
        Task DeactivateUserSystemAccess(int userId, string systemCode, DateTime terminationDate, string remoteAddress);
        Task ReactivateUserSystemAccess(int userId, string systemCode, DateTime effectiveDate, string BrandName, string remoteAddress);
        Task UpdateUserInformation(UserDTO userDetails, string remoteAddress);
        Task UpdateUserRole(int userId, string oldRole, string newRole, string remoteAddress);
        Task<UserAccessInfoDTO> GetSelectedUserAccessInfo(SearchCriteriaDTO searchCriteria, string orgId,
            string? userRole, int BrokerClientId, bool? IsMbrAvailable,
            bool? IsIndRptAvailable, bool? IsStlsRptAvailable, string actualUserName);
        Task<UserFeatureAccessPermissionDTO> GetSelectedUserDTO(UserListBySearchCriteriaEntity userDetails,
            string orgId, int BrokerClientId, bool? IsMbrAvailable, bool? IsIndRptAvailable,
            bool? IsStlsRptAvailable, bool? newUser, string actualUserName, GetUUIDResponse FRUsersList = null);
        Task<Models.UserDetailsDTO> GetUsersOfFR(IConfiguration configuration, string OrgId, string userName, GetUUIDResponse FRUsersList = null);
        List<UserAccessVerficationEntity> GetUserVerificationToList(string systemCode, string userName, string userRole);
        string SaveAuditInfoForGPAndArchive(string _userName, string _clientNKey, string _SessionNKey, string _effectiveDateISO, string _TermDateISO);
        GroupUserCrossWalkResponse SaveGroupUserCrossWalk(GroupUserCrossWalkDTO groupUserCrossWalkDTO);
        GroupUserCrossWalkResponse UpdateGroupUserCrossWalkActiveFlag(string logInSystemUserName, ClassifiedSegmentInstanceNKeyWIthStatus[] classifiedSegmentInstanceNKeyWithStatus, string lastUpdateUserNKey);
        GroupUserCrossWalkResponse UpdateGroupUserCrossWalkActiveFlagWithAllowAllBillsToAccount(string logInSystemUserName, ClassifiedSegmentInstanceNKeyWIthStatus[] classifiedSegmentInstanceNKeyWithStatus, string lastUpdateUserNKey, bool allAllBillsToAccountFlag);
        GroupUserCrossWalkResponse UpdateGroupUserCrossWalkMasterLoginSystemUser(string logInSystemUserName, string[] classifiedSegmentInstanceNKey, string masterLoginSystemUserNKey, string lastUpdateUserNKey);

        SecurityAssignableItemHeaderDTO GetAssignableClientSelected(int userId, int systemId, string brandName, string cid, string clientName);
        void AddOrUpdateUserConsent(int LoginSystemUserId, int ClassifiedSegmentInstanceId, int ConsentTypeId, bool ConsentFlag, string CreatedLoginUserId);
        string? GetAutoProvisionStatus(string RID);
        ClientAccessInfoDTO GetAuthorizedClientAccessInfo(string userName, string clientId, string accessTypes);
        IList<AccountAccessInfoDTO> GetAuthorizedAccountsAndSubaccounts(string userName, string clientId);
        string DisableCrossWalkUser(string _userName, string loginUser);
        List<GroupPortalCrossWalkDTO> GetUserCrossWalkInformation(int userId);
    }
}
