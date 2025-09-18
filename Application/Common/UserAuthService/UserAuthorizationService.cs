using AHA.IS.Common.Authorization.DTO.New;
using AutoMapper;
using CoreLib.Application.Common.Constants;
using CoreLib.Application.Common.Interfaces;
using CoreLib.Application.Common.Models;
using CoreLib.Application.Common.SqlEntities;
using CoreLib.Application.Common.Utility;
using Forgerock.SuperAdmin;
using Forgerock.SuperAdmin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Globalization;
using UserAuthServiceNew1;


namespace CoreLib.Application.Common.UserAuthService
{
    internal static partial class UserAuthorizationServiceMessages
    {
        [LoggerMessage(Level = LogLevel.Information, EventId = 1, Message = "method = {method}, elapsedMs = {elapsedMs}")]
        internal static partial void MethodElapsedMs(this ILogger logger, string method, double elapsedMs);

        [LoggerMessage(Level = LogLevel.Error, EventId = 1)]
        internal static partial void Exception(this ILogger logger, Exception ex);

        [LoggerMessage(Level = LogLevel.Information, EventId = 1, Message = "userName = {userName}, clientNKey = {clientNKey}, sessionID = {sessionNKey}, effectiveDate = {effectiveDate}, termDate = {termDate}, masterLoginIdentifier = {masterLoginIdentifier}")]
        internal static partial void SaveAuditInfoForGPAndArchive(this ILogger logger, string username, string clientNkey, string sessionNKey, string effectiveDate, string termDate, string masterLoginIdentifier);

        [LoggerMessage(Level = LogLevel.Information, EventId = 1, Message = "groupUserCrossWalkDTO = {groupUserCrossWalkDTO}")]
        internal static partial void SaveGroupUserCrossWalk(this ILogger logger, string groupUserCrossWalkDTO);

        [LoggerMessage(Level = LogLevel.Information, EventId = 1, Message = "logInSystemUserName = {logInSystemUserName}, classifiedSegmentInstanceNKeyWithStatus = {classifiedSegmentInstanceNKeyWithStatus}, lastUpdateUserNKey = {lastUpdateUserNKey}")]
        internal static partial void UpdateGroupUserCrossWalkActiveFlag(this ILogger logger, string logInSystemUserName, string classifiedSegmentInstanceNKeyWithStatus, string lastUpdateUserNKey);

        [LoggerMessage(Level = LogLevel.Information, EventId = 1, Message = "logInSystemUserName = {logInSystemUserName}, classifiedSegmentInstanceNKeys = {classifiedSegmentInstanceNKeys}, masterLoginSystemUserNKey = {masterLoginSystemUserNKey}, lastUpdateUserNKey = {lastUpdateUserNKey}")]
        internal static partial void UpdateGroupUserCrossWalkMasterLoginSystemUser(this ILogger logger, string logInSystemUserName, string classifiedSegmentInstanceNKeys, string masterLoginSystemUserNKey, string lastUpdateUserNKey);

        [LoggerMessage(Level = LogLevel.Information, EventId = 1, Message = "LoginSystemUserId = {LoginSystemUserId}, ClassifiedSegmentInstanceId = {ClassifiedSegmentInstanceId}, ConsentTypeId = {ConsentTypeId}, ConsentFlag = {ConsentFlag}, CreatedLoginUserId = {CreatedLoginUserId}")]
        internal static partial void AddOrUpdateUserConsent(this ILogger logger, int LoginSystemUserId, int ClassifiedSegmentInstanceId, int ConsentTypeId, bool ConsentFlag, string CreatedLoginUserId);
    }

