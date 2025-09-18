using AHA.IS.Common.Authorization.DTO.New.Enums;
using AuthServiceNewRef;
using AutoMapper;
using CoreLib.Application.Common.Models;
using Microsoft.Extensions.Configuration;

#pragma warning disable S3900
namespace CoreLib.Application.Common
{
    public interface IAuthorizationNugetServices
    {
        public IList<AuthServiceNewRef.UserAuthorizationAccessDetailDTO> GetAllAccessForUser(string systemCode, IList<string> accessTypeCodes, IList<ClassifiedSegmentEnum> level, int userId);
        public UserAuthorizationAccessDTO ValidateAuthorization(UserDefaultDTO user, bool isExistingUser);
        public UserAuthorizationAccessDTO ValidateAuthorization(UserDefaultDTO user);
        public bool CanCurrentUserAccess(string accessType);
        public bool CanCurrentUserAccess(string accessType, int userId);
        public bool CanCurrentUserAccessFromSetting(string settingName);
        public bool CanCurrentUserAccessSubAccountFromSetting(string settingName, string subAccountNKey);
        public bool CanCurrentUserAccessSubAccount(string settingName, string subAccountNKey);
        public bool HasAnyGroupFromSetting(string settingName);
        public IList<string> GetAllAccessibleInstancesForUser(int userId, Enums.ClassifiedSegmentEnum level, string accessType);
        public IList<InstanceHierarchyDTO> GetInstanceHierarchys(IList<AuthService.InstanceHeaderDTO> items);
        public bool IsUserTenantAdmin();

    }
    public class AuthorizationNugetServices : IAuthorizationNugetServices
    {
        private readonly AuthorizationNewCore.Nuget.IAuthorizationManager _authorizationManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthorizationNugetServices(AuthorizationNewCore.Nuget.IAuthorizationManager authorizationManager, IMapper mapper, IConfiguration configuration)
        {
            _authorizationManager = authorizationManager;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public UserAuthorizationAccessDTO ValidateAuthorization(UserDefaultDTO user, bool isExistingUser)
        {
            _authorizationManager.SetApplicationHeaders(userNameHeader: user.GivenName ?? String.Empty, groupsHeader: user.GroupsHeader ?? String.Empty, 
                    groupSetHeader: user.GroupSetHeader ?? String.Empty,
                    firstNameHeader: user.FirstNameHeader ?? String.Empty, 
                    lastNameHeader: user.LastNameHeader ?? String.Empty, 
                    emailHeader: user.EmailHeader ?? String.Empty,
                    logInSystemCode: user.LoginSystemCode ?? String.Empty, 
                    refreshMillis: user.RefreshMillis ?? String.Empty, 
                    authorizationMode: user.AuthorizationMode ?? String.Empty,
                    applicationCode: user.ApplicationCode ?? String.Empty, sessionCreateToken: user.SessionCreateToken ?? String.Empty, 
                    sessionProcessingUserSystem: user.SessionProcessingUserSystem ?? String.Empty, 
                    sessionProcessingUserName: user.SessionProcessingUserName ?? String.Empty, 
                    _configuration.GetValue<string>("AUTHORIZATION_ENDPOINT")??String.Empty);
            return _authorizationManager.ValidateAuthorization(String.Empty, isExistingUser);

        }
        public UserAuthorizationAccessDTO ValidateAuthorization(UserDefaultDTO user)
        {
            if (string.IsNullOrEmpty(user.GivenName))
            {
                throw new InvalidOperationException("unble to get the username from headers");
            }
            try
            {
                _authorizationManager.SetApplicationHeaders(userNameHeader: user.GivenName ?? String.Empty, groupsHeader: user.GroupsHeader ?? String.Empty,
                        groupSetHeader: user.GroupSetHeader ?? String.Empty,
                        firstNameHeader: user.FirstNameHeader ?? String.Empty,
                        lastNameHeader: user.LastNameHeader ?? String.Empty,
                        emailHeader: user.EmailHeader ?? String.Empty,
                        logInSystemCode: user.LoginSystemCode ?? String.Empty,
                        refreshMillis: user.RefreshMillis ?? String.Empty,
                        authorizationMode: user.AuthorizationMode ?? String.Empty,
                        applicationCode: user.ApplicationCode ?? String.Empty, sessionCreateToken: user.SessionCreateToken ?? String.Empty,
                        sessionProcessingUserSystem: user.SessionProcessingUserSystem ?? String.Empty,
                        sessionProcessingUserName: user.SessionProcessingUserName ?? String.Empty,
                        _configuration.GetValue<string>("AUTHORIZATION_ENDPOINT") ?? String.Empty);
                var response = _authorizationManager.ValidateAuthorization(String.Empty, true);
                return response;
            }catch(Exception ex)
            {
                throw new InvalidOperationException(ex.StackTrace+$"::Invalid with userNameHeader: {user.GivenName}, groupsHeader: {user.GroupsHeader}, groupSetHeader: {user.GroupSetHeader}, " +
                    $"firstNameHeader: {user.FirstNameHeader}, lastNameHeader: {user.LastNameHeader}, emailHeader: {user.EmailHeader}, " +
                    $"logInSystemCode: {user.LoginSystemCode}, refreshMillis: {user.RefreshMillis}, authorizationMode: {user.AuthorizationMode}, " +
                    $"applicationCode: {user.ApplicationCode}, sessionCreateToken: {user.SessionCreateToken}, " +
                    $"sessionProcessingUserSystem: {user.SessionProcessingUserSystem}, sessionProcessingUserName: {user.SessionProcessingUserName}");
            }

        }

        public IList<AuthServiceNewRef.UserAuthorizationAccessDetailDTO> GetAllAccessForUser(string systemCode, IList<string> accessTypeCodes, IList<ClassifiedSegmentEnum> level, int userId)
        {
            return _authorizationManager.GetAllAccessForUser(systemCode, accessTypeCodes.ToList(), level.ToList(), userId).ToList();
        }

        public bool CanCurrentUserAccess(string accessType)
        {
            return _authorizationManager.CanCurrentUserAccess(accessType);
        }

        public bool CanCurrentUserAccess(string accessType, int userId)
        {
            return _authorizationManager.CanCurrentUserAccess(userId, accessType);
        }

        public bool CanCurrentUserAccessFromSetting(string settingName)
        {
            return _authorizationManager.CanCurrentUserAccessFromSetting(settingName);
        }

        public bool CanCurrentUserAccessSubAccountFromSetting(string settingName, string subAccountNKey)
        {
            return _authorizationManager.CanCurrentUserAccessSubAccountFromSetting(settingName, subAccountNKey);
        }

        public bool CanCurrentUserAccessSubAccount(string settingName, string subAccountNKey)
        {
            return _authorizationManager.CanCurrentUserAccessSubAccount(settingName, subAccountNKey);
        }

        public bool HasAnyGroupFromSetting(string settingName)
        {
            return _authorizationManager.HasAnyGroupFromSetting(settingName);
        }
        public IList<string> GetAllAccessibleInstancesForUser(int userId, Enums.ClassifiedSegmentEnum level, string accessType)
        {

            return _authorizationManager.GetAllAccessibleInstancesForUser(userId, (ClassifiedSegmentEnum)level, accessType);
        }
        public IList<InstanceHierarchyDTO> GetInstanceHierarchys(IList<AuthService.InstanceHeaderDTO> items)
        {
            var entities = _mapper.Map<List<AuthService.InstanceHeaderDTO>, List<AuthServiceNewRef.InstanceHeaderDTO>>(items.ToList());
            return _authorizationManager.GetInstanceHierarchys(entities);
        }
        public bool IsUserTenantAdmin()
        {
            return _authorizationManager.IsUserTenantAdmin();
        }
    }
}
