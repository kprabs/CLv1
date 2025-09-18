using AHA.IS.Common.Authorization.Domain;
using AHA.IS.Common.Authorization.DTO.New;
using CoreLib.Application.Common.Enums;
using CoreLib.Application.Common.Models;
using CoreLib.Application.Common.SqlEntities;

namespace CoreLib.Application.Common.Interfaces
{
    public interface IProvisionHandlerService
    {
        TreeGridDTO<int> GetSelectableClients(ICollection<SecurityAssignableItemDTO> clients, string brandName);
        Task<UserFeatureAccessPermissionDTO> ReviseUserAccessDetails(UserFeatureAccessPermissionDTO dto, string brandName, int reviseClientId, bool? newUser);
        IList<SecurityAssignableItemDTO> GetAllItems(IList<SecurityAssignableItemDTO> items);
        UserApplicationDetailsDTO GetUserApplicationDetailsSQLWrapper(int userId, int systemId, string brandName, IList<int> clientIds = null, IList<string> clientsNKey = null);
        UserApplicationDetailsDTO GetUserApplicationDetails(int userId, int systemId, string brandName);
        UserApplicationDetailsDTO GetUserApplicationDetailsWithSelectedClients(int userId, int systemId, string brandName, IList<int> clientIds);
        UserApplicationDetailsDTO GetUserApplicationDetailsWithSelectedClientsNKey(int userId, int systemId, string brandName, IList<string> clientNKeys);
        TreeGridDTO<int> GetFeatureAccessModel(Models.FeatureDTO featureDTO, UserApplicationDetailsDTO authInfo, UserFeatureAccessPermissionDTO dto, int BrokerClientId, string brandName);
        Models.FeatureDTO GetFeature(int systemPermissionGroupSetId, List<FeatureEntity> AllFeaturesList);
        UserApplicationDetailsDTO GetNewUserApplicationDetails(int userId, int systemId, string brandName, IList<int> clientIds = null);
        UserApplicationDetailsDTO GetNewUserApplicationDetailsSQLWrapper(int userId, int systemId, string brandName, IList<int> clientIds = null);
        UserApplicationDetailsDTO ProcessFeaturesForUser(UserAccessInfoModel? userInfoDetails, int userId, int systemId, string brandName);
        void UpdateFeatureCustomRow(UserFeatureAccessPermissionDTO dto, UserManagementEditFeatureDTO feature, bool isRevise = false);
        List<SecurityAssignableItemDTO> GetAssignableItemsByType(List<SecurityAssignableItemDTO> items, string segmentCode);
        SecurityAssignableItemHeaderDTO GetAssignableItemHeaders(int userId, int systemId, List<Tenant> tenants, List<int> AssignedClassifiedSegmentInstanceId);
        List<SystemPermissionGroupSetGrouping> GetSystemPermissionGroupSetGrouping(FeatureAssingedInstanceEntity x, List<FeatureAssingedInstanceEntity> featuresAssigned);
        UserPermission GetUserPermissions(FeatureAssingedInstanceEntity x, List<FeatureAssingedInstanceEntity> featuresAssigned);
        AppVerions GetAppVersionForPermissionCode(string permissionCode);
        Task<bool> SaveUserAuthorization(UserFeatureAccessPermissionDTO authinfo, string role, string brandName, bool? newUser);
        List<SecurityAssignableItemDTO> RecursiveGetAssignableItems(string classifiedSegmentName, ICollection<SecurityAssignableItemDTO> items);
        Task UpdateUserApplicationDetails(UserApplicationDetailsDTO userDetails, string remoteAddress);
        IList<UserManagementEditFeatureDTO> SetDefaultFeatures(IList<UserManagementEditFeatureDTO> features, string role);
        UserFeatureAccessPermissionDTO GetPlatformIndicatorUpdated(UserFeatureAccessPermissionDTO dto, string? reviseClientId);
        UserFeatureAccessPermissionDTO GetPlatformIndicatorUpdated(UserFeatureAccessPermissionDTO dto);
        Task<UserFeatureAccessPermissionDTO> ReviseUserAccessDetails(UserFeatureAccessPermissionDTO dto, string brandName, IList<int> reviseClientIds, bool? newUser);
        string GetPlatformIndicatorForClient(string clientInstanceNkey);
    }
}