    public class UserAuthorizationService(IMapper mapper, ISqlRepository _sqlRepository, IConfiguration _configuration, IFREndPoints _frEndPoints,
            ILogger<UserAuthorizationService> _logger, IFeatureService _featureService, IFeatureHandlerService _featureHandlerService, IAutoHandlerService _autoHandlerService,
            IProvisionHandlerService _provisionHandlerService, IHttpContextAccessor _httpContextAccessor) : IUserAuthorizationService
    {
        private static readonly int hours = 5;
        private static readonly int minutes = 30;

        public List<UserListBySearchCriteriaEntity> GetUserListBySearchCriteria(int systemId, SearchCriteriaDTO searchCriteria)
        {
            string userNames = GetSearchCriteria(SearchCriteriaHelpers.UserName, searchCriteria);
            string firstNames = GetSearchCriteria(SearchCriteriaHelpers.FirstName, searchCriteria);
            string lastNames = GetSearchCriteria(SearchCriteriaHelpers.LastName, searchCriteria);
            string emailaddresses = GetSearchCriteria(SearchCriteriaHelpers.EmailAddress, searchCriteria);
            string status = GetSearchCriteria(SearchCriteriaHelpers.Status, searchCriteria);
            return _sqlRepository.GetUserListBySearchCriteriaToList(systemId.ToString(CultureInfo.InvariantCulture), userNames,
                                                                lastNames?.Replace("'", "''", StringComparison.InvariantCultureIgnoreCase),
                                                                firstNames?.Replace("'", "''", StringComparison.InvariantCultureIgnoreCase),
                                                                emailaddresses,
                                                                status);
        }

        public static string GetSearchCriteria(string name, SearchCriteriaDTO searchCriteria)
        {
            var criteria = searchCriteria?.Criteria?.SingleOrDefault(x => string.Equals(x.SearchField, name, StringComparison.InvariantCultureIgnoreCase));
            if (criteria == null)
            {
                return null;
            }

            var criteriaString = criteria.SearchValue?.Trim();
            if (string.IsNullOrEmpty(criteriaString))
            {
                throw new InvalidDataException(name + " not specified");
            }
            return criteriaString;
        }

        public AHA.IS.Common.Authorization.DTO.New.UserDetailsDTO GetUserDetails(int userId, string remoteAddress)
        {
            using (UserAuthorizationServiceNewClient client = new(UserAuthorizationServiceNewClient.EndpointConfiguration.BasicHttpsBinding_IUserAuthorizationServiceNew, remoteAddress))
            {
                DateTime dt = DateTime.UtcNow;
                var result = client.GetUserDetails(userId);
                _logger.MethodElapsedMs("GetUserDetails", (DateTime.UtcNow - dt).TotalMilliseconds);
                return result;
            }
        }

        public SecurityAssignableItemHeaderDTO GetAssignableClientSelected(int userId, int systemId, string brandName, string cid, string clientName)
        {
            try
            {
                DateTime dt = DateTime.UtcNow;
                var result = GetAssignableClientInfoSQLWrapper(userId, systemId, brandName, cid, clientName);
                _logger.MethodElapsedMs("GetAssignableClientSelected", (DateTime.UtcNow - dt).TotalMilliseconds);

                return result;
            }
            catch (Exception)
            {
                return new SecurityAssignableItemHeaderDTO();
            }
        }

        public SecurityAssignableItemHeaderDTO GetAssignableClientInfoSQLWrapper(int userId, int systemId, string brandName, string cid, string clientName)
        {
            var userInfoDetails = _sqlRepository.GetClientsForBrand(systemId.ToString(CultureInfo.InvariantCulture), userId.ToString(CultureInfo.InvariantCulture),
                brandName, cid, clientName);
            SecurityAssignableItemHeaderDTO assignableItems =
                _provisionHandlerService.GetAssignableItemHeaders(userId, systemId, userInfoDetails.tenants,
                    userInfoDetails.userInfo.Select(csii => Int32.Parse(csii.AssignedClassifiedSegmentInstanceId, CultureInfo.InvariantCulture)).ToList());
            return assignableItems;
        }

        public List<UserAccessVerficationEntity> GetUserVerificationToList(string systemCode, string userName, string userRole)
        {
            return _sqlRepository.GetUserVerificationToList(systemCode, userName, userRole);
        }

        /// <summary>
        /// this method is used for fetching the User provision
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="systemId"></param>
        /// <param name="remoteAddress"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task DeactivateUserSystemAccess(int userId, string systemCode, DateTime terminationDate, string remoteAddress)
        {
            using (UserAuthorizationServiceNewClient client = new(UserAuthorizationServiceNewClient.EndpointConfiguration.BasicHttpsBinding_IUserAuthorizationServiceNew, remoteAddress))
            {
                //await Task.Run(() => client.DeactivateUserSystemAccess(userId, systemCode, terminationDate)); //This is needed for future reference. Please dont remove it
                client.DeactivateUserSystemAccess(userId, systemCode, terminationDate);
            }
        }

        public async Task ReactivateUserSystemAccess(int userId, string systemCode, DateTime effectiveDate, string BrandName, string remoteAddress)
        {
            using (UserAuthorizationServiceNewClient client = new(UserAuthorizationServiceNewClient.EndpointConfiguration.BasicHttpsBinding_IUserAuthorizationServiceNew, remoteAddress))
            {
                //await Task.Run(() => client.ReactivateUserSystemAccess(userId, systemCode, effectiveDate, BrandName));  //This is needed for future reference. Please dont remove it
                client.ReactivateUserSystemAccess(userId, systemCode, effectiveDate, BrandName);
            }
        }

        public async Task UpdateUserInformation(UserDTO userDetails, string remoteAddress)
        {
            using (UserAuthorizationServiceNewClient client = new(UserAuthorizationServiceNewClient.EndpointConfiguration.BasicHttpsBinding_IUserAuthorizationServiceNew, remoteAddress))
            {
                await Task.Run(() => client.UpdateUserInformation(userDetails));
            }
        }

        public async Task UpdateUserRole(int userId, string oldRole, string newRole, string remoteAddress)
        {
            using (UserAuthorizationServiceNewClient client = new(UserAuthorizationServiceNewClient.EndpointConfiguration.BasicHttpsBinding_IUserAuthorizationServiceNew, remoteAddress))
            {
                await Task.Run(() => client.UpdateUserRole(userId, oldRole, newRole));
            }
        }

        public async Task<UserAccessInfoDTO> GetSelectedUserAccessInfo(SearchCriteriaDTO searchCriteria, string orgId,
            string? userRole, int BrokerClientId, bool? IsMbrAvailable, bool? IsIndRptAvailable,
            bool? IsStlsRptAvailable, string actualUserName)
        {
            List<UserListBySearchCriteriaEntity> authSearchResult = GetUserListBySearchCriteria(ApplicationConstant.SYSTEM_ID, searchCriteria);
            UserListBySearchCriteriaEntity userDetails = authSearchResult.Find(x => x.Active == "true");
            DateTime dt = DateTime.UtcNow;

            var userInfo = await GetSelectedUserDTO(userDetails, orgId, BrokerClientId, IsMbrAvailable, IsIndRptAvailable, IsStlsRptAvailable, null, actualUserName);
            _logger.MethodElapsedMs("GetSelectedUserDTO", (DateTime.UtcNow - dt).TotalMilliseconds);
            dt = DateTime.UtcNow;

            UserAccessInfoDTO userAccessInfo = new()
            {
                CurrentVersion = AppSystemConst.CurrentVersion.GetTypeTableDisplayName(),
                AllClients = userInfo.AllClients,
                SelectedClientIds = userInfo.SelectedClientsTreeData.Rows.Select(x => new SelectedClientIdsDTO
                {
                    clientId = x.InstanceNkey,
                    clientName = x.Text,
                    clientStatus = x.ClientStatus,
                    clientBrand = x.ParentClassifiedSegmentName,
                    platformIndicator = x.PlatformIndicator,
                    icpClientInfo = userInfo.ICPClientInfo?[x.InstanceNkey]
                }).ToList()
            };

            //_featureHandlerService.GetSelectedClient(userAccessInfo, userInfo, BrokerClientId);
            if (!userInfo.AllClients)
            {
                foreach (var feature in userInfo.Features)
                {
                    var featureInfo = _featureService.UpdateFeatureAccessInfo(feature, BrokerClientId);
                    if (userRole == UserRoleConstant.StopLossCarrier)
                    {
                        if (featureInfo.FeatureName == Enums.SystemPermissionGroupSet.StoplossReports.GetTypeTableDisplayName() ||
                            featureInfo.FeatureName == Enums.SystemPermissionGroupSet.AccountProfile.GetTypeTableDisplayName() ||
                            feature.DisplayName == Common.Enums.SystemPermissionGroupSet.Resources.GetTypeTableDisplayName())
                        {
                            featureInfo.SubFeatures[0].HasAccess = true;
                        }
                        else
                        {
                            foreach (var subFeature in featureInfo.SubFeatures)
                            {
                                subFeature.HasAccess = false;
                            }
                        }
                    }
                    userAccessInfo.Features.Add(featureInfo);
                }
            }
            else
            {
                foreach (var feature in userInfo.Features)
                {

                    if (feature.DisplayName == Common.Enums.SystemPermissionGroupSet.UserAdmin.GetTypeTableDisplayName()
                        || feature.DisplayName == Common.Enums.SystemPermissionGroupSet.Resources.GetTypeTableDisplayName()
                        || feature.DisplayName == Common.Enums.SystemPermissionGroupSet.Providers.GetTypeTableDisplayName())
                    {
                        userAccessInfo.Features.Add(_featureService.UpdateFeatureAccessInfo(feature, ApplicationConstant.BROKER_CLIENT_ID_DEFAULT));
                    }
                }
            }
            _featureHandlerService.AddCustomFeatures(userAccessInfo.Features, userRole);

            _logger.MethodElapsedMs("GetSelectedUserAccessInfo", (DateTime.UtcNow - dt).TotalMilliseconds);
            userAccessInfo.RID = userInfo.RID;
            userAccessInfo.financialManager = userInfo.financialManager;

            return userAccessInfo;
        }

        public async Task<UserFeatureAccessPermissionDTO> GetSelectedUserDTO(UserListBySearchCriteriaEntity userDetails,
                                                                string orgId, int BrokerClientId, bool? IsMbrAvailable,
                                                                bool? IsIndRptAvailable, bool? IsStlsRptAvailable,
                                                                bool? newUser, string actualUserName, GetUUIDResponse FRUsersList = null)
        {
            DateTime dt1 = DateTime.UtcNow;
            bool isAdmin = userDetails?.UserName == ApplicationConstant.DUMMY_ADMIN;
            bool isBroker = userDetails?.UserName == ApplicationConstant.DUMMY_BROKER;
            Models.UserDetailsDTO userDetailsFR = null;
            if (!isAdmin && !isBroker)
            {
                userDetailsFR = await GetUsersOfFR(_configuration, orgId, actualUserName, FRUsersList);
            }
            _logger.MethodElapsedMs("GetSelectedUserDTO-GetUserOfFR", (DateTime.UtcNow - dt1).TotalMilliseconds);
            dt1 = DateTime.UtcNow;
            UserApplicationDetailsDTO userInfo = new();
            if (userDetails != null)
            {
                if (isBroker)
                {
                    List<string> clientNKeys = [BrokerClientId.ToString(CultureInfo.InvariantCulture)];
                    userInfo = _provisionHandlerService.GetUserApplicationDetailsWithSelectedClientsNKey(Int32.Parse(userDetails.LoginSystemUserId, CultureInfo.InvariantCulture),
                                                                                ApplicationConstant.SYSTEM_ID,
                                                                                _configuration.GetSection(AppSettingsConstants.ConfigKeyOrgGuid).GetValue<string>(orgId),
                                                                                clientNKeys);
                }
                else if (newUser != null && newUser == true)
                {
                    userInfo = _provisionHandlerService.GetNewUserApplicationDetails(Int32.Parse(userDetails.LoginSystemUserId, CultureInfo.InvariantCulture),
                                                                ApplicationConstant.SYSTEM_ID, _configuration.GetSection(AppSettingsConstants.ConfigKeyOrgGuid).GetValue<string>(orgId));
                }
                else
                {
                    userInfo = _provisionHandlerService.GetUserApplicationDetails(Int32.Parse(userDetails.LoginSystemUserId, CultureInfo.InvariantCulture),
                                                                ApplicationConstant.SYSTEM_ID, _configuration.GetSection(AppSettingsConstants.ConfigKeyOrgGuid).GetValue<string>(orgId));
                }
            }
            _logger.MethodElapsedMs("GetSelectedUserDTO-ProvisionApplicationDetails", (DateTime.UtcNow - dt1).TotalMilliseconds);
            dt1 = DateTime.UtcNow;

            var dto = mapper.Map<UserApplicationDetailsDTO, UserFeatureAccessPermissionDTO>(userInfo);
            if ((userInfo != null && userInfo.LoginSystemUserId > 0) || (isAdmin) || (isBroker))
            {
                var accessibleItems = userInfo.AssignableItemsHeader.AssignableItems.Where(x => x.InstanceNKey == _configuration.GetSection(AppSettingsConstants.ConfigKeyOrgGuid).GetValue<string>(orgId)).ToList();
                var tenants = _featureHandlerService.RecursiveGetAssignableItems("Tenant", accessibleItems);
                var clients = _featureHandlerService.RecursiveGetAssignableItems("Client", accessibleItems);
                dto.AllClients = false;

                if (isAdmin)
                {
                    dto.AllClients = true;
                }
                else if (isBroker)
                {
                    dto.SelectedClientIds = clients.Where(x => x.InstanceNKey == BrokerClientId.ToString(CultureInfo.InvariantCulture)).Select(x => x.ClassifiedSegmentInstanceId).ToList();
                    //dto.SelectedClientIds.Add(BrokerClientId);
                }
                else if (userDetailsFR.UserSystemAccesses.Any(x => ApplicationConstant.STOPLOSS_ROLE.Split(',').Contains(x.Role)))
                {
                    dto.SelectedClientIds = [];

                }
                else if (tenants.TrueForAll(x => x.SelectedForUser) || !clients.Exists(x => x.SelectedForUser))
                {
                    dto.SelectedClientIds = [];
                }
                else
                {
                    dto.SelectedClientIds = clients.Where(x => x.SelectedForUser).Select(x => x.ClassifiedSegmentInstanceId).ToList();
                }

                if (dto.SelectedClientIds.Count > 0)
                {
                    dto.SelectedClientsTreeData = _provisionHandlerService.GetSelectableClients(clients.Where(x => dto.SelectedClientIds.Contains(x.ClassifiedSegmentInstanceId)).ToList(), _configuration.GetSection(AppSettingsConstants.ConfigKeyOrgGuid).GetValue<string>(orgId));
                    var selectedClientsTreeData = dto.SelectedClientsTreeData;

                    foreach (TreeGridRowDTO<int> client in selectedClientsTreeData.Rows)
                    {

                        dto.SelectedClientsTreeData.Rows.First(x => x.InstanceNkey == client.InstanceNkey).IsAllSelected = _autoHandlerService.IsAutoProvisionsConcent(userInfo.LoginSystemUserId, client.InstanceNkey);
                    }

                }
                DateTime dt = DateTime.UtcNow;
                var GetAllFeatures = _sqlRepository.GetAllFeaturesToList(Convert.ToString(ApplicationConstant.SYSTEM_ID, CultureInfo.InvariantCulture));
                _logger.MethodElapsedMs("GetAllFeaturesToList", (DateTime.UtcNow - dt).TotalMilliseconds);
                dt = DateTime.UtcNow;
                string? orgName = _configuration.GetSection(AppSettingsConstants.ConfigKeyOrgGuid).GetValue<string>(orgId);
                foreach (var featureSelection in userInfo.UserFeatureSelections)
                {
                    var featureInfo = _provisionHandlerService.GetFeature(featureSelection.SystemPermissionGroupSetId, GetAllFeatures);
                    var selections = featureInfo.FeatureSelections.Select(
                        x =>
                            new UserManagementEditFeatureSelectionDTO
                            {
                                Id = x.SystemPermissionGroupSetGroupingId,
                                Value = x.LabelName,
                                PermissionCode = x.PermissionCode
                            }
                        ).ToList();
                    if (featureInfo.Name == Enums.SystemPermissionGroupSet.EDI.GetTypeTableDisplayName())
                    {
                        selections = [];
                    }

                    dto.Features.Add(
                        _featureHandlerService.UpdateHardcodeFeature(
                       featureInfo,
                       new UserManagementEditFeatureDTO
                       {
                           DisplayName = featureInfo.Name,
                           FeatureId = featureInfo.SystemPermissionGroupSetId,
                           SelectedOptionId = _featureHandlerService.GetSelectedOptionIdForName(new GetSelectedOptionIdForNameModel
                           {
                               featureInfo = featureInfo,
                               userInfo = userInfo,
                               featureSelection = featureSelection,
                               dto = dto,
                               BrokerClientId = BrokerClientId,
                               IsMbrAvailable = IsMbrAvailable,
                               IsIndRptAvailable = IsIndRptAvailable,
                               IsStlsRptAvailable = IsStlsRptAvailable
                           }),
                           SelectedOptionIds = featureInfo.Name == Enums.SystemPermissionGroupSet.ClientReports.GetTypeTableDisplayName()
                                                            ? _featureHandlerService.GetSelectedOptionIds(featureSelection) : null,
                           Selections = selections,
                           AllowsCustomOption = featureInfo.AllowsCustomAccounts,
                           CustomTreeViewOptions = _provisionHandlerService.GetFeatureAccessModel(featureInfo, userInfo, dto, BrokerClientId, orgName)
                       }
                    ));
                    if (!isAdmin && !isBroker && userDetailsFR != null &&
                        userDetailsFR.UserSystemAccesses.Any(x => x.Role.Equals(UserRoleConstant.StopLossCarrier, StringComparison.InvariantCultureIgnoreCase))
                            && (featureInfo.Name == Enums.SystemPermissionGroupSet.StoplossReports.GetTypeTableDisplayName() ||
                                featureInfo.Name == Enums.SystemPermissionGroupSet.AccountProfile.GetTypeTableDisplayName()))
                    {
                        dto.Features.Last().SelectedOptionId = featureSelection.SystemPermissionGroupSetId;
                    }
                }
                if (dto.TerminationDate != null)
                {
                    dto.TerminationDate = Convert.ToDateTime(dto.TerminationDate, CultureInfo.InvariantCulture).AddHours(hours).AddMinutes(minutes);
                }
                _logger.MethodElapsedMs("UserFeatureSelections Iteration", (DateTime.UtcNow - dt).TotalMilliseconds);

                // Update the features with custom options:
                dt = DateTime.UtcNow;
                foreach (var feature in dto.Features)
                {
                    _provisionHandlerService.UpdateFeatureCustomRow(dto, feature);
                }
                _logger.MethodElapsedMs("UpdateFeatureCustomRow", (DateTime.UtcNow - dt).TotalMilliseconds);

            }
            if (!isAdmin && !isBroker)
            {
                dto = dto.GetVersionizedFeatures(_configuration, userDetailsFR.UserSystemAccesses.Any(x => x.Role.Equals(UserRoleConstant.StopLossCarrier, StringComparison.InvariantCultureIgnoreCase)));
            }
            string RequestGUID = System.Guid.NewGuid().ToString();
            DateTime dtt = DateTime.UtcNow;
            bool isAutoProvision = false;
            dto.IsAllAcountSubAccount = new List<ClientAccountSelection>();
            foreach (var client in dto.SelectedClientsTreeData?.Rows)
            {
                if (_autoHandlerService.IsAutoProvisionsConcent(dto.UserId, client.Id.ToString(CultureInfo.InvariantCulture)))
                {
                    isAutoProvision = true;
                    dto.IsAllAcountSubAccount.Add(new ClientAccountSelection { clientNkey = client.InstanceNkey.ToString(CultureInfo.InvariantCulture), isAllAccountsSubAccountsSelected = true });
                }
                else
                {
                    dto.IsAllAcountSubAccount.Add(new ClientAccountSelection { clientNkey = client.InstanceNkey.ToString(CultureInfo.InvariantCulture), isAllAccountsSubAccountsSelected = false });
                }
            }
            // Need Business confirmation on triggerring autoprovision when super admin come on to user Profile. if Yes, Then we can uncomment the below condition
            if (_httpContextAccessor.HttpContext.Request.Path == "/api/v1/User/GetUserDetails" && !isAdmin && !isBroker && !userDetailsFR.UserSystemAccesses.Any(x => x.Role.Equals("stoplosscarrier", StringComparison.InvariantCultureIgnoreCase)))
            {
                if (dto.SelectedClientsTreeData?.Rows?.First()?.InstanceNkey != "1722822")
                {

                    if (isAutoProvision)
                    {
                        Task.Run(() => _autoHandlerService.AutoProvisioning(dto, RequestGUID));
                    }
                }
            }
            _logger.MethodElapsedMs("GetSelectedUserDTO-AutoProvisionCheck", (DateTime.UtcNow - dtt).TotalMilliseconds);


            dto.RID = RequestGUID;
            dtt = DateTime.UtcNow;
            if (!isBroker)
            {
                dto = _provisionHandlerService.GetPlatformIndicatorUpdated(dto);
            }
            else
            {
                dto = _provisionHandlerService.GetPlatformIndicatorUpdated(dto, BrokerClientId.ToString(CultureInfo.InvariantCulture));
            }

            _logger.MethodElapsedMs("GetSelectedUserDTO-ProvisionsSet", (DateTime.UtcNow - dt1).TotalMilliseconds);

            List<GroupPortalCrossWalkDTO> CrossWalkInfo = _sqlRepository.GetUserCrossWalkInfo(userInfo.LoginSystemUserId);
            // Fix for CS0029: Cannot implicitly convert type 'string' to 'bool'
            // Change:
            // bool financialManager = CrossWalkInfo?.FirstOrDefault() is GroupPortalCrossWalkDTO crossWalk
            //     ? crossWalk.AllowAllBillToAccountsFlag
            //     : false;
            //
            // To:
            bool financialManager = CrossWalkInfo?.FirstOrDefault() is GroupPortalCrossWalkDTO crossWalk
                ? string.Equals(crossWalk.AllowAllBillToAccountsFlag, "true", StringComparison.InvariantCultureIgnoreCase)
                : false;

            dto.financialManager = financialManager;

            return dto;
        }

        public string? GetAutoProvisionStatus(string RID)
        {
            return _autoHandlerService.AutoProvisionStatus(RID);
        }

        private static UserFeatureAccessPermissionDTO RemoveNonICPInfo(UserFeatureAccessPermissionDTO dto, List<AccountDto>? icpAccountDetails)
        {
            UserFeatureAccessPermissionDTO modifiedDto = dto;
            foreach (var feature in dto.Features)
            {
                foreach (var client in feature?.CustomTreeViewOptions.Rows)
                {
                    foreach (var account in client.Children)
                    {
                        AccountDto? selectedICPAccount = icpAccountDetails?.Find(x => x.AccountName == account.InstanceNkey);
                        if (selectedICPAccount != null)
                        {
                            foreach (var subAccount in account.Children)
                            {
                                if (!selectedICPAccount.Subaccounts.Any(x => x.SubaccountId == subAccount.InstanceNkey))
                                {
                                    modifiedDto.Features.First(x => x.FeatureId == feature.FeatureId)
                                    .CustomTreeViewOptions.Rows.First(x => x.InstanceNkey == client.InstanceNkey)
                                        .Children.First(y => y.InstanceNkey == account.InstanceNkey).Children.Remove(subAccount);
                                    modifiedDto.Features.First(x => x.FeatureId == feature.FeatureId)
                                       .CustomTreeViewOptions.Rows.First(x => x.InstanceNkey == client.InstanceNkey).SubAccountCount =
                                       modifiedDto.Features.First(x => x.FeatureId == feature.FeatureId)
                                       .CustomTreeViewOptions.Rows.First(x => x.InstanceNkey == client.InstanceNkey).SubAccountCount > 0
                                           ? modifiedDto.Features.First(x => x.FeatureId == feature.FeatureId)
                                           .CustomTreeViewOptions.Rows.First(x => x.InstanceNkey == client.InstanceNkey).SubAccountCount - 1
                                           : 0;
                                }
                            }
                        }
                        else
                        {
                            modifiedDto.Features.First(x => x.FeatureId == feature.FeatureId)
                                .CustomTreeViewOptions.Rows.First(x => x.InstanceNkey == client.InstanceNkey)
                                    .Children.Remove(account);
                            modifiedDto.Features.First(x => x.FeatureId == feature.FeatureId)
                                .CustomTreeViewOptions.Rows.First(x => x.InstanceNkey == client.InstanceNkey).AccountCount =
                                modifiedDto.Features.First(x => x.FeatureId == feature.FeatureId)
                                .CustomTreeViewOptions.Rows.First(x => x.InstanceNkey == client.InstanceNkey).AccountCount > 0
                                    ? modifiedDto.Features.First(x => x.FeatureId == feature.FeatureId)
                                    .CustomTreeViewOptions.Rows.First(x => x.InstanceNkey == client.InstanceNkey).AccountCount - 1
                                    : 0;
                        }
                    }
                }
            }
            return modifiedDto;
        }

        public string SaveAuditInfoForGPAndArchive(string _userName, string _clientNKey, string _SessionNKey, string _effectiveDateISO, string _TermDateISO)
        {
            string resultant = string.Empty;
            try
            {
                var XwalkData = _sqlRepository.GetMasterLoginIdentifier(_userName);
                var clientXWalkData = XwalkData.Find(x => x.ClassifiedAreaSegmentNKey == _clientNKey);
                bool allowAllBillToAccountsFlag = clientXWalkData != null &&
                   string.Equals(clientXWalkData.AllowAllBillToAccountsFlag, "true", StringComparison.InvariantCultureIgnoreCase);
                if (clientXWalkData != null && !allowAllBillToAccountsFlag)
                {

                    string masterLoginIdentifier = clientXWalkData != null ? (string.IsNullOrEmpty(clientXWalkData.MasterLoginSystemUserId) ? clientXWalkData.MasterLoginSystemUserId : clientXWalkData.MasterLoginSystemUserNKey) : string.Empty;

                    using (UserAuthorizationServiceNewClient client = new(
                        UserAuthorizationServiceNewClient.EndpointConfiguration.BasicHttpsBinding_IUserAuthorizationServiceNew,
                         _configuration.GetValue<string>("USER_AUTHORIZATION_ENDPOINT")))
                    {
                        _logger.SaveAuditInfoForGPAndArchive(_userName, _clientNKey, _SessionNKey, _effectiveDateISO, _TermDateISO, masterLoginIdentifier);

                        client.SaveAuditInfoForGPAndArchive(_userName, _clientNKey, _SessionNKey, _effectiveDateISO, _TermDateISO, masterLoginIdentifier);

                        resultant = string.Format("Result of Execution of Save Audit for Group Portal:Success at {0}", DateTime.Now.ToString(CultureInfo.InvariantCulture));
                    }
                }
                else if (allowAllBillToAccountsFlag)
                {
                    foreach(var item in XwalkData)
                    {
                        using (UserAuthorizationServiceNewClient client = new(
                            UserAuthorizationServiceNewClient.EndpointConfiguration.BasicHttpsBinding_IUserAuthorizationServiceNew,
                             _configuration.GetValue<string>("USER_AUTHORIZATION_ENDPOINT")))
                        {
                            _logger.SaveAuditInfoForGPAndArchive(_userName, item.ClassifiedAreaSegmentNKey, _SessionNKey, _effectiveDateISO, _TermDateISO, _userName);
                            client.SaveAuditInfoForGPAndArchive(_userName, item.ClassifiedAreaSegmentNKey, _SessionNKey, _effectiveDateISO, _TermDateISO, _userName);
                        }
                    }
                }
                else if(clientXWalkData == null)
                {
                    using (UserAuthorizationServiceNewClient client = new(
                            UserAuthorizationServiceNewClient.EndpointConfiguration.BasicHttpsBinding_IUserAuthorizationServiceNew,
                             _configuration.GetValue<string>("USER_AUTHORIZATION_ENDPOINT")))
                    {
                        _logger.SaveAuditInfoForGPAndArchive(_userName, _clientNKey, _SessionNKey, _effectiveDateISO, _TermDateISO, _userName);
                        client.SaveAuditInfoForGPAndArchive(_userName, _clientNKey, _SessionNKey, _effectiveDateISO, _TermDateISO, _userName);
                    }
                }
            }
            catch (Exception ex)
            {
                resultant = string.Format("Error at {0} with Message: {1}", DateTime.Now.ToString(CultureInfo.InvariantCulture), ex.Message);
                _logger.LogError(resultant);
                throw;

            }
            return resultant;
        }



        public void AddOrUpdateUserConsent(int LoginSystemUserId, int ClassifiedSegmentInstanceId, int ConsentTypeId, bool ConsentFlag, string CreatedLoginUserId)
        {
            using (UserAuthorizationServiceNewClient client = new(
                UserAuthorizationServiceNewClient.EndpointConfiguration.BasicHttpsBinding_IUserAuthorizationServiceNew,
                 _configuration.GetValue<string>("USER_AUTHORIZATION_ENDPOINT")))
            {
                _logger.AddOrUpdateUserConsent(LoginSystemUserId, ClassifiedSegmentInstanceId, ConsentTypeId, ConsentFlag, CreatedLoginUserId);

                client.AddOrUpdateUserConsent(LoginSystemUserId, ClassifiedSegmentInstanceId, ConsentTypeId, ConsentFlag, CreatedLoginUserId);
            }
        }

        public async Task<Models.UserDetailsDTO> GetUsersOfFR(IConfiguration configuration, string OrgId, string userName, GetUUIDResponse FRUsersList = null)
        {
            SearchWith search = new()
            {
                Criteria = [new Criteria { SearchField = "username", SearchValue = userName }]
            };
            bool isAdmin = userName == ApplicationConstant.DUMMY_ADMIN;
            bool isBroker = userName == ApplicationConstant.DUMMY_BROKER;

            List<Models.UserDetailsDTO> searchResult = [];
            if (!isAdmin && !isBroker)
            {
                GetUUIDResponse GetFRUsersList;
                if (FRUsersList == null)
                {
                    GetFRUsersList = await _frEndPoints.GetUsersOfFR(configuration, OrgId, search);
                    string orgName = _configuration.GetSection(AppSettingsConstants.ConfigKeyOrgGuid).GetValue<string>(OrgId);
                    GetUUIDResponse BrokerUserList = orgName switch
                    {
                        "IBX" => await _frEndPoints.GetUsersOfFR(_configuration, _configuration.GetValue<string>("BROKER_ORG_ID_IBX"), search),
                        "AH" => await _frEndPoints.GetUsersOfFR(_configuration, _configuration.GetValue<string>("BROKER_ORG_ID_AH"), search),
                        _ => throw new InvalidOperationException("Invalid organization name"),
                    };
                    GetFRUsersList.result.AddRange(BrokerUserList.result);
                    GetFRUsersList.resultCount += BrokerUserList.resultCount;
                }
                else
                {
                    GetFRUsersList = FRUsersList;
                }
                searchResult = mapper.Map<List<UserDetails>, List<Models.UserDetailsDTO>>(GetFRUsersList.result.ToList());
            }
            Models.UserDetailsDTO userDetailsDTO = searchResult.FirstOrDefault();

            if (userDetailsDTO != null || isAdmin || isBroker)
            {
                var searchCriteria = new SearchCriteriaDTO
                {
                    Criteria = [
                        new UserSearchResultDTO
                        {
                            SearchField = "UserName",
                            SearchValue = userName
                        }
                    ]
                };

                var authSearchResult = GetUserListBySearchCriteria(ApplicationConstant.SYSTEM_ID, searchCriteria);

                foreach (var item in searchResult)
                {
                    if (authSearchResult.Find(x => x.UserName.Equals(userDetailsDTO.UserName, StringComparison.InvariantCultureIgnoreCase)) != null)
                    {
                        var userDetails = authSearchResult.Find(x => x.UserName.Equals(item.UserName, StringComparison.InvariantCultureIgnoreCase));
                        userDetailsDTO.UserId = authSearchResult.Find(x => x.UserName.Equals(userDetailsDTO.UserName, StringComparison.InvariantCultureIgnoreCase)).LoginSystemUserId.ToString(CultureInfo.InvariantCulture);
                        userDetailsDTO.Status = item.Status = item.UserSystemAccesses.Any(x => ApplicationConstant.SYSTEM_MANAGED_USER.Split(',').ToList().Contains(x.Role))
                                    ? item.Status
                                    : (item.Status ? userDetails.Active == "true" : item.Status);
                    }
                    else
                    {
                        userDetailsDTO.Status = item.UserSystemAccesses.Any(x => ApplicationConstant.SYSTEM_MANAGED_USER.Split(',').ToList().Contains(x.Role)) && item.Status;
                    }
                }
            }
            return userDetailsDTO;
        }

        public ClientAccessInfoDTO GetAuthorizedClientAccessInfo(string userName, string clientId, string accessTypes)
        {
            UserAccessInfoDTO userAccess = new();
            var featureData = _sqlRepository.GetUserInfoOnPermissionCodeToList(Convert.ToString(ApplicationConstant.SYSTEM_ID, CultureInfo.InvariantCulture),
                                                                                userName, accessTypes);
            userAccess.Features = UserAccessStruct(featureData);

            var feature = userAccess.Features.SingleOrDefault(x => x.SubFeatures.Any(y => y.PermissionCode == accessTypes
                                                                    && y.Clients.Any(z => z.ClientId == clientId)
                                                                    && y.HasAccess == true));
            var subfeature = feature?.SubFeatures.SingleOrDefault(y => y.PermissionCode == accessTypes
                                                                    && y.Clients.Any(z => z.ClientId == clientId)
                                                                    && y.HasAccess == true);
            var assignedAccSubAccList = subfeature?.Clients.SingleOrDefault(z => z.ClientId == clientId && z.Accounts.Count != 0);

            return assignedAccSubAccList;
        }

        private static List<FeatureAccessInfoDTO> UserAccessStruct(List<UserInfoOnPermissionCode> featureData)
        {
            var structData = from client in featureData
                             group client by client.ClientId into clientGrp
                             select new FeatureAccessInfoDTO
                             {
                                 FeatureId = Convert.ToInt32(clientGrp.First().FeatureId, CultureInfo.InvariantCulture),
                                 FeatureName = clientGrp.First().FeatureName,
                                 SubFeatures = [ new() {
                                                SubFeatureId = Convert.ToInt32(clientGrp.First().SubFeatureId, CultureInfo.InvariantCulture),
                                                SubFeatureName = clientGrp.First().SubFeatureName,
                                                PermissionCode = clientGrp.First().PermissionCode,
                                                HasAccess=Convert.ToBoolean(clientGrp.First().HasAccess, CultureInfo.InvariantCulture),
                                                Clients = [
                                                    new ClientAccessInfoDTO {
                                                        ClientId = clientGrp.Key,
                                                        Accounts = (from account in clientGrp.AsEnumerable()
                                                                    where Convert.ToInt32(account.AccountId, CultureInfo.InvariantCulture) != 0
                                                                    group account by account.AccountId into accountGrp
                                                                    select new AccountAccessInfoDTO
                                                                    {
                                                                        AccountId = accountGrp.Key,
                                                                        SubAccounts = (from subaccount in
                                                                        accountGrp.AsEnumerable()
                                                                        where
                                                                        Convert.ToInt32(subaccount.SubAccountId) != 0
                                                                        select new SubAccountAccessInfoDTO
                                                                        {
                                                                            SubAccountId = subaccount.SubAccountId
                                                                        }).ToList(),
                                                                    }).ToList()
                                                    }
                                                ]
                                            }
                                        ]
                             };

            return structData.ToList();
        }

        public IList<AccountAccessInfoDTO> GetAuthorizedAccountsAndSubaccounts(string userName, string clientId)
        {
            UserAccessInfoDTO userAccess = new();
            var featureData = _sqlRepository.GetUserInfoForClientToList(Convert.ToString(ApplicationConstant.SYSTEM_ID, CultureInfo.InvariantCulture),
                                                                                    userName, clientId);
            userAccess.Features = UserAccessStructByFeatures(featureData);

            var assignedAccSubAccList = userAccess.Features
                .SelectMany(f => f.SubFeatures)
                .Where(sf => sf.HasAccess == true)
                .SelectMany(sf => sf.Clients)
                .Where(c => c.Accounts.Count != 0)
                .SelectMany(c => c.Accounts);

            // Get distinct accounts and subaccounts
            var distinctAccounts = assignedAccSubAccList
                .GroupBy(a => a.AccountId)
                .Select(g => new AccountAccessInfoDTO
                {
                    AccountId = g.Key,
                    SubAccounts = g.SelectMany(a => a.SubAccounts)
                                   .GroupBy(sa => sa.SubAccountId)
                                   .Select(sg => sg.First())
                                   .ToList()
                })
                .ToList();

            return distinctAccounts;
        }

        private static List<FeatureAccessInfoDTO> UserAccessStructByFeatures(List<UserInfoOnPermissionCode> featureData)
        {
            var structData = from client in featureData
                             group client by client.FeatureId into featureGrp
                             select new FeatureAccessInfoDTO
                             {
                                 FeatureId = Convert.ToInt32(featureGrp.First().FeatureId, CultureInfo.InvariantCulture),
                                 FeatureName = featureGrp.First().FeatureName,
                                 SubFeatures = [ new() {
                                                SubFeatureId = Convert.ToInt32(featureGrp.First().SubFeatureId, CultureInfo.InvariantCulture),
                                                SubFeatureName = featureGrp.First().SubFeatureName,
                                                PermissionCode = featureGrp.First().PermissionCode,
                                                HasAccess=Convert.ToBoolean(featureGrp.First().HasAccess, CultureInfo.InvariantCulture),
                                                Clients = [
                                                    new ClientAccessInfoDTO {
                                                        ClientId = featureGrp.Key,
                                                        Accounts = (from account in featureGrp.AsEnumerable()
                                                                    where Convert.ToInt32(account.AccountId, CultureInfo.InvariantCulture) != 0
                                                                    group account by account.AccountId into accountGrp
                                                                    select new AccountAccessInfoDTO
                                                                    {
                                                                        AccountId = accountGrp.Key,
                                                                        SubAccounts = (from subaccount in
                                                                        accountGrp.AsEnumerable()
                                                                        where
                                                                        Convert.ToInt32(subaccount.SubAccountId) != 0
                                                                        select new SubAccountAccessInfoDTO
                                                                        {
                                                                            SubAccountId = subaccount.SubAccountId
                                                                        }).ToList(),
                                                                    }).ToList()
                                                    }
                                                ]
                                            }
                                        ]
                             };

            return structData.ToList();
        }
        public GroupUserCrossWalkResponse SaveGroupUserCrossWalk(GroupUserCrossWalkDTO groupUserCrossWalkDTO)
        {
            string resultant = string.Empty;
            try
            {
                using (UserAuthorizationServiceNewClient client = new(
                    UserAuthorizationServiceNewClient.EndpointConfiguration.BasicHttpsBinding_IUserAuthorizationServiceNew,
                     _configuration.GetValue<string>("USER_AUTHORIZATION_ENDPOINT")))
                {
                    _logger.SaveGroupUserCrossWalk(JsonConvert.SerializeObject(groupUserCrossWalkDTO));

                    return client.SaveGroupUserCrossWalk(groupUserCrossWalkDTO, ApplicationConstant.SYSTEM_ID.ToString());
                }
            }
            catch (Exception ex)
            {
                resultant = string.Format("Error at {0} with Message: {1}", DateTime.Now.ToString(CultureInfo.InvariantCulture), ex.Message);
                _logger.LogError(resultant);
                throw;

            }
        }

        public GroupUserCrossWalkResponse UpdateGroupUserCrossWalkActiveFlag(string logInSystemUserName, ClassifiedSegmentInstanceNKeyWIthStatus[] classifiedSegmentInstanceNKeyWithStatus, string lastUpdateUserNKey)
        {
            string resultant = string.Empty;
            try
            {
                using (UserAuthorizationServiceNewClient client = new(
                    UserAuthorizationServiceNewClient.EndpointConfiguration.BasicHttpsBinding_IUserAuthorizationServiceNew,
                     _configuration.GetValue<string>("USER_AUTHORIZATION_ENDPOINT")))
                {
                    _logger.UpdateGroupUserCrossWalkActiveFlag(logInSystemUserName, JsonConvert.SerializeObject(classifiedSegmentInstanceNKeyWithStatus), lastUpdateUserNKey);

                    return client.UpdateGroupUserCrossWalkActiveFlag(logInSystemUserName, classifiedSegmentInstanceNKeyWithStatus, lastUpdateUserNKey, ApplicationConstant.SYSTEM_ID.ToString(), null);
                }
            }
            catch (Exception ex)
            {
                resultant = string.Format("Error at {0} with Message: {1}", DateTime.Now.ToString(CultureInfo.InvariantCulture), ex.Message);
                _logger.LogError(resultant);
                throw;

            }
        }

        public GroupUserCrossWalkResponse UpdateGroupUserCrossWalkActiveFlagWithAllowAllBillsToAccount(string logInSystemUserName, ClassifiedSegmentInstanceNKeyWIthStatus[] classifiedSegmentInstanceNKeyWithStatus, string lastUpdateUserNKey, bool allAllBillsToAccountFlag)
        {
            string resultant = string.Empty;
            try
            {
                using (UserAuthorizationServiceNewClient client = new(
                    UserAuthorizationServiceNewClient.EndpointConfiguration.BasicHttpsBinding_IUserAuthorizationServiceNew,
                     _configuration.GetValue<string>("USER_AUTHORIZATION_ENDPOINT")))
                {
                    _logger.UpdateGroupUserCrossWalkActiveFlag(logInSystemUserName, JsonConvert.SerializeObject(classifiedSegmentInstanceNKeyWithStatus), lastUpdateUserNKey);

                    return client.UpdateGroupUserCrossWalkActiveFlag(logInSystemUserName, classifiedSegmentInstanceNKeyWithStatus, lastUpdateUserNKey, ApplicationConstant.SYSTEM_ID.ToString(), allAllBillsToAccountFlag);
                }
            }
            catch (Exception ex)
            {
                resultant = string.Format("Error at {0} with Message: {1}", DateTime.Now.ToString(CultureInfo.InvariantCulture), ex.Message);
                _logger.LogError(resultant);
                throw;

            }
        }

        public GroupUserCrossWalkResponse UpdateGroupUserCrossWalkMasterLoginSystemUser(string logInSystemUserName, string[] classifiedSegmentInstanceNKeys, string masterLoginSystemUserNKey, string lastUpdateUserNKey)
        {
            string resultant = string.Empty;
            try
            {
                using (UserAuthorizationServiceNewClient client = new(
                    UserAuthorizationServiceNewClient.EndpointConfiguration.BasicHttpsBinding_IUserAuthorizationServiceNew,
                     _configuration.GetValue<string>("USER_AUTHORIZATION_ENDPOINT")))
                {
                    _logger.UpdateGroupUserCrossWalkMasterLoginSystemUser(logInSystemUserName, classifiedSegmentInstanceNKeys.SerializeObjectToJson().ToString(), masterLoginSystemUserNKey, lastUpdateUserNKey);

                    return client.UpdateGroupUserCrossWalkMasterLoginSystemUser(logInSystemUserName, classifiedSegmentInstanceNKeys, masterLoginSystemUserNKey, lastUpdateUserNKey, ApplicationConstant.SYSTEM_ID.ToString());
                }
            }
            catch (Exception ex)
            {
                resultant = string.Format("Error at {0} with Message: {1}", DateTime.Now.ToString(CultureInfo.InvariantCulture), ex.Message);
                _logger.LogError(resultant);
                throw;
            }
        }
        public string DisableCrossWalkUser(string _userName, string loginUser)
        {
            string resultant = string.Empty;
            try
            {
                var masterLoginIdentifier = _sqlRepository.GetAllClientNKeyAssociatedToUser(_userName);


                ClassifiedSegmentInstanceNKeyWIthStatus[] classifiedSegmentInstanceNKeyWithStatusList = new ClassifiedSegmentInstanceNKeyWIthStatus[masterLoginIdentifier.Count];
                for (int i = 0; i < masterLoginIdentifier.Count; i++)
                {
                    var item = masterLoginIdentifier[i];
                    classifiedSegmentInstanceNKeyWithStatusList[i] = new ClassifiedSegmentInstanceNKeyWIthStatus
                    {
                        ClassifiedSegmentInstanceNKey = item.ClassifiedAreaSegmentNKey,
                        ActiveFlag = false,
                        MasterLoginSystemUserNKey = string.Empty
                    };
                }

                UpdateGroupUserCrossWalkActiveFlag(_userName, classifiedSegmentInstanceNKeyWithStatusList, loginUser);
            }
            catch (Exception ex)
            {
                resultant = string.Format("Error at {0} with Message: {1}", DateTime.Now.ToString(CultureInfo.InvariantCulture), ex.Message);
                _logger.LogError(resultant);
                throw;

            }
            return resultant;
        }


        public List<GroupPortalCrossWalkDTO> GetUserCrossWalkInformation(int userId)
        {
            List<GroupPortalCrossWalkDTO> resultant = new();
            try
            {
                return _sqlRepository.GetUserCrossWalkInfo(userId);

            }
            catch (Exception ex)
            {
                //resultant = string.Format("Error at {0} with Message: {1}", DateTime.Now.ToString(CultureInfo.InvariantCulture), ex.Message);
                _logger.LogError("GetUserCrossWalkInformation :: " + ex.Message);
            }
            return resultant;
        }
    }
}
