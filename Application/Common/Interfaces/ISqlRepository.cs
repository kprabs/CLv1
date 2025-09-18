using CoreLib.Application.Common.Models;
using CoreLib.Application.Common.SqlEntities;
using CoreLib.Entities;
using CoreLib.Entities.Messages;
using System.Data;

namespace CoreLib.Application.Common.Interfaces
{
    public interface ISqlRepository
    {
        DataTable GetAllFeatures(string systemId);

        List<FeatureEntity> GetAllFeaturesToList(string systemId);

        DataTable GetAssignableClientsHeirarchyForUser(string systemId, string userId);

        List<Tenant> GetAssignableClientsHeirarchyForUserToList(string systemId, string userId);
        DataTable GetUserInformationOfUser(string systemId, string userName);

        List<UserInfoEntity> GetUserInformationOfUserToList(string systemId, string userId);
        DataTable GetFeatureAssignedInstancesForUser(string systemId, string userId);

        List<FeatureAssingedInstanceEntity> GetFeatureAssignedInstancesForUserToList(string systemId, string userId);
        DataTable GetUserInformationOfUser(string userName);
        List<UserInfoEntity> GetUserInformationOfUserToList(string userId);
        DataTable GetUserInfoOnPermissionCode(string systemId, string userId, string permissionCode);
        List<UserInfoOnPermissionCode> GetUserInfoOnPermissionCodeToList(string systemId, string userName, string permissionCode);
        List<UserInfoOnPermissionCode> GetUserInfoForClientToList(string systemId, string userName, string clientId);
        DataTable GetUserListBySearchCriteria(string systemId, string userName, string lastName, string firstName, string email, string status);
        List<UserListBySearchCriteriaEntity> GetUserListBySearchCriteriaToList(string systemId, string? userName, string? lastName, string? firstName, string? email, string? status);
        UserAccessInfoModel GetNewUserInfoDetails(string systemId, string userId, string brandName, IList<int>? clientIds);
        UserAccessInfoModel GetUserInfoDetails(string systemId, string userId, string brandName, IList<int>? clientId, IList<string>? clientNKeys);
        List<UserAccessVerficationEntity> GetUserVerificationToList(string systemCode, string userName, string userRole);
        List<ClassifiedInstancesEntity> GetClassifiedInstancesByClientId(string clientId);
        UserAccessInfoModel GetClientsForBrand(string systemId, string userId, string BrandName, string cid, string clientName);
        UserConsentType? GetUserConsentType(int SystemLoginUserId, string ClassifiedSegmentInstanceId);
        List<UsersByClientEntity> GetUsersByClientData(string CID, string brandName, string userType);
        List<T> GetQueryResult<T>(string query);
        List<CoreLib.Entities.Feature> GetFeature(string featureName);
        List<GroupPortalCrossWalkPartialDTO> GetMasterLoginIdentifier(string username);
        List<UserAssociateClientInforamtion> GetAllClientNKeyAssociatedToUser(string loginUserName);
        List<GroupPortalCrossWalkDTO> GetUserCrossWalkInfo(int userId);

        string GetAdminMessageQuery(CoreLib.Entities.Messages.MessageSearchCriteria criteria);
        string GetUserMessageQuery(long? id);

        string GetMessageGroupMembersQuery(long id);
        string GetMessageGroupMembersQuery(List<long> ids);

        List<CoreLib.Entities.Messages.MessageView> GetMessages(string query);
        List<CoreLib.Entities.Messages.MessageGroupMembersView> GetMessageMembers(string query);

        List<CoreLib.Entities.Messages.MessageAudienceType> GetMessageAudienceTypes();
        List<CoreLib.Entities.Messages.MessageStatus> GetMessageStatuses();
        List<CoreLib.Entities.Messages.MessageType> GetMessageTypes();
        List<MessageViewModel> GetAllMessages();
    }
}
