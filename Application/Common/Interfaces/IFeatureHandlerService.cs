using AHA.IS.Common.Authorization.DTO.New;
using CoreLib.Application.Common.Models;

namespace CoreLib.Application.Common.Interfaces
{
    public interface IFeatureHandlerService
    {
        List<TreeGridRowDTO<int>> GetTreeGridRows(List<SecurityAssignableItemDTO> items, AHA.IS.Common.Authorization.DTO.New.UserFeatureSelectionDTO selections,
            List<int> valueHeaderIds, IList<string> allowedSegmentCodes, string brandName, int level = 0, int parentId = 0);
        int GetAllSubAccountsCount(List<SecurityAssignableItemDTO> accounts);
        List<bool?> GetRowProperties(SecurityAssignableItemDTO item, AHA.IS.Common.Authorization.DTO.New.UserFeatureSelectionDTO selections, List<int> valueHeaderIds,
            int parentId = 0);
        ICollection<string> GetConfiguredWorkbaskets();
        int? GetSelectedOptionId(UserApplicationDetailsDTO userInfo, Models.FeatureDTO featureInfo,
            AHA.IS.Common.Authorization.DTO.New.UserFeatureSelectionDTO featureSelection, bool allClients);
        int? GetSelectedOptionId(AHA.IS.Common.Authorization.DTO.New.UserFeatureSelectionDTO featureSelection);
        int? GetSelectedOptionIdForName(Models.FeatureDTO featureInfo, UserApplicationDetailsDTO userInfo,
            AHA.IS.Common.Authorization.DTO.New.UserFeatureSelectionDTO featureSelection, UserFeatureAccessPermissionDTO dto,
            bool isStopLoss);
        int? GetSelectedOptionIdForName(GetSelectedOptionIdForNameModel selectedOptionIdForName);
        int?[] GetSelectedOptionIds(AHA.IS.Common.Authorization.DTO.New.UserFeatureSelectionDTO featureSelection);
        UserManagementEditFeatureDTO UpdateHardcodeFeature(Models.FeatureDTO featureDto, UserManagementEditFeatureDTO userFeatureDto);
        List<SecurityAssignableItemDTO> RecursiveGetAssignableItems(string classifiedSegmentName, ICollection<SecurityAssignableItemDTO> items);
        IList<SelectedClientIdsDTO> GetSelectedClient(UserAccessInfoDTO userAccessInfo, UserFeatureAccessPermissionDTO userInfo, int BrokeyClientId);
        void AddCustomFeatures(IList<FeatureAccessInfoDTO> features, string userRole);
        bool HasAccessByUserRole(string permissionCode, string userRole);
    }
}
