using AHA.IS.Common.Authorization.Domain;
using AHA.IS.Common.Authorization.DTO.New;
using AHA.IS.Common.Authorization.DTO.New.Enums;
using CoreLib.Application.Common.Constants;
using CoreLib.Application.Common.Enums;
using CoreLib.Application.Common.Interfaces;
using CoreLib.Application.Common.Models;
using CoreLib.Application.Common.SqlEntities;
using CoreLib.Application.Common.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Globalization;
using UserAuthServiceNew1;
using static CoreLib.Application.Common.SqlEntities.Tenant.Client;

namespace CoreLib.Application.Common.UserAuthService
{
    public class ProvisionHandlerService(IFeatureHandlerService _featureHandlerService, ISqlRepository _sqlRepository,
        IConfiguration _configuration, ILogger<ProvisionHandlerService> _logger, IClientInformationService _clientInformationService) : IProvisionHandlerService
    {
        private readonly string userAuthorizationAddress = _configuration.GetValue<string>("USER_AUTHORIZATION_ENDPOINT");

        public string GetPlatformIndicatorForClient(string clientInstanceNkey)
        {
            ApiResponse<List<AccountDto>?> getICPInforForClient = _clientInformationService.SearchAccountAndSubaccount(
                                                                        new SearchAccountAndSubaccountRequestDto { ClientId = clientInstanceNkey }).Result;
            return GetClientPlatform(getICPInforForClient?.data) ?? string.Empty;
        }

        public TreeGridDTO<int> GetSelectableClients(ICollection<SecurityAssignableItemDTO> clients, string brandName)
        {
            int headerIndex = 0;
            headerIndex = 0;

            var rows = new List<TreeGridRowDTO<int>>();
            foreach (var client in clients)
            {
                //ApiResponse<List<AccountDto>?> getICPInforForClient = _clientInformationService.SearchAccountAndSubaccount(
                //                                                            new SearchAccountAndSubaccountRequestDto { ClientId = client.InstanceNKey }).Result;
                TreeGridRowDTO<int> row = new()
                {
                    InstanceNkey = client.InstanceNKey,
                    LevelIndex = 0,
                    Text = client.InstanceName,
                    //PlatformIndicator = GetClientPlatform(getICPInforForClient?.data),
                    Id = client.ClassifiedSegmentInstanceId,
                    ParentClassifiedSegmentName = brandName,
                    ClientStatus = _clientInformationService.GetClient(_clientInformationService.PadClientId(client.InstanceNKey)).Result?.Status,
                    Expanded = false,
                    Visible = true,

                    Children = [],
                    Properties = [
                        client.SelectedForUser
                    ]
                };

                rows.Add(row);
            }
            return new TreeGridDTO<int>()
            {
                LevelHeaders = [
                    new TreeGridHeaderDTO
                    {
                        Code = headerIndex.ToString(CultureInfo.InvariantCulture),
                        Name = "Clients"
                    }
                ],
                ValueHeaders = [
                    new TreeGridHeaderDTO
                    {
                        Code = (headerIndex + 1).ToString(CultureInfo.InvariantCulture),
                        Name = "Select"
                    }
                ],
                Rows = rows.OrderBy(x => x.Text).ToList()
            };
        }

        public static string GetClientPlatform(List<AccountDto>? icpClientInformation)
        {
            string clientPlatformIndicator = string.Empty;
            List<string> PIs = icpClientInformation.Select(x => x.AccountPlatformName).ToList();
            List<string> subaccountPIs = [];
            foreach (var accountInformation in icpClientInformation)
            {
                subaccountPIs.AddRange(accountInformation.Subaccounts.Select(x => x.PlatformName));
            }
            PIs.AddRange(subaccountPIs);
            var platformIndicators = PIs
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (platformIndicators.Count > 1)
            {
                clientPlatformIndicator = "Mixed";
            }
            else if (platformIndicators.Count == 1)
            {
                clientPlatformIndicator = platformIndicators[0];
            }
            else
            {
                //SC: nothing to handle
            }
            return clientPlatformIndicator;
        }

        public string GetAccountPlatform(string accountId, List<AccountDto>? icpClientInformation)
        {
            string accountPlatformIndicator = string.Empty;
            List<string> PIs = icpClientInformation.Where(x => x.AccountId == accountId).Select(x => x.AccountPlatformName).ToList();
            List<string> subaccountPIs = icpClientInformation.FirstOrDefault(x => x.AccountId == accountId)?.Subaccounts.Select(x => x.PlatformName).ToList();
            if (subaccountPIs != null)
            {
                PIs.AddRange(subaccountPIs);
            }
            var platformIndicators = PIs.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (platformIndicators.Count > 1)
            {
                accountPlatformIndicator = "Mixed";
            }
            else if (platformIndicators.Count == 1)
            {
                accountPlatformIndicator = platformIndicators.First();
            }
            else
            {
                //SC: nothing to handle
            }
            return accountPlatformIndicator;
        }

        public string GetSubaccountPlatform(string subaccountId, string accountId, List<AccountDto>? icpClientInformation)
        {
            string accountPlatformIndicator = string.Empty;
            string subaccountPI = icpClientInformation.FirstOrDefault(x => x.AccountId == accountId)?.Subaccounts
                                        .FirstOrDefault(x => x.SubaccountId == subaccountId)?.PlatformName ?? string.Empty;
            return subaccountPI;
        }

        public async Task<UserFeatureAccessPermissionDTO> ReviseUserAccessDetails(UserFeatureAccessPermissionDTO dto, string brandName, int reviseClientId, bool? newUser)
        {
            dto.SelectedClientIds ??= [];

            var userInfo = newUser != null && newUser == true ? GetNewUserApplicationDetails(dto.UserId, dto.SystemId, brandName, dto.SelectedClientIds)
                                     : dto.SelectedClientIds.Count > 0 ? GetUserApplicationDetailsWithSelectedClients(dto.UserId, dto.SystemId, brandName, dto.SelectedClientIds)
                                         : GetUserApplicationDetails(dto.UserId, dto.SystemId, brandName);
            var originalAssignableItems = GetAllItems(userInfo.AssignableItemsHeader.AssignableItems);
            var GetAllFeatures = _sqlRepository.GetAllFeaturesToList(Convert.ToString(ApplicationConstant.SYSTEM_ID, CultureInfo.InvariantCulture));

            foreach (var feature in dto.Features)
            {
                if (feature.DisplayName == Enums.SystemPermissionGroupSet.ClaimsWorkbaskets.GetTypeTableDisplayName())
                {
                    foreach (var item in originalAssignableItems)
                    {
                        item.SelectedForUser = (item.ClassifiedSegmentCode == Enums.ClassifiedSegmentEnum.UserGroup.GetTypeTableCode() &&
                            dto.SelectedClientIds.Contains(item.ClassifiedSegmentInstanceId));
                    }
                }
                else if (feature.DisplayName == Enums.SystemPermissionGroupSet.EDI.GetTypeTableDisplayName())
                {
                    foreach (var item in originalAssignableItems)
                    {
                        item.SelectedForUser =
                            (item.ClassifiedSegmentCode == Enums.ClassifiedSegmentEnum.TradingPartner.GetTypeTableCode() && dto.SelectedClientIds.Contains(item.ClassifiedSegmentInstanceId));
                    }
                }
                else
                {
                    foreach (var item in originalAssignableItems)
                    {
                        item.SelectedForUser = dto.AllClients || (item.ClassifiedSegmentCode == Enums.ClassifiedSegmentEnum.Client.GetTypeTableCode()
                                                                        && dto.SelectedClientIds.Contains(item.ClassifiedSegmentInstanceId));
                    }
                }
                var featureInfo = GetFeature(feature.FeatureId.Value, GetAllFeatures);

                UpdateFeatureCustomRow(dto, feature, true);

                var newTree = GetFeatureAccessModel(featureInfo, userInfo, dto, ApplicationConstant.BROKER_CLIENT_ID_DEFAULT, brandName);
                if (feature.CustomTreeViewOptions.Rows != null)
                {
                    CopySelected(feature.CustomTreeViewOptions.Rows, newTree.Rows);
                }

                feature.CustomTreeViewOptions = newTree;
            }
            dto = dto.GetVersionizedFeatures(_configuration);

            dto = GetPlatformIndicatorUpdated(dto);

            return dto;
        }
        public async Task<UserFeatureAccessPermissionDTO> ReviseUserAccessDetails(UserFeatureAccessPermissionDTO dto, string brandName, IList<int> reviseClientIds, bool? newUser)
        {
            dto.SelectedClientIds ??= [];

            var userInfo = newUser != null && newUser == true ? GetNewUserApplicationDetails(dto.UserId, dto.SystemId, brandName, dto.SelectedClientIds)
                                     : dto.SelectedClientIds.Count > 0 ? GetUserApplicationDetailsWithSelectedClients(dto.UserId, dto.SystemId, brandName, dto.SelectedClientIds)
                                         : GetUserApplicationDetails(dto.UserId, dto.SystemId, brandName);
            var originalAssignableItems = GetAllItems(userInfo.AssignableItemsHeader.AssignableItems);
            var GetAllFeatures = _sqlRepository.GetAllFeaturesToList(Convert.ToString(ApplicationConstant.SYSTEM_ID, CultureInfo.InvariantCulture));


            foreach (var feature in dto.Features)
            {
                // IF custom, update the data to be used from the selected dto information or change to all if all clients:
                //if (feature.AllowsCustomOption)
                //{

                if (feature.DisplayName == Common.Enums.SystemPermissionGroupSet.ClaimsWorkbaskets.GetTypeTableDisplayName())
                {
                    foreach (var item in originalAssignableItems)
                    {
                        item.SelectedForUser = (item.ClassifiedSegmentCode == Common.Enums.ClassifiedSegmentEnum.UserGroup.GetTypeTableCode() &&
                            dto.SelectedClientIds.Contains(item.ClassifiedSegmentInstanceId));
                    }
                }
                else if (feature.DisplayName == Common.Enums.SystemPermissionGroupSet.EDI.GetTypeTableDisplayName())
                {
                    foreach (var item in originalAssignableItems)
                    {
                        item.SelectedForUser =
                            (item.ClassifiedSegmentCode == Common.Enums.ClassifiedSegmentEnum.TradingPartner.GetTypeTableCode() && dto.SelectedClientIds.Contains(item.ClassifiedSegmentInstanceId));
                    }
                }
                else
                {
                    foreach (var item in originalAssignableItems)
                    {
                        item.SelectedForUser = dto.AllClients || (
                        item.ClassifiedSegmentCode == Common.Enums.ClassifiedSegmentEnum.Client.GetTypeTableCode() && dto.SelectedClientIds.Contains(item.ClassifiedSegmentInstanceId));
                    }
                }
                //}
                //else
                //{
                //    // If not custom, unselect everything
                //    foreach (var item in originalAssignableItems)
                //    {
                //        item.SelectedForUser = false;
                //    }
                //    feature.CustomTreeViewOptions = new TreeGridDTO<int>();
                //}
                var featureInfo = GetFeature(feature.FeatureId.Value, GetAllFeatures);

                UpdateFeatureCustomRow(dto, feature, true);

                var newTree = GetFeatureAccessModel(featureInfo, userInfo, dto, ApplicationConstant.BROKER_CLIENT_ID_DEFAULT, brandName);
                if (feature.CustomTreeViewOptions.Rows != null)
                {
                    CopySelected(feature.CustomTreeViewOptions.Rows, newTree.Rows);
                }

                feature.CustomTreeViewOptions = newTree;
            }
            dto = dto.GetVersionizedFeatures(_configuration);
            foreach (int reviseClientId in reviseClientIds)
            {
                dto = GetPlatformIndicatorUpdated(dto, dto.SelectedClientsTreeData.Rows.First(x => x.Id == reviseClientId).InstanceNkey);
            }
            return dto;
        }

        public UserFeatureAccessPermissionDTO GetPlatformIndicatorUpdated(UserFeatureAccessPermissionDTO dto)
        {
            Dictionary<string, List<AccountDto?>> ClientICPInfoList = new();
            foreach (var clientNkey in dto.SelectedClientsTreeData?.Rows.Select(x => x.InstanceNkey))
            {
                if (!ClientICPInfoList.ContainsKey(clientNkey))
                {
                    ClientICPInfoList.Add(clientNkey, _clientInformationService.SearchAccountAndSubaccount(new SearchAccountAndSubaccountRequestDto { ClientId = clientNkey }).Result?.data);
                }
            }

            foreach (var feature in dto.Features)
            {
                foreach (var row in feature.CustomTreeViewOptions.Rows)
                {
                    List<AccountDto?> clientICPInfo = ClientICPInfoList.FirstOrDefault(x => x.Key == row.InstanceNkey).Value;
                    row.PlatformIndicator = GetClientPlatform(clientICPInfo);
                    foreach (var account in row.Children)
                    {
                        account.PlatformIndicator = GetAccountPlatform(account.InstanceNkey, clientICPInfo);
                        foreach (var subaccount in account.Children)
                        {
                            subaccount.PlatformIndicator = GetSubaccountPlatform(subaccount.InstanceNkey, account.InstanceNkey, clientICPInfo);
                        }
                    }
                }
            }

            foreach (var row in dto.SelectedClientsTreeData?.Rows)
            {
                List<AccountDto?> clientICPInfo = ClientICPInfoList.FirstOrDefault(x => x.Key == row.InstanceNkey).Value;
                row.PlatformIndicator = GetClientPlatform(clientICPInfo);
                foreach (var account in row.Children)
                {
                    account.PlatformIndicator = GetAccountPlatform(account.InstanceNkey, clientICPInfo);
                    foreach (var subaccount in account.Children)
                    {
                        subaccount.PlatformIndicator = GetSubaccountPlatform(subaccount.InstanceNkey, account.InstanceNkey, clientICPInfo);
                    }
                }
            }
            if (dto.ICPClientInfo == null)
            {
                dto.ICPClientInfo = new Dictionary<string, List<AccountDto?>>();
            }
            dto.ICPClientInfo = ClientICPInfoList;
            return dto;
        }
        public UserFeatureAccessPermissionDTO GetPlatformIndicatorUpdated(UserFeatureAccessPermissionDTO dto, string? reviseClientId)
        {

            var ICPClientInfo = _clientInformationService.SearchAccountAndSubaccount(new SearchAccountAndSubaccountRequestDto { ClientId = reviseClientId }).Result?.data;

            foreach (var feature in dto.Features)
            {
                var clientFeature = feature.CustomTreeViewOptions.Rows.FirstOrDefault(x => x.InstanceNkey == reviseClientId);
                {
                    if (clientFeature != null)
                    {
                        List<AccountDto?> clientICPInfo = ICPClientInfo;
                        clientFeature.PlatformIndicator = GetClientPlatform(clientICPInfo);
                        foreach (var account in clientFeature.Children)
                        {
                            account.PlatformIndicator = GetAccountPlatform(account.InstanceNkey, clientICPInfo);
                            foreach (var subaccount in account.Children)
                            {
                                subaccount.PlatformIndicator = GetSubaccountPlatform(subaccount.InstanceNkey, account.InstanceNkey, clientICPInfo);
                            }
                        }
                    }
                }
            }
            var selectedClient = dto.SelectedClientsTreeData?.Rows.FirstOrDefault(x => x.InstanceNkey == reviseClientId);
            {
                List<AccountDto?> clientICPInfo = ICPClientInfo;
                selectedClient.PlatformIndicator = GetClientPlatform(clientICPInfo);
                foreach (var account in selectedClient.Children)
                {
                    account.PlatformIndicator = GetAccountPlatform(account.InstanceNkey, clientICPInfo);
                    foreach (var subaccount in account.Children)
                    {
                        subaccount.PlatformIndicator = GetSubaccountPlatform(subaccount.InstanceNkey, account.InstanceNkey, clientICPInfo);
                    }
                }
            }
            dto.ICPClientInfo ??= [];
            dto.ICPClientInfo.Add(reviseClientId, ICPClientInfo);
            return dto;
        }

        public void UpdateFeatureCustomRow(UserFeatureAccessPermissionDTO dto, UserManagementEditFeatureDTO feature, bool isRevise = false)
        {
            if ((dto.AllClients || dto.SelectedClientIds == null || dto.SelectedClientIds.Count == 0 || !feature.AllowsCustomOption)
                    && feature.DisplayName != Enums.SystemPermissionGroupSet.EDI.GetTypeTableDisplayName())
            {
                feature.Selections.ToList().RemoveAll(x => x.Id == -1);
                if (feature.SelectedOptionIds != null && feature.SelectedOptionIds.Contains(-1))
                {
                    feature.SelectedOptionIds = [];
                }
            }
            else
            {
                if (feature.DisplayName == Enums.SystemPermissionGroupSet.UserAdmin.GetTypeTableDisplayName())
                {
                    feature.SelectedOptionId = null;
                    feature.SelectedOptionIds = null;
                }
                if (feature.DisplayName.Contains("INDEX", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (isRevise && feature.DisplayName == Enums.SystemPermissionGroupSet.INDEXReports.GetTypeTableDisplayName())
                    {
                        feature.SelectedOptionId = -1;
                    }
                    foreach (var selection in feature.Selections)
                    {
                        selection.ShowsCustomButton = true;
                        selection.RestrictCustomId = selection.Id;
                    }
                }
                else if (feature.Selections.All(x => x.Id != -1))
                {
                    feature.Selections.Add
                    (
                        new UserManagementEditFeatureSelectionDTO
                        {
                            Id = -1,
                            Value = "Custom",
                            ShowsCustomButton = true
                        }
                    );
                }
                else
                {
                    // SQ:  default?
                }
            }
        }

        private static void CopySelected(IList<TreeGridRowDTO<int>> from, IList<TreeGridRowDTO<int>> to)
        {
            ListMatcher<TreeGridRowDTO<int>, TreeGridRowDTO<int>> matcher = new(from, to, (x, y) => (x.Id == y.Id));

            foreach (var common in matcher.InBoth)
            {

                if (common.Obj1.Properties == null)
                {
                    continue;
                }

                if (common.Obj2.Properties == null)
                {
                    common.Obj2.Properties = [];
                }
                if (common.Obj1 != null && common.Obj2 != null)
                {
                    common.Obj2.PlatformIndicator = common.Obj1.PlatformIndicator;
                    common.Obj2.ParentClassifiedSegmentName = common.Obj1.ParentClassifiedSegmentName;
                    common.Obj2.IsAllSelected = common.Obj1.IsAllSelected;
                    common.Obj2.ClientStatus = common.Obj1.ClientStatus;
                    common.Obj2.Visible = common.Obj1.Visible;
                }

                while (common.Obj2.Properties.Count < common.Obj1.Properties.Count)
                {
                    common.Obj2.Properties.Add(false);
                }

                for (int i = 0; i < common.Obj1.Properties.Count; i++)
                {
                    common.Obj2.Properties[i] = common.Obj1.Properties[i];
                }

                if (common.Obj1.Children.Any())
                {
                    CopySelected(common.Obj1.Children, common.Obj2.Children);
                }
            }
        }

        public IList<SecurityAssignableItemDTO> GetAllItems(IList<SecurityAssignableItemDTO> items)
        {
            var curItems = items.ToList();
            if (items != null)
            {
                foreach (var item in items)
                {
                    if (item.Children != null && item.Children.Count != 0)
                    {
                        curItems.AddRange(GetAllItems(item.Children));
                    }
                }
            }
            return curItems;
        }

        public UserApplicationDetailsDTO GetUserApplicationDetailsSQLWrapper(int userId, int systemId, string brandName, IList<int> clientIds = null,
                                                                                IList<string> clientsNKey = null)
        {
            UserAccessInfoModel? userInfoDetails = _sqlRepository.GetUserInfoDetails(systemId.ToString(CultureInfo.InvariantCulture),
                                                                    userId.ToString(CultureInfo.InvariantCulture), brandName, clientIds, clientsNKey);
            return ProcessFeaturesForUser(userInfoDetails, userId, systemId, brandName);
        }

        public UserApplicationDetailsDTO GetUserApplicationDetails(int userId, int systemId, string brandName)
        {
            try
            {
                DateTime dt = DateTime.UtcNow;
                var result = GetUserApplicationDetailsSQLWrapper(userId, systemId, brandName);
                _logger.MethodElapsedMs("GetUserApplicationDetailsSQLWrapper", (DateTime.UtcNow - dt).TotalMilliseconds);

                return result;
            }
            catch (Exception)
            {
                return new UserApplicationDetailsDTO();
            }
        }

        public UserApplicationDetailsDTO GetUserApplicationDetailsWithSelectedClients(int userId, int systemId, string brandName, IList<int> clientIds)
        {
            try
            {
                DateTime dt = DateTime.UtcNow;
                var result = GetUserApplicationDetailsSQLWrapper(userId, systemId, brandName, clientIds);
                _logger.MethodElapsedMs("GetUserApplicationDetailsSQLWrapper", (DateTime.UtcNow - dt).TotalMilliseconds);

                return result;
            }
            catch (Exception)
            {
                return new UserApplicationDetailsDTO();
            }
        }

        public UserApplicationDetailsDTO GetUserApplicationDetailsWithSelectedClientsNKey(int userId, int systemId, string brandName, IList<string> clientNKeys)
        {
            try
            {
                DateTime dt = DateTime.UtcNow;
                var result = GetUserApplicationDetailsSQLWrapper(userId, systemId, brandName, null, clientNKeys);
                _logger.MethodElapsedMs("GetUserApplicationDetailsSQLWrapper", (DateTime.UtcNow - dt).TotalMilliseconds);

                return result;
            }
            catch (Exception)
            {
                return new UserApplicationDetailsDTO();
            }
        }

        public TreeGridDTO<int> GetFeatureAccessModel(Models.FeatureDTO featureDTO, UserApplicationDetailsDTO authInfo, UserFeatureAccessPermissionDTO dto,
                                                        int BrokerClientId, string brandName)
        {
            DateTime dt = DateTime.UtcNow;
            Guard.NotNull(authInfo, nameof(authInfo));

            List<SecurityAssignableItemDTO> adjustedAssignableItems = authInfo.AssignableItemsHeader.AssignableItems;

            if ((!featureDTO.AllowsCustomAccounts || dto.AllClients) && featureDTO.Name != Enums.SystemPermissionGroupSet.EDI.GetTypeTableDisplayName())
            {
                return new TreeGridDTO<int>
                {
                    LevelHeaders = [],
                    ValueHeaders = [],
                    Rows = []
                };
            }

            // Remove tenant level from assignable items, making Client the highest level:
            if (featureDTO.Name == Enums.SystemPermissionGroupSet.ClaimsWorkbaskets.GetTypeTableDisplayName())
            {
                adjustedAssignableItems = GetAssignableItemsByType(adjustedAssignableItems, Enums.ClassifiedSegmentEnum.UserGroup.GetTypeTableCode());
                adjustedAssignableItems = adjustedAssignableItems.Where(y => _featureHandlerService.GetConfiguredWorkbaskets().Contains(y.InstanceName))
                                                                 .OrderBy(x => x.InstanceName).ToList();
            }
            else if (featureDTO.Name == Enums.SystemPermissionGroupSet.EDI.GetTypeTableDisplayName())
            {
                adjustedAssignableItems = GetAssignableItemsByType(adjustedAssignableItems
                    .Where(x => x.InstanceName != AppSystemEnum.MemberEnrollment.GetTypeTableDisplayName()).ToList(), Enums.ClassifiedSegmentEnum.TradingPartner.GetTypeTableCode())
                    .OrderBy(x => x.InstanceName).ToList();
            }
            else
            {
                var tempAdjustedAssignableItems = adjustedAssignableItems;
                adjustedAssignableItems = GetAssignableItemsByType(tempAdjustedAssignableItems, Enums.ClassifiedSegmentEnum.Client.GetTypeTableCode());
                if (!dto.AllClients)
                {
                    var clientIds = adjustedAssignableItems.Where(x => x.SelectedForUser).Select(y => y.ClassifiedSegmentInstanceId).ToList();
                    adjustedAssignableItems = clientIds.Count == 0 && BrokerClientId > 0
                        ? adjustedAssignableItems.Where(x => BrokerClientId.ToString(CultureInfo.InvariantCulture) == x.InstanceNKey).ToList()
                        : adjustedAssignableItems.Where(x => clientIds.Contains(x.ClassifiedSegmentInstanceId)).ToList();
                }
            }

            featureDTO.AllowedInstanceClassifiedSegmentCodes = featureDTO.AllowedInstanceClassifiedSegmentCodes
                .Where(x => TypeTableCodeHelper.GetEnumFromCode<Enums.ClassifiedSegmentEnum>(x) != Enums.ClassifiedSegmentEnum.Tenant).ToList();

            List<TreeGridHeaderDTO> levelHeaders = [];
            int headerIndex = 0;
            foreach (var code in featureDTO.AllowedInstanceClassifiedSegmentCodes)
            {
                if (TypeTableCodeHelper.GetEnumFromCode<Enums.ClassifiedSegmentEnum>(code) == Enums.ClassifiedSegmentEnum.TradingPartner)
                {
                    levelHeaders.Add(new TreeGridHeaderDTO
                    {
                        Code = headerIndex++.ToString(CultureInfo.InvariantCulture),
                        Name = "Trading Partner ID"
                    });
                }
                levelHeaders.Add(new TreeGridHeaderDTO
                {
                    Code = headerIndex++.ToString(CultureInfo.InvariantCulture),
                    Name = TypeTableCodeHelper.GetEnumFromCode<Enums.ClassifiedSegmentEnum>(code) == Enums.ClassifiedSegmentEnum.UserGroup
                            ? "Workbasket Name"
                            : TypeTableCodeHelper.GetEnumFromCode<Enums.ClassifiedSegmentEnum>(code).GetTypeTableDisplayName(),
                });
            }

            List<int> valueHeaderIds = [];
            List<TreeGridHeaderDTO> valueHeaders = [];
            headerIndex = 0;
            foreach (var featureSelection in featureDTO.FeatureSelections)
            {
                valueHeaders.Add(
                    new TreeGridHeaderDTO
                    {
                        DataKey = featureSelection.SystemPermissionGroupSetGroupingId.ToString(CultureInfo.InvariantCulture),
                        Code = headerIndex++.ToString(CultureInfo.InvariantCulture),
                        Name = featureSelection.CustomLabelName,
                        PermissionCode = featureSelection.PermissionCode,
                    }
                );
                valueHeaderIds.Add(featureSelection.SystemPermissionGroupSetGroupingId);
            }

            var selection = authInfo.UserFeatureSelections.Single(x => x.SystemPermissionGroupSetId == featureDTO.SystemPermissionGroupSetId);
            var rows = _featureHandlerService.GetTreeGridRows(adjustedAssignableItems, selection, valueHeaderIds, featureDTO.AllowedInstanceClassifiedSegmentCodes, brandName);

            int clientIndex = 0;
            foreach (var client in rows)
            {
                int rowIndex = 0;
                foreach (var account in client.Children)
                {
                    if (account.Children.Count > 0)
                    {
                        rows[clientIndex].Children[rowIndex].Visible = account.Children.Any(x => x.Properties.Contains(true)
                                                                                            || x.Children.Any(y => y.Properties.Contains(true)));
                        int colIndex = 0;
                        foreach (var sa in account.Children)
                        {
                            rows[clientIndex].Children[rowIndex].Children[colIndex].Visible = rows[clientIndex].Children[rowIndex].Children[colIndex]
                                                                                                    .Properties.Contains(true);
                            colIndex++;
                        }
                    }
                    else
                    {
                        rows[clientIndex].Children[rowIndex].Visible = account.Properties.Contains(true);
                    }
                    rowIndex++;
                }
                clientIndex++;
            }

            return new TreeGridDTO<int>
            {
                LevelHeaders = levelHeaders,
                ValueHeaders = valueHeaders,
                Rows = rows
            };
        }
        public Models.FeatureDTO GetFeature(int systemPermissionGroupSetId, List<FeatureEntity> AllFeaturesList)
        {
            var selectedFeature = AllFeaturesList.FirstOrDefault(x => x.SystemPermissionGroupSetId == systemPermissionGroupSetId.ToString(CultureInfo.InvariantCulture));
            if (selectedFeature != null)
            {
                List<Models.FeatureSelectionDTO> featureSelection = [];
                var selectionList = selectedFeature.FeatureSelection.Split("***").ToList();
                foreach (string selection in selectionList)
                {
                    featureSelection.Add(new Models.FeatureSelectionDTO
                    {
                        DisplayOrderNum = Int32.TryParse(selection.Split(',')[0], CultureInfo.InvariantCulture, out int displayOrder) ? displayOrder : null,
                        LabelName = selection.Split(',')[1],
                        CustomLabelName = selection.Split(',')[2],
                        SystemPermissionGroupSetGroupingId = Int32.Parse(selection.Split(',')[3], CultureInfo.InvariantCulture),
                        PermissionCode = selection.Split(',')[4]
                    });
                }

                Models.FeatureDTO mappedFeature = new()
                {
                    SystemPermissionGroupSetId = Int32.Parse(selectedFeature.SystemPermissionGroupSetId, CultureInfo.InvariantCulture),
                    DisplayOrder = Int32.TryParse(selectedFeature.DisplayOrderNum, CultureInfo.InvariantCulture, out int ordernumber) ? ordernumber : null,
                    AllowedInstanceClassifiedSegmentCodes = selectedFeature.SegmentCode.Split(",")
                                                                .Distinct().OrderBy(
                                                                   x =>
                                                                      x == Enums.ClassifiedSegmentEnum.Tenant.GetTypeTableCode() ? 0 :
                                                                      x == Enums.ClassifiedSegmentEnum.Client.GetTypeTableCode() ? 1 :
                                                                      x == Enums.ClassifiedSegmentEnum.Account.GetTypeTableCode() ? 2 :
                                                                      x == Enums.ClassifiedSegmentEnum.SubAccount.GetTypeTableCode() ? 3 :
                                                                      x == Enums.ClassifiedSegmentEnum.BillingGroup.GetTypeTableCode() ? 4 : 5
                                                               ).ToList(),
                    Name = selectedFeature.DisplayName,
                    AllowsCustomAccounts = selectedFeature.CustomLabelName != null,
                    FeatureSelections = featureSelection
                };

                return mappedFeature;
            }
            else
            {
                return null;
            }
        }

        public UserApplicationDetailsDTO GetNewUserApplicationDetails(int userId, int systemId, string brandName, IList<int> clientIds = null)
        {
            try
            {
                DateTime dt = DateTime.UtcNow;
                var result = GetNewUserApplicationDetailsSQLWrapper(userId, systemId, brandName, clientIds);
                _logger.MethodElapsedMs("GetNewUserApplicationDetailsSQLWrapper", (DateTime.UtcNow - dt).TotalMilliseconds);

                return result;
            }
            catch (Exception)
            {
                return new UserApplicationDetailsDTO();
            }
        }

        public UserApplicationDetailsDTO GetNewUserApplicationDetailsSQLWrapper(int userId, int systemId, string brandName, IList<int> clientIds = null)
        {
            var userInfoDetails = _sqlRepository.GetNewUserInfoDetails(systemId.ToString(CultureInfo.InvariantCulture), userId.ToString(CultureInfo.InvariantCulture),
                                                                    brandName, clientIds);
            return ProcessFeaturesForUser(userInfoDetails, userId, systemId, brandName);
        }

        public UserApplicationDetailsDTO ProcessFeaturesForUser(UserAccessInfoModel? userInfoDetails, int userId, int systemId, string brandName)
        {
            UserApplicationDetailsDTO userDto = new();
            List<AHA.IS.Common.Authorization.DTO.New.UserFeatureSelectionDTO> userFeatures = [];
            List<UserInfoEntity> userInfo = userInfoDetails.userInfo;
            List<Tenant> tenants = userInfoDetails.tenants;
            List<FeatureEntity> allFeatures = userInfoDetails.features;
            List<FeatureAssingedInstanceEntity> featuresAssigned = userInfoDetails.featureAssingedInstances;
            foreach (var systemFeature in allFeatures)
            {
                AHA.IS.Common.Authorization.DTO.New.UserFeatureSelectionDTO selection = new()
                {
                    SystemPermissionGroupSetId = Int32.Parse(systemFeature.SystemPermissionGroupSetId, CultureInfo.InvariantCulture),
                };

                if (!featuresAssigned.Exists(x => x.SPGSG_SystemPermissionGroupSetId == systemFeature.SystemPermissionGroupSetId))
                {
                    selection.SimpleSystemPermissionGroupSetGroupingId = null;
                    selection.AreCustomInstancesSelected = false;
                    selection.PermissionGroupId = null;
                }
                else
                {
                    var permissions = featuresAssigned.ListDistinctBy(xx => xx.SPGSG_SystemPermissionGroupSetGroupingId)
                        .Where(x => x.SPGSG_SystemPermissionGroupSetId == systemFeature.SystemPermissionGroupSetId)
                        .Select(
                            x =>
                                new
                                {
                                    UserPermissionRow = GetUserPermissions(x, featuresAssigned),
                                    SystemPermissionGroupSetGrouping = GetSystemPermissionGroupSetGrouping(x, featuresAssigned)

                                }
                        ).ToList();

                    if (permissions.Exists(x => x.SystemPermissionGroupSetGrouping.Exists(y =>
                        y.SystemPermissionGroupSet.DisplayName == SystemPermissionGroupSetEnum.ClientReports.GetTypeTableDisplayName()))
                            && permissions.TrueForAll(x => !(x.UserPermissionRow.CustomAccessFlag ?? false)))
                    {
                        var allAccessPerm = permissions.ToList();
                        List<int> SimpleSystemPermissionGroupSetGroupingIds = [];
                        foreach (var SetGroups in allAccessPerm.Select(x => x.SystemPermissionGroupSetGrouping))
                        {
                            SimpleSystemPermissionGroupSetGroupingIds.AddRange(SetGroups.Select(x => x.SystemPermissionGroupSetGroupingId).ToList());
                        }

                        selection.SimpleSystemPermissionGroupSetGroupingIds = SimpleSystemPermissionGroupSetGroupingIds;
                        selection.AreCustomInstancesSelected = false;
                        selection.PermissionGroupId = allAccessPerm.Select(x => x.UserPermissionRow.PermissionGroupId).First();
                    }
                    // All Access to means only one row in UserPermission for the GroupSet and CustomAccessFlag = 0
                    else if (permissions.Count == 1 && permissions.TrueForAll(x => !(x.UserPermissionRow.CustomAccessFlag ?? false)))
                    {
                        var allAccessPerm = permissions.Single();
                        selection.SimpleSystemPermissionGroupSetGroupingId = allAccessPerm.SystemPermissionGroupSetGrouping.FirstOrDefault()
                                                                                    .SystemPermissionGroupSetGroupingId;
                        selection.AreCustomInstancesSelected = false;
                        selection.PermissionGroupId = allAccessPerm.UserPermissionRow.PermissionGroupId;
                    }
                    else
                    {
                        // Custom Access means at least one row in UserPermission and all have CustomAccessFlag = 1
                        if (permissions.Count > 0 && permissions.TrueForAll(x => (x.UserPermissionRow.CustomAccessFlag ?? false)))
                        {
                            selection.SimpleSystemPermissionGroupSetGroupingId = null;
                            selection.AreCustomInstancesSelected = true;
                            selection.PermissionGroupId = null;

                            selection.CustomSelectedInstances = permissions.Select(
                                x =>
                                    new UserFeatureSelectionCustomDTO()
                                    {
                                        SystemPermissionGroupSetGroupingId = x.SystemPermissionGroupSetGrouping.FirstOrDefault().SystemPermissionGroupSetGroupingId,
                                        SelectedInstanceIds = x.UserPermissionRow.UserPermissionInstanceDetails.Select(y => y.ClassifiedSegmentInstanceId).ToList()
                                    }
                            ).ToList();
                        }
                        else
                        {
                            throw new Exception("Invalid Permission State");
                        }
                    }
                }
                userFeatures.Add(selection);
            }

            if (userInfo.Count > 0)
            {
                SecurityAssignableItemHeaderDTO assignableItems = GetAssignableItemHeaders(userId, systemId, tenants,
                                                    userInfo.Select(csii => Int32.Parse(csii.AssignedClassifiedSegmentInstanceId, CultureInfo.InvariantCulture)).ToList());
                userDto = new UserApplicationDetailsDTO
                {
                    SystemId = Int32.Parse(userInfo[0].SystemId, CultureInfo.InvariantCulture),
                    LoginSystemUserId = Int32.Parse(userInfo[0].LogInSystemUserId, CultureInfo.InvariantCulture),
                    SourceLogInSystemGroupNames = userInfo.Select(x => x.GroupName).ToList(),
                    AccessEffectiveDate = !string.IsNullOrEmpty(userInfo[0].EffDate) ? DateTime.ParseExact(userInfo[0].EffDate,
                                                "M/d/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture) : new DateTime(1900, 1, 1),
                    AccessTerminationDate = !string.IsNullOrEmpty(userInfo[0].TermDate) ? DateTime.ParseExact(userInfo[0].EffDate, "M/d/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture) : null,
                    EmailAddress = userInfo[0].EmailAddress,
                    FirstName = userInfo[0].FirstName,
                    LastName = userInfo[0].LastName,
                    LastLogin = null,
                    Active = userInfo[0].ActiveFlag.Equals("true", StringComparison.InvariantCultureIgnoreCase),
                    UserName = userInfo[0].SourceLogInSystemUserName,
                    AssignableItemsHeader = assignableItems,
                    RestrictedInstanceName = string.Join(", ", assignableItems.AssignableItems
                                                                            .Where(x => x.ClassifiedSegmentCode.Equals(Enums.ClassifiedSegmentEnum.Tenant.ToString(), StringComparison.InvariantCultureIgnoreCase))
                                                                            .Select(x => x.InstanceName).ToArray()),
                    SystemCode = userInfo[0].SystemCode,
                    LogInSystemCode = userInfo[0].LoginSystemCode,
                    SourceLogInSystemGroupSetName = userInfo[0].SourceLogInSystemGroupName,
                    UserFeatureSelections = userFeatures
                };
            }
            return userDto;
        }
        public List<SecurityAssignableItemDTO> GetAssignableItemsByType(List<SecurityAssignableItemDTO> items, string segmentCode)
        {
            var newList = items.Where(x => x.ClassifiedSegmentCode == segmentCode).ToList();

            foreach (var item in items.ToList())
            {
                if (item.Children.Count != 0)
                {
                    var childItems = GetAssignableItemsByType(item.Children, segmentCode);
                    newList.AddRange(childItems);
                }
            }
            return newList;
        }
        public SecurityAssignableItemHeaderDTO GetAssignableItemHeaders(int userId, int systemId, List<Tenant> tenants, List<int> AssignedClassifiedSegmentInstanceId)
        {
            var SecurityAssignableObj = new SecurityAssignableItemHeaderDTO
            {
                LoginSystemUserId = userId,
                SystemId = systemId,
                AssignableItems = tenants.Select(tenant => new SecurityAssignableItemDTO
                {
                    ClassifiedAreaCode = tenant.AreaCode,
                    ClassifiedAreaName = tenant.AreaName,
                    ClassifiedSegmentCode = tenant.SegmentCode,
                    ClassifiedSegmentName = tenant.SegmentName,
                    ClassifiedSegmentInstanceId = tenant.ClassifiedSegmentInstanceId,
                    InstanceName = tenant.ClassifiedAreaSegmentName,
                    InstanceNKey = tenant.ClassifiedAreaSegmentNKey,
                    SelectedForUser = AssignedClassifiedSegmentInstanceId.Contains(tenant.ClassifiedSegmentInstanceId),
                    Children = tenant.Clients.ListDistinctBy(z => z.ClassifiedSegmentInstanceId).Where(client => client.SegmentName != tenant.SegmentName)
                                            .Select(client => new SecurityAssignableItemDTO
                                            {
                                                ClassifiedAreaCode = client.AreaCode,
                                                ClassifiedAreaName = client.AreaName,
                                                ClassifiedSegmentCode = client.SegmentCode,
                                                ClassifiedSegmentName = client.SegmentName,
                                                ClassifiedSegmentInstanceId = client.ClassifiedSegmentInstanceId,
                                                InstanceName = client.ClassifiedAreaSegmentName,
                                                InstanceNKey = client.ClassifiedAreaSegmentNKey,
                                                SelectedForUser = AssignedClassifiedSegmentInstanceId.Contains(client.ClassifiedSegmentInstanceId),
                                                Children = PrepareAccounts(tenant.Clients.Where(clients => clients.ClassifiedSegmentInstanceId == client.ClassifiedSegmentInstanceId).ToList(), AssignedClassifiedSegmentInstanceId)
                                            }).ToList()
                }).ToList()
            };
            return SecurityAssignableObj;
        }
        private static List<SecurityAssignableItemDTO> PrepareAccounts(List<Tenant.Client> clients, List<int> AssignedClassifiedSegmentInstanceId)
        {
            List<SecurityAssignableItemDTO> resultAccounts = [];
            List<Account> clientAccounts = [];
            foreach (Tenant.Client client in clients)
            {
                clientAccounts.AddRange(client.Accounts.Where(account => account.SegmentName != client.SegmentName));
            }

            foreach (var accounts in clientAccounts.GroupBy(z => z.ClassifiedSegmentInstanceId))
            {
                var firstAccount = accounts.FirstOrDefault();
                resultAccounts.Add(new SecurityAssignableItemDTO
                {
                    ClassifiedAreaCode = firstAccount.AreaCode,
                    ClassifiedAreaName = firstAccount.AreaName,
                    ClassifiedSegmentCode = firstAccount.SegmentCode,
                    ClassifiedSegmentName = firstAccount.SegmentName,
                    ClassifiedSegmentInstanceId = firstAccount.ClassifiedSegmentInstanceId,
                    InstanceName = firstAccount.ClassifiedAreaSegmentName,
                    InstanceNKey = firstAccount.ClassifiedAreaSegmentNKey,
                    SelectedForUser = AssignedClassifiedSegmentInstanceId.Contains(firstAccount.ClassifiedSegmentInstanceId),
                    Children = PrepareSubAccounts(accounts.ToList(), AssignedClassifiedSegmentInstanceId)
                });
            }

            return resultAccounts;
        }

        private static List<SecurityAssignableItemDTO> PrepareSubAccounts(List<Account> accounts, List<int> AssignedClassifiedSegmentInstanceId)
        {
            List<SecurityAssignableItemDTO> resultSubAccounts = [];
            foreach (var account in accounts)
            {
                resultSubAccounts.AddRange(account.Children.Where(child => child.SegmentName != account.SegmentName).Select(child => new SecurityAssignableItemDTO
                {
                    ClassifiedAreaCode = child.AreaCode,
                    ClassifiedAreaName = child.AreaName,
                    ClassifiedSegmentCode = child.SegmentCode,
                    ClassifiedSegmentName = child.SegmentName,
                    ClassifiedSegmentInstanceId = child.ClassifiedSegmentInstanceId,
                    InstanceName = child.ClassifiedAreaSegmentName,
                    InstanceNKey = child.ClassifiedAreaSegmentNKey,
                    SelectedForUser = AssignedClassifiedSegmentInstanceId.Contains(child.ClassifiedSegmentInstanceId),
                }).ToList());
            }
            return resultSubAccounts;
        }
        public AppVerions GetAppVersionForPermissionCode(string permissionCode)
        {
            Dictionary<AppVerions, List<string>> featureVerionMapping = new()
            {
                {
                    AppVerions.Ver1,
                    new List<string> { "MBR_VIEW", "IDX_MEM_RPT", "IDX_RPT", "IDX_SL_RPT", "GP_RESOURCE", "GP_USERADMIN",
                                    "GP_CLIENTINFO", "GP_SPAC_READ", "GP_SPAC_SUPER","GP_ACCPROFILE", "PR_FACCESS",
                                    "EBPP_INV_PHI", "EBPP_INV_NPHI", "EBPP_MAKE_PMT", "MET_EDIT", "MET_VIEW"}
                }
            };

            foreach (var key in featureVerionMapping.Keys)
            {
                featureVerionMapping.TryGetValue(key, out List<string> features);
                if (features.Contains(permissionCode))
                {
                    return key;
                }
            }
            return AppVerions.NotRelease;
        }

        public UserPermission GetUserPermissions(FeatureAssingedInstanceEntity x, List<FeatureAssingedInstanceEntity> featuresAssigned)
        {
            UserPermission userPermissionRow = new()
            {
                CustomAccessFlag = x.UP_CustomAccessFlag.Equals("true", StringComparison.InvariantCultureIgnoreCase),
                EffDate = !string.IsNullOrEmpty(x.UP_EffDate)
                            ? DateTime.ParseExact(x.UP_EffDate, "M/d/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)
                            : new DateTime(1900, 1, 1),
                TermDate = !string.IsNullOrEmpty(x.UP_TermDate)
                            ? DateTime.ParseExact(x.UP_TermDate, "M/d/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)
                            : null,
                LogInSystemUser = new LogInSystemUser
                {
                    ActiveFlag = x.LSU_ACTIVEFLAG.Equals("true", StringComparison.InvariantCultureIgnoreCase),
                    LogInSystemGroupSetId = Int32.Parse(x.LSU_LogInSystemGroupSetId, CultureInfo.InvariantCulture),
                },
                LogInSystemUserId = Int32.Parse(x.UP_LogInSystemUserId, CultureInfo.InvariantCulture),
                PermissionGroupId = Int32.Parse(x.UP_PermissionGroupId, CultureInfo.InvariantCulture),
                UserPermissionId = Int32.Parse(x.UP_UserPermissionId, CultureInfo.InvariantCulture),
                UserPermissionInstanceDetails = featuresAssigned.Where(y => y.SPGSG_SystemPermissionGroupSetId == x.SPGSG_SystemPermissionGroupSetId
                                                                        && y.SPGSG_SystemPermissionGroupSetGroupingId == x.SPGSG_SystemPermissionGroupSetGroupingId).Select(z =>
                                                                                new UserPermissionInstanceDetail
                                                                                {
                                                                                    ClassifiedSegmentInstanceId = Int32.Parse(z.UPID_ClassifiedSegmentInstanceId, CultureInfo.InvariantCulture),
                                                                                    ExcludeFlag = z.UPID_ExcludeFlag.Equals("true", StringComparison.InvariantCultureIgnoreCase),
                                                                                    UserPermissionId = Int32.Parse(z.UPID_UserPermissionId, CultureInfo.InvariantCulture),
                                                                                    UserPermissionInstanceDetailId = Int32.Parse(z.UPID_UserPermissionInstanceDetailId, CultureInfo.InvariantCulture)
                                                                                }
                                                                        ).ToList()
            };
            return userPermissionRow;
        }

        public List<SystemPermissionGroupSetGrouping> GetSystemPermissionGroupSetGrouping(FeatureAssingedInstanceEntity x, List<FeatureAssingedInstanceEntity> featuresAssigned)
        {
            var systemPermissionGroupSetGrouping = featuresAssigned.Where(y => y.SPGSG_SystemPermissionGroupSetId == x.SPGSG_SystemPermissionGroupSetId
            && y.SPGSG_SystemPermissionGroupSetGroupingId == x.SPGSG_SystemPermissionGroupSetGroupingId).Select(z =>
                                            new SystemPermissionGroupSetGrouping
                                            {
                                                CustomLabelName = z.SPGSG_CustomLabelName,
                                                DisplayOrderNum = short.TryParse(z.SPGSG_DisplayOrderNum, CultureInfo.InvariantCulture, out short orderNum) ? orderNum : null,
                                                LabelName = z.SPGSG_LabelName,
                                                LowestAssignableClassifiedAreaSegmentId = Int32.TryParse(z.SPGSG_LowestAssignableClassifiedAreaSegmentId, CultureInfo.InvariantCulture, out int lowerCID)
                                                                                            ? lowerCID : null,
                                                PermissionGroupId = Int32.Parse(z.SPGSG_PermissionGroupId, CultureInfo.InvariantCulture),
                                                SystemPermissionGroupSetGroupingId = Int32.Parse(z.SPGSG_SystemPermissionGroupSetGroupingId, CultureInfo.InvariantCulture),
                                                SystemPermissionGroupSetId = Int32.Parse(z.SPGSG_SystemPermissionGroupSetId, CultureInfo.InvariantCulture),
                                                SystemPermissionGroupSet = new AHA.IS.Common.Authorization.Domain.SystemPermissionGroupSet
                                                {
                                                    DisplayName = z.SPGS_DisplayName,
                                                    DisplayOrderNum = short.TryParse(z.SPGS_DisplayOrderNum, CultureInfo.InvariantCulture, out short dispOrderNo) ? dispOrderNo : null,
                                                }
                                            }
                                        ).ToList();
            return systemPermissionGroupSetGrouping;
        }

        public async Task<bool> SaveUserAuthorization(UserFeatureAccessPermissionDTO authinfo, string role, string brandName, bool? newUser)
        {
            Guard.NotNull(authinfo, nameof(authinfo));
            try
            {
                authinfo.Features = SetDefaultFeatures(authinfo.Features, role);

                var info = newUser != null && newUser == true
                                ? GetNewUserApplicationDetails(authinfo.UserId, authinfo.SystemId, brandName, authinfo.SelectedClientIds)
                                : GetUserApplicationDetailsWithSelectedClients(authinfo.UserId, authinfo.SystemId, brandName, authinfo.SelectedClientIds);
                info.AccessEffectiveDate = authinfo.EffectiveDate;
                info.AccessTerminationDate = authinfo.TerminationDate;

                if (authinfo.AllClients)
                {
                    var tenants = RecursiveGetAssignableItems("Tenant", info.AssignableItemsHeader.AssignableItems);
                    foreach (var tenant in tenants)
                    {
                        tenant.SelectedForUser = true;
                    }
                }
                else
                {
                    var clients = RecursiveGetAssignableItems("Client", info.AssignableItemsHeader.AssignableItems);

                    List<SecurityAssignableItemDTO> assignableItems = [];
                    foreach (var selectedClientId in authinfo.SelectedClientIds)
                    {
                        var selectedClient = authinfo?.SelectedClientsTreeData?.Rows;
                        _logger.LogInformation($"selected Client id ::: {JsonConvert.SerializeObject(selectedClient)}");

                        foreach (var client1 in clients)
                        {
                            var selectedDtoClient = selectedClient.FirstOrDefault(x => x.Id == client1.ClassifiedSegmentInstanceId);

                            if (selectedDtoClient == null)
                            {
                                client1.SelectedForUser = false;
                            }
                            else
                            {
                                client1.SelectedForUser = true;
                                if (!assignableItems.Any(x => x.InstanceNKey == client1.InstanceNKey))
                                {
                                    assignableItems.Add(client1);
                                }
                            }
                        }
                    }

                    info.AssignableItemsHeader.AssignableItems = assignableItems;
                }
                var GetAllFeatures = _sqlRepository.GetAllFeaturesToList(Convert.ToString(ApplicationConstant.SYSTEM_ID, CultureInfo.InvariantCulture));
                foreach (var featureInfo in info.UserFeatureSelections)
                {
                    var featureDto = authinfo.Features.SingleOrDefault(x => x.FeatureId == featureInfo.SystemPermissionGroupSetId);
                    if (featureDto != null)
                    {
                        var featureDetail = GetFeature(featureInfo.SystemPermissionGroupSetId, GetAllFeatures);

                        // TODO: Fix this logic to work with non-1 customs, make it look for ShowsCustomButton  (Data from UpdateFeatureCustomRow)
                        // TODO: Make sure that custom save data is filtered by IsShown                    

                        if (featureDto.SelectedOptionId == -1 ||
                            (featureDto.SelectedOptionIds != null && featureDto.SelectedOptionIds.Any(x => x.Value == -1)) ||
                            CustomPermissionsForIndexReports(authinfo, featureDto))
                        {
                            featureInfo.SimpleSystemPermissionGroupSetGroupingId = null;
                            featureInfo.SimpleSystemPermissionGroupSetGroupingIds = null;
                            featureInfo.AreCustomInstancesSelected = true;
                            featureInfo.CustomSelectedInstances = GetTreeSelections(featureDto.CustomTreeViewOptions, featureDetail);
                        }
                        else if (featureDto.SelectedOptionId != null)
                        {
                            featureInfo.AreCustomInstancesSelected = false;
                            featureInfo.SimpleSystemPermissionGroupSetGroupingId = featureDto.SelectedOptionId;
                        }
                        else if (featureDto.SelectedOptionIds != null)
                        {
                            featureInfo.AreCustomInstancesSelected = false;
                            if (featureInfo.SimpleSystemPermissionGroupSetGroupingIds == null)
                            {
                                featureInfo.SimpleSystemPermissionGroupSetGroupingIds = [];
                            }
                            else
                            {
                                featureInfo.SimpleSystemPermissionGroupSetGroupingIds.Clear();
                            }

                            foreach (var id in featureDto.SelectedOptionIds)
                            {
                                featureInfo.SimpleSystemPermissionGroupSetGroupingIds.Add((int)id);
                            }
                        }
                        else
                        {
                            featureInfo.AreCustomInstancesSelected = false;
                            featureInfo.SimpleSystemPermissionGroupSetGroupingId = null;
                        }
                    }
                }

                // Temporary to deal with too much data going over WCF Call (Failing)
                // Remove children of Clients.
                if (info.AssignableItemsHeader.AssignableItems.Count > 0)
                {
                    foreach (var item in info.AssignableItemsHeader.AssignableItems[0].Children)
                    {
                        item.Children = [];
                    }
                }
                await UpdateUserApplicationDetails(info, userAuthorizationAddress);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public IList<UserManagementEditFeatureDTO> SetDefaultFeatures(IList<UserManagementEditFeatureDTO> features, string role)
        {
            List<string> defaultFeatures = role switch
            {
                var r when r.Equals(UserRoleConstant.ClientUser, StringComparison.InvariantCultureIgnoreCase) =>
                    ["Resources", "Providers", "Client Information", "Account Profile"],
                var r when r.Equals(UserRoleConstant.StopLossCarrier, StringComparison.InvariantCultureIgnoreCase) =>
                    ["Stoploss Reports", "Account Profile", "Resources"],
                var r when r.Equals(UserRoleConstant.SuperAdmin, StringComparison.InvariantCultureIgnoreCase) =>
                    ["UserAdmin", "Resources"],
                _ => []
            };

            foreach (var feature in features)
            {
                if (feature.IsDefault)
                {
                    if (feature.DisplayName == "Providers")
                    {
                        feature.SelectedOptionId = feature.Selections.FirstOrDefault()?.Id;
                        feature.SelectedOptionIds = null;
                    }
                    else
                    {
                        foreach (var item in feature.CustomTreeViewOptions.Rows)
                        {
                            item.Properties = SetPropertiesToFalse(feature.CustomTreeViewOptions.ValueHeaders.Count);
                            foreach (var acc in item.Children)
                            {
                                acc.Properties = SetPropertiesToFalse(feature.CustomTreeViewOptions.ValueHeaders.Count);
                                if (defaultFeatures.Contains(feature.DisplayName))
                                {
                                    feature.SelectedOptionId = -1;
                                    feature.SelectedOptionIds = null;
                                    item.Properties[0] = true;
                                }
                                foreach (var subAcc in acc.Children)
                                {
                                    subAcc.Properties = SetPropertiesToFalse(feature.CustomTreeViewOptions.ValueHeaders.Count);
                                    if (defaultFeatures.Contains(feature.DisplayName))
                                    {
                                        feature.SelectedOptionId = -1;
                                        feature.SelectedOptionIds = null;
                                        item.Properties[0] = true;
                                    }
                                }
                            }

                            if (defaultFeatures.Count > 0 && defaultFeatures.Contains(feature.DisplayName))
                            {
                                feature.SelectedOptionId = -1;
                                feature.SelectedOptionIds = null;
                                item.Properties[0] = true;
                            }
                        }
                    }
                }
            }
            return features;
        }

        public List<SecurityAssignableItemDTO> RecursiveGetAssignableItems(string classifiedSegmentName, ICollection<SecurityAssignableItemDTO> items)
        {
            Guard.NotNull(items, nameof(items));

            var returnItems = items.Where(x => x.ClassifiedSegmentName == classifiedSegmentName).ToList();

            foreach (var item in items)
            {
                if (item.Children.Count > 0)
                {
                    var childItems = RecursiveGetAssignableItems(classifiedSegmentName, item.Children);
                    returnItems.AddRange(childItems);
                }
            }
            return returnItems;
        }

        private static bool CustomPermissionsForIndexReports(UserFeatureAccessPermissionDTO dto, UserManagementEditFeatureDTO featureDto)
        {
            return !dto.AllClients && dto.SelectedClientIds != null && dto.SelectedClientIds.Count > 0 &&
                featureDto.DisplayName == Enums.SystemPermissionGroupSet.INDEXReports.GetTypeTableDisplayName() && featureDto.SelectedOptionId != null;
        }

        private static List<UserFeatureSelectionCustomDTO> GetTreeSelections(TreeGridDTO<int> grid, Models.FeatureDTO feature)
        {
            var dtos = new List<UserFeatureSelectionCustomDTO>();
            for (int i = 0; i < grid.ValueHeaders.Count; i++)
            {
                if (grid.ValueHeaders[i].IsShown)
                {
                    dtos.Add(GetTreeSelections(grid, i, feature));
                }
            }
            return dtos;
        }

        private static UserFeatureSelectionCustomDTO GetTreeSelections(TreeGridDTO<int> grid, int selectionIndex, Models.FeatureDTO feature)
        {
            var curFeatureOption = feature.FeatureSelections.Single(x => x.CustomLabelName == grid.ValueHeaders[selectionIndex].Name);

            return new UserFeatureSelectionCustomDTO()
            {
                SelectedInstanceIds = GetSelectedIndices(grid.Rows, selectionIndex),
                SystemPermissionGroupSetGroupingId = curFeatureOption.SystemPermissionGroupSetGroupingId
            };
        }
        public async Task UpdateUserApplicationDetails(UserApplicationDetailsDTO userDetails, string remoteAddress)
        {
            using (UserAuthorizationServiceNewClient client = new(UserAuthorizationServiceNewClient.EndpointConfiguration.BasicHttpsBinding_IUserAuthorizationServiceNew,
                                                                    remoteAddress))
            {
                await Task.Run(() => client.UpdateUserApplicationDetails(userDetails));
            }
        }
        private static IList<bool?> SetPropertiesToFalse(int ValueHeaderCount)
        {
            IList<bool?> newProps = [];
            for (int i = 0; i < ValueHeaderCount; i++)
            {
                newProps.Add(false);
            }
            return newProps;
        }

        private static List<int> GetSelectedIndices(IList<TreeGridRowDTO<int>> rows, int selectedIndex)
        {
            var selectedRows = rows.Where(
                x =>
                    x.Properties != null
                    && x.Properties.Count > selectedIndex
                    && x.Properties[selectedIndex].HasValue
                    && x.Properties[selectedIndex].Value
            ).Select(x => x.Id).ToList();

            selectedRows.AddRange(
                rows.SelectMany(
                    x =>
                        x.Children.Any() ? GetSelectedIndices(x.Children, selectedIndex) : []
                ).ToList()
            );

            return selectedRows;
        }
    }
}
