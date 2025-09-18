using CoreLib.Application.Common.Constants;
using CoreLib.Application.Common.Interfaces;
using CoreLib.Application.Common.Models;
using CoreLib.Application.Common.Utility;
using CoreLib.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Globalization;

namespace CoreLib.Application.Common.UserAuthService
{
    internal static partial class AutoHandlerServiceMessages
    {
        [LoggerMessage(Level = LogLevel.Information, EventId = 1, Message = "AutoProvisionLog :: SystemUserName = {SystemUserName}, " +
             "isAutoProvision = {isAutoProvision}, ClientNkey = {ClientNkey}, " +
             "CreateDate = {CreateDate}, RemarkDetail = {RemarkDetail}, dto = {dto}")]
        internal static partial void AutoProvisionLog(this ILogger logger, string SystemUserName, bool isAutoProvision, string ClientNkey, DateTime CreateDate,
                                                        string RemarkDetail, string dto);

        [LoggerMessage(Level = LogLevel.Information, EventId = 1, Message = "BeforeAutoProvisionLog :: SystemUserName = {SystemUserName}, " +
                     "dto = {dto}")]
        internal static partial void BeforeAutoProvisionLog(this ILogger logger, string SystemUserName, string dto);


    }
    public partial class AutoHandlerService(ISqlRepository sqlRepository, IProvisionHandlerService provisionHandlerService,
                                            IGroupPortalRepository groupPortalRepository, IClientInformationService clientInformationService,
                                            ILogger<AutoHandlerService> logger) : IAutoHandlerService
    {
        private string RemarkDetails = DefaultString;
        const string DefaultString = "Added Provisions ";       
        private string RequestGUID;
        private List<string> Accounts = [];
        private List<string> SubAccounts = [];
        private List<string> BTA = [];
        private const int RemarkDetailsMaxLength = 1000;

        public void AutoProvisioning(UserFeatureAccessPermissionDTO dto, string requestGUID)
        {
            Accounts = [];
            SubAccounts = [];
            BTA = [];
            RequestGUID = requestGUID;
            groupPortalRepository.SetAutoProvisionLog(new AutoProvisionLog
            {
                ClientNkey = dto.SelectedClientIds.FirstOrDefault().ToString(CultureInfo.InvariantCulture),
                CreateDate = DateTime.Now,
                RemarkDetail = string.Empty,
                SystemUserName = dto.UserName,
                AutoProvisionStatusCode = AutoPorvisionStatusCode.Initiated,
                RequestUserIdNKey = RequestGUID,
            });
            foreach (int clientId in dto.SelectedClientIds)
            {
                if (!string.IsNullOrEmpty(dto.TenantName) && IsAutoProvisionsConcent(dto.UserId, clientId.ToString(CultureInfo.InvariantCulture)))
                {
                    groupPortalRepository.UpdateAutoProvisionLog(RequestGUID, AutoPorvisionStatusCode.InProgress, string.Empty);
                    _ = ApplyAutoProvisions(dto);
                }
                else
                {
                    groupPortalRepository.UpdateAutoProvisionLog(RequestGUID, AutoPorvisionStatusCode.Completed, "It is Custom Provision");
                }
            }
            //if the concent is true check if the provisions for the subaccounts and bta's have same provisions
            //if the provisions are same then apply the same to other subaccounts and bta's
            //call save authorization
            //log the status.
        }

        public string? AutoProvisionStatus(string RID)
        {
            return groupPortalRepository.GetAutoProvisionLog(RID)?.AutoProvisionStatusCode;
        }

        public bool IsAutoProvisionsConcent(int loginId, string ClassifiedSegmentInstanceId)
        {
            bool isAutoProvision = false;
            UserConsentType? userConsentType = sqlRepository.GetUserConsentType(loginId, ClassifiedSegmentInstanceId);
            if (userConsentType == null)
            {
                isAutoProvision = false;
            }
            else if (userConsentType.ConsentFlag.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase))
            {
                isAutoProvision = true;
            }
            else
            {
                //SC: nothing to handle
            }
            return isAutoProvision;
        }

        private UpdatedClient ApplyAutoProvisions(UserFeatureAccessPermissionDTO actualDto)
        {
            bool isAutoPorvisionApplicable = false;
            bool isAutoProvisionForAllFeatures = false;
            UserFeatureAccessPermissionDTO updatedDto = actualDto;
            Dictionary<string, List<AccountDto>?> icpAccountsInfoDictionary = new Dictionary<string, List<AccountDto>?>();
            logger.BeforeAutoProvisionLog(actualDto.UserName, JsonConvert.SerializeObject(updatedDto));

            string selectedClientId = string.Empty;
            try
            {
                if (actualDto.SelectedClientsTreeData?.Rows.Count > 0 && actualDto.SelectedClientIds.Count > 0)
                {
                    foreach (var client in actualDto.SelectedClientsTreeData?.Rows)
                    {
                        if (!icpAccountsInfoDictionary.ContainsKey(client.InstanceNkey))
                        {

                            //selectedClientId = actualDto.SelectedClientsTreeData.Rows.First(x => x.Id == actualDto.SelectedClientIds.FirstOrDefault()).InstanceNkey;
                            var getICPInforForClient = clientInformationService.SearchAccountAndSubaccount(new SearchAccountAndSubaccountRequestDto { ClientId = client.InstanceNkey }).Result;
                            icpAccountsInfoDictionary.Add(client.InstanceNkey, (getICPInforForClient?.data));
                        }
                    }
                }

                if (icpAccountsInfoDictionary?.Count > 0)
                {
                    foreach (var feature in actualDto.Features)
                    {
                        if (feature.CustomTreeViewOptions.Rows.Count > 0)
                        {
                            foreach (var client in feature.CustomTreeViewOptions.Rows)
                            {
                                if (client.Children.Count > 0)
                                {
                                    List<AccountDto>? icpAccountsInfo = new();
                                    if (icpAccountsInfoDictionary.ContainsKey((string)client.InstanceNkey))
                                    {
                                        icpAccountsInfo = icpAccountsInfoDictionary[client.InstanceNkey];
                                    }
                                    var accountSelectionList = client.Children.Select(x => x.Properties).ToList();
                                    var selectedAccountProvisions = accountSelectionList.Where(accountProperties => accountProperties
                                                                                                        .Any(value => value == true)).ToList();
                                    var unSelectedAccountProvisions = accountSelectionList.Where(accountProperties => accountProperties
                                                                                                        .All(value => value == false)).ToList();
                                    if (selectedAccountProvisions.Count != 0
                                        && accountSelectionList.Select(subList => subList.Count).ToList().Count == selectedAccountProvisions.Count
                                        && selectedAccountProvisions.All(obj => obj.SequenceEqual(selectedAccountProvisions.First())))
                                    {
                                        var accounts = client.Children.Where(accounts => accounts.Children.Count > 0).ToList();
                                        foreach (var account in accounts)
                                        {
                                            List<AccountDto?> selectedAccount = icpAccountsInfo?.Where(x => x.AccountId == account.InstanceNkey).ToList();
                                            UpdatedProvision obj = UpdateSubAccountProps(account, icpAccountsInfo);
                                            if (obj.IsAutoPorvisionApplicable)
                                            {
                                                if (selectedAccount.Count > 0)
                                                {
                                                    isAutoPorvisionApplicable = true;
                                                    updatedDto.Features.First(updateFeature => updateFeature.FeatureId == feature.FeatureId)
                                                                                                .CustomTreeViewOptions.Rows.First(updateClient => updateClient.Id == client.Id)
                                                                                                .Children.First(updateAccount => updateAccount.Id == account.Id).Properties = obj.Account.Properties;

                                                    foreach (var subAccount in account.Children)
                                                    {
                                                        isAutoPorvisionApplicable = true;
                                                        updatedDto.Features.First(updateFeature => updateFeature.FeatureId == feature.FeatureId)
                                                    .CustomTreeViewOptions.Rows.First(updateClient => updateClient.Id == client.Id)
                                                        .Children.First(updateAccount => updateAccount.Id == account.Id)
                                                            .Children.First(updateSubAccount => updateSubAccount.Id == subAccount.Id).Properties = obj.Account.Children.First(x => x.Id == subAccount.Id).Properties;

                                                    }
                                                }
                                                else
                                                {
                                                    updatedDto.Features.First(updateFeature => updateFeature.FeatureId == feature.FeatureId)
                                                                                                        .CustomTreeViewOptions.Rows.First(updateClient => updateClient.Id == client.Id)
                                                                                                        .Children.First(updateAccount => updateAccount.Id == account.Id)
                                                                                                        .Properties = Enumerable.Repeat<bool?>(false, obj.Account.Properties.Count)
                                                                                                        .ToList();
                                                }
                                            }
                                        }
                                    }
                                    else if (selectedAccountProvisions.Count > 0 && selectedAccountProvisions.Count + unSelectedAccountProvisions.Count == accountSelectionList.Count
                                         && selectedAccountProvisions.All(obj => obj.SequenceEqual(selectedAccountProvisions.FirstOrDefault())))
                                    {
                                        var accounts = client.Children.Where(accounts => accounts.Children.Count > 0).ToList();

                                        //// START - SUBACCOUNTS OF ALL ACCOUNTS CHECK ////
                                        //IList<IList<bool?>> subAccountPropertiesList = new List<IList<bool?>>();
                                        //foreach (var account in accounts)
                                        //{
                                        //    var selectedSubAccountList = account.Children.Where(subAccounts => subAccounts.Properties.Any(x => x == true)).ToList();
                                        //    foreach (var selectedSubAccount in selectedSubAccountList)
                                        //    {
                                        //        subAccountPropertiesList.Add(selectedSubAccount.Properties);
                                        //    }
                                        //}
                                        //if ((subAccountPropertiesList.Any() && subAccountPropertiesList.All(obj => obj.SequenceEqual(subAccountPropertiesList.First()))) || !subAccountPropertiesList.Any())
                                        //// END - SUBACCOUNTS OF ALL ACCOUNTS CHECK ////

                                        foreach (var account in accounts)
                                        {
                                            List<AccountDto?> selectedAccount = icpAccountsInfo?.Where(x => x.AccountId == account?.InstanceNkey).ToList();

                                            UpdatedProvision obj = UpdateSubAccountProps(account, selectedAccountProvisions.First(), icpAccountsInfo);
                                            if (obj.IsAutoPorvisionApplicable)
                                            {
                                                if (selectedAccount?.Count > 0)
                                                {
                                                    isAutoPorvisionApplicable = true;
                                                    updatedDto.Features.First(updateFeature => updateFeature.FeatureId == feature.FeatureId)
                                                                                                        .CustomTreeViewOptions.Rows.First(updateClient => updateClient.Id == client.Id)
                                                                                                        .Children.First(updateAccount => updateAccount.Id == account.Id)
                                                                                                        .Properties = obj.Account.Properties;

                                                    foreach (var subAccount in account.Children)
                                                    {

                                                        isAutoPorvisionApplicable = true;
                                                        updatedDto.Features.First(updateFeature => updateFeature.FeatureId == feature.FeatureId)
                                                            .CustomTreeViewOptions.Rows.First(updateClient => updateClient.Id == client.Id)
                                                                .Children.First(updateAccount => updateAccount.Id == account.Id)
                                                                    .Children.First(updateSubAccount => updateSubAccount.Id == subAccount.Id)
                                                                    .Properties = obj.Account.Children.First(x => x.Id == subAccount.Id).Properties;
                                                    }
                                                }
                                                else
                                                {
                                                    updatedDto.Features.First(updateFeature => updateFeature.FeatureId == feature.FeatureId)
                                                              .CustomTreeViewOptions.Rows.First(updateClient => updateClient.Id == client.Id)
                                                              .Children.First(updateAccount => updateAccount.Id == account.Id).Properties = Enumerable.Repeat<bool?>(false, obj.Account.Properties.Count)
                                                              .ToList();
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (var account in client.Children.Where(accounts => accounts.Children.Count > 0))
                                        {
                                            IList<IList<bool?>> subAccountProvisionsList = [];
                                            foreach (var subAccount in account.Children)
                                            {
                                                subAccountProvisionsList.Add(subAccount.Properties);
                                            }
                                            var selectedSubAccountProvisionsList = subAccountProvisionsList.Where(subAccountProperties => subAccountProperties.Any(value => value == true));
                                            if (selectedSubAccountProvisionsList.Any())
                                            {
                                                if (selectedSubAccountProvisionsList.All(obj => obj.SequenceEqual(selectedSubAccountProvisionsList.First())))
                                                {
                                                    UpdatedProvision obj = UpdateSubAccountProps(account, selectedSubAccountProvisionsList.First(), icpAccountsInfo);
                                                    if (obj.IsAutoPorvisionApplicable)
                                                    {

                                                        isAutoPorvisionApplicable = true;
                                                        updatedDto.Features.First(updateFeature => updateFeature.FeatureId == feature.FeatureId)
                                                                                                            .CustomTreeViewOptions.Rows.First(updateClient => updateClient.Id == client.Id)
                                                                                                            .Children.First(updateAccount => updateAccount.Id == account.Id)
                                                                                                            .Properties = obj.Account.Properties;

                                                        foreach (var subAccount in account.Children)
                                                        {

                                                            isAutoPorvisionApplicable = true;
                                                            updatedDto.Features.First(updateFeature => updateFeature.FeatureId == feature.FeatureId)
                                                                      .CustomTreeViewOptions.Rows.First(updateClient => updateClient.Id == client.Id)
                                                                      .Children.First(updateAccount => updateAccount.Id == account.Id)
                                                                      .Children.First(updateSubAccount => updateSubAccount.Id == subAccount.Id).Properties = obj.Account.Children.First(x => x.Id == subAccount.Id)
                                                                      .Properties;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    updatedDto.Features.FirstOrDefault(updateFeature => updateFeature.FeatureId == feature.FeatureId)
                                                            .CustomTreeViewOptions.Rows.First(updateClient => updateClient.Id == client.Id)
                                                                .Children.First(updateAccount => updateAccount.Id == account.Id).Properties = account.Properties;
                                                    foreach (var subAccount in account.Children)
                                                    {
                                                        //subAccount.Properties = obj.Account.Children.First(x => x.Id == subAccount.Id).Properties;
                                                        updatedDto.Features.First(updateFeature => updateFeature.FeatureId == feature.FeatureId)
                                                        .CustomTreeViewOptions.Rows.First(updateClient => updateClient.Id == client.Id)
                                                            .Children.First(updateAccount => updateAccount.Id == account.Id)
                                                                .Children.First(updateSubAccount => updateSubAccount.Id == subAccount.Id).Properties = account.Children.First(x => x.Id == subAccount.Id).Properties;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                updatedDto.Features.FirstOrDefault(updateFeature => updateFeature.FeatureId == feature.FeatureId)
                                                        .CustomTreeViewOptions.Rows.First(updateClient => updateClient.Id == client.Id)
                                                            .Children.First(updateAccount => updateAccount.Id == account.Id).Properties = account.Properties;
                                                foreach (var subAccount in account.Children)
                                                {
                                                    //subAccount.Properties = obj.Account.Children.First(x => x.Id == subAccount.Id).Properties;
                                                    updatedDto.Features.First(updateFeature => updateFeature.FeatureId == feature.FeatureId)
                                                    .CustomTreeViewOptions.Rows.First(updateClient => updateClient.Id == client.Id)
                                                        .Children.First(updateAccount => updateAccount.Id == account.Id)
                                                            .Children.First(updateSubAccount => updateSubAccount.Id == subAccount.Id).Properties = account.Children.First(x => x.Id == subAccount.Id).Properties;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (feature.DisplayName != Enums.SystemPermissionGroupSet.INDEXReports.GetTypeTableDisplayName() ||
                                feature.DisplayName != Enums.SystemPermissionGroupSet.Resources.GetTypeTableDisplayName() ||
                                feature.DisplayName != Enums.SystemPermissionGroupSet.UserAdmin.GetTypeTableDisplayName())
                            {
                                feature.SelectedOptionId = -1;
                                feature.SelectedOptionIds = [];
                            }
                            //revisedFeature.CustomTreeViewOptions.Rows.Where(c => c.Id == client.Id).ToList().ForEach(revisedClient => revisedClient = revisedClientInfo);

                        }
                        if (isAutoPorvisionApplicable && !isAutoProvisionForAllFeatures)
                        {
                            isAutoProvisionForAllFeatures = true;
                        }
                        else if (!isAutoPorvisionApplicable && isAutoProvisionForAllFeatures)
                        {
                            break;
                        }
                        else
                        {
                            //SC: nothing to handle
                        }
                    }
                    if (isAutoPorvisionApplicable)
                    {
                        UpdatedClient updatedClient = new()
                        {
                            Client = updatedDto,
                            IsAutoPorvisionApplicable = isAutoPorvisionApplicable
                        };
                        bool isSaveUserProvisions = provisionHandlerService.SaveUserAuthorization(updatedDto, updatedDto.OAMRoleCode, updatedDto.TenantName, null).Result;
                        if (isSaveUserProvisions)
                        {
                            RemarkDetails = DefaultString;
                            RemarkDetails += Accounts.Count > 0 ? $"[A]: {String.Join(",", Accounts)} " : String.Empty;
                            RemarkDetails += SubAccounts.Count > 0 ? $"[SA]: {String.Join(",", SubAccounts)} " : String.Empty;
                            RemarkDetails += BTA.Count > 0 ? $"[BTA]:{String.Join(",", BTA)} " : String.Empty;
                            LogAutoProvisionsAction(updatedDto, true, RemarkDetails, AutoPorvisionStatusCode.Completed);
                            var saveNotes = new SystemUserLoginLog
                            {
                                LastUpdateDate = DateTime.Now,
                                LastUpdateUserName = "AutoProvision",
                                RemarkDetail = RemarkDetails.Length > RemarkDetailsMaxLength ? $"{RemarkDetails.Substring(0, RemarkDetailsMaxLength - 3)}..." : RemarkDetails,
                                SystemUserName = updatedDto.UserName,
                                NewData = string.Empty,
                                PreviousData = string.Empty
                            };
                            if (Accounts.Count > 0 || SubAccounts.Count > 0 || BTA.Count > 0)
                            {
                                groupPortalRepository.SetUserLoginLogAsync(saveNotes);
                            }
                        }
                        else
                        {
                            LogAutoProvisionsAction(updatedDto, true, "Unable to save provisions", AutoPorvisionStatusCode.Error);
                        }
                        return updatedClient;
                        //updated Provisions

                        //revisedDto.Features.Where(x => x.FeatureId == feature.FeatureId && x.DisplayName == feature.DisplayName).ToList().ForEach(revisableFeature => revisableFeature = revisedFeature);
                    }
                }
                LogAutoProvisionsAction(actualDto, false, "No Changes Made", AutoPorvisionStatusCode.Completed);
            }
            catch (Exception ex)
            {
                LogAutoProvisionsAction(actualDto, false, ex.StackTrace, AutoPorvisionStatusCode.Error);
            }

            return new UpdatedClient
            {
                Client = actualDto,
                IsAutoPorvisionApplicable = isAutoPorvisionApplicable
            };
        }

        private void LogAutoProvisionsAction(UserFeatureAccessPermissionDTO updatedDto, bool isAutoProvision, string remarks, string statusCode)
        {
            string clientNKey = updatedDto.SelectedClientsTreeData?.Rows.First(x => x.Id == updatedDto.SelectedClientIds?.First()).InstanceNkey;
            DateTime currentDate = DateTime.Now;

            logger.AutoProvisionLog(updatedDto.UserName, isAutoProvision, clientNKey, currentDate, remarks, JsonConvert.SerializeObject(updatedDto));
            //groupPortalRepository.SetAutoProvisionLog(new Entities.AutoProvisionLog
            //{
            //    AutoProvisionFlag = isAutoProvision,
            //    ClientNkey = clientNKey,
            //    CreateDate = currentDate,
            //    RemarkDetail = remarks,
            //    SystemUserName = updatedDto.UserName,
            //    AutoProvisionStatusCode="initiated",
            //    RequestUserIdNKey = System.Guid.NewGuid().ToString(),
            //});

            groupPortalRepository.UpdateAutoProvisionLog(RequestGUID, statusCode,
                                            remarks.Length > RemarkDetailsMaxLength ? $"{remarks.Substring(0, RemarkDetailsMaxLength - 3)}..." : remarks);
        }

        private UpdatedProvision UpdateSubAccountProps(TreeGridRowDTO<int> account, List<AccountDto>? icpAccountDetails)
        {
            bool isAutoPorvisionApplicable = false;
            try
            {
                IList<IList<bool?>> subAccountSelectionList = [];
                AccountDto? selectedAccount = icpAccountDetails?.Find(x => x.AccountId == account.InstanceNkey);
                if (selectedAccount != null)
                {
                    foreach (var subAccount in account.Children)
                    {
                        subAccountSelectionList.Add(subAccount.Properties);
                    }
                    var selectedSubAccounts = subAccountSelectionList.Where(subAccount => subAccount.Any(value => value == true));


                    //if (account.Children.Any(subAccountProperties => subAccountProperties.Properties.Any(value => value == true)
                    //                                    && account.Children.Where(otherSubAccounts => otherSubAccounts.Properties != subAccountProperties.Properties)
                    //                                                .All(otherProps => otherProps.Properties.All(otherValues => otherValues == false))))

                    if (selectedSubAccounts.Any() && selectedSubAccounts.All(eachSubAccount => eachSubAccount.SequenceEqual(selectedSubAccounts.First())))
                    {
                        var selectedSubaccountsList = account.Children.Where(subAccountProperties => subAccountProperties.Properties.Any(value => value == true))
                                                                       .Select(x => x.Properties);
                        var selectedSuAccountsInfoList = account.Children.Where(subAccountProperties => subAccountProperties.Properties.Any(value => value == true));
                        if (selectedSubaccountsList.Any() && (selectedSubaccountsList.All(obj => obj.SequenceEqual(selectedSubaccountsList.First()))))
                        {
                            var unSelectedSubaccountsList = account.Children.Where(subAccountProperties => subAccountProperties.Properties.All(value => value == false));
                            if (unSelectedSubaccountsList.Any())
                            {
                                foreach (var subAccount in account.Children)
                                {
                                    if (selectedAccount.Subaccounts.Any(x => x.SubaccountId == subAccount.InstanceNkey) || subAccount.InstanceNkey.Length > 10)
                                    {
                                        isAutoPorvisionApplicable = true;
                                        if (!RemarkDetails.Contains(subAccount.InstanceNkey + ", ", StringComparison.InvariantCulture)
                                            && !subAccount.Properties.Any(value => value == true))
                                        {
                                            RemarkDetails += subAccount.InstanceNkey + ", ";
                                            if (subAccount.InstanceNkey.Length > 10)
                                            {
                                                BTA.Add(subAccount.InstanceNkey);
                                            }
                                            else
                                            {
                                                SubAccounts.Add(subAccount.InstanceNkey);
                                            }
                                        }
                                        subAccount.Properties = selectedSubaccountsList.First();
                                    }
                                }
                                account.Properties = selectedSubaccountsList.First();

                            }
                        }
                        else if (account.Properties.Any(x => x == true))
                        {
                            foreach (var subAccount in account.Children)
                            {
                                if (selectedAccount.Subaccounts.Any(x => x.SubaccountId == subAccount.InstanceNkey) || subAccount.InstanceNkey.Length > 10)
                                {
                                    isAutoPorvisionApplicable = true;
                                    if (!RemarkDetails.Contains(subAccount.InstanceNkey + ", ", StringComparison.InvariantCulture) && !subAccount.Properties.Any(value => value == true))
                                    {
                                        RemarkDetails += subAccount.InstanceNkey + ", ";
                                        if (subAccount.InstanceNkey.Length > 10)
                                        {
                                            BTA.Add(subAccount.InstanceNkey);
                                        }
                                        else
                                        {
                                            SubAccounts.Add(subAccount.InstanceNkey);
                                        }
                                    }
                                    subAccount.Properties = account.Properties;
                                }

                            }
                        }
                        else
                        {
                            //SC: nothing to handle
                        }
                    }
                    else if (!selectedSubAccounts.Any() && account.Properties.Any(x => x == true) && account.Children.Count > 0)
                    {
                        foreach (var subAccount in account.Children)
                        {
                            if (selectedAccount.Subaccounts.Any(x => x.SubaccountId == subAccount.InstanceNkey) || subAccount.InstanceNkey.Length > 10)
                            {
                                isAutoPorvisionApplicable = true;
                                if (!RemarkDetails.Contains(subAccount.InstanceNkey + ", ", StringComparison.InvariantCultureIgnoreCase) && !subAccount.Properties.Any(value => value == true))
                                {
                                    RemarkDetails += subAccount.InstanceNkey + ", ";
                                    if (subAccount.InstanceNkey.Length > 10)
                                    {
                                        BTA.Add(subAccount.InstanceNkey);
                                    }
                                    else
                                    {
                                        SubAccounts.Add(subAccount.InstanceNkey);
                                    }
                                }
                                subAccount.Properties = account.Properties;
                            }

                        }
                    }
                    else
                    {
                        //SC: nothing to handle
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation("AutoProvision Exception UpdateSubAccountProps 1 :: " + ex.StackTrace);
            }
            return new UpdatedProvision { Account = account, IsAutoPorvisionApplicable = isAutoPorvisionApplicable };
        }
        private UpdatedProvision UpdateSubAccountProps(TreeGridRowDTO<int> account, IList<bool?> propertiesToBeUpdatedWith, List<AccountDto>? icpAccountDetails)
        {
            bool isAutoPorvisionApplicable = false;
            try
            {
                AccountDto? selectedAccount = icpAccountDetails?.Find(x => x.AccountId == account.InstanceNkey);
                if (selectedAccount != null) //&& (account.Properties.All(x => x == false)))
                {
                    ////START - SUBACCOUNTS OF SINGLE ACCOUNT CHECK////
                    var selectedSubAccountList = account.Children.Where(subAccounts => subAccounts.Properties.Any(x => x == true)).ToList();
                    if (!RemarkDetails.Contains(account.InstanceNkey + ", ", StringComparison.InvariantCulture) && (!account.Properties.Any(value => value == true) && selectedSubAccountList.Count == 0))
                    {
                        RemarkDetails += account.InstanceNkey + ", ";
                        Accounts.Add(account.InstanceNkey);
                    }

                    IList<IList<bool?>> subAccountPropertiesList = [];
                    foreach (var subAccount in account.Children)
                    {
                        subAccountPropertiesList.Add(subAccount.Properties);
                    }
                    if (selectedSubAccountList.Count != 0)
                    {
                        if (selectedSubAccountList.All(others => others.Properties.SequenceEqual(selectedSubAccountList.First().Properties)))
                        {
                            foreach (var subAccount in account.Children)
                            {
                                if (selectedAccount.Subaccounts.Any(x => x.SubaccountId == subAccount.InstanceNkey) || subAccount.InstanceNkey.Length > 10)
                                {
                                    isAutoPorvisionApplicable = true;

                                    if (!RemarkDetails.Contains(subAccount.InstanceNkey + ", ", StringComparison.InvariantCultureIgnoreCase) && !subAccount.Properties.Any(value => value == true))
                                    {
                                        RemarkDetails += subAccount.InstanceNkey + ", ";
                                        if (subAccount.InstanceNkey.Length > 10)
                                        {
                                            BTA.Add(subAccount.InstanceNkey);
                                        }
                                        else
                                        {
                                            SubAccounts.Add(subAccount.InstanceNkey);
                                        }
                                    }
                                    subAccount.Properties = selectedSubAccountList.First().Properties;
                                }
                                else if (subAccount.Properties.Any(x => x == true))
                                {
                                    subAccount.Properties = subAccount.Properties;
                                }
                                else
                                {
                                    //SC: nothing to handle
                                }

                            }
                            account.Properties = selectedSubAccountList.First().Properties;
                        }
                    }
                    else
                    {
                        if ((subAccountPropertiesList.Any() && subAccountPropertiesList.All(obj => obj.SequenceEqual(subAccountPropertiesList.First()))) || !subAccountPropertiesList.Any())
                        {
                            foreach (var subAccount in account.Children)
                            {
                                if (selectedAccount.Subaccounts.Any(x => x.SubaccountId == subAccount.InstanceNkey) || subAccount.InstanceNkey.Length > 10)
                                {
                                    isAutoPorvisionApplicable = true;
                                    if (!RemarkDetails.Contains(subAccount.InstanceNkey + ", ", StringComparison.InvariantCultureIgnoreCase) && !subAccount.Properties.Any(value => value == true))
                                    {
                                        RemarkDetails += subAccount.InstanceNkey + ", ";
                                        if (subAccount.InstanceNkey.Length > 10)
                                        {
                                            BTA.Add(subAccount.InstanceNkey);
                                        }
                                        else
                                        {
                                            SubAccounts.Add(subAccount.InstanceNkey);
                                        }
                                    }
                                    subAccount.Properties = propertiesToBeUpdatedWith;
                                }

                            }
                            account.Properties = propertiesToBeUpdatedWith;
                        }
                        else if (selectedSubAccountList.Count == 0 && account.Properties.Any(x => x == true) && account.Children.Count > 0)
                        {
                            foreach (var subAccount in account.Children)
                            {
                                if (selectedAccount.Subaccounts.Any(x => x.SubaccountId == subAccount.InstanceNkey) || subAccount.InstanceNkey.Length > 10)
                                {
                                    isAutoPorvisionApplicable = true;
                                    if (!RemarkDetails.Contains(subAccount.InstanceNkey + ", ", StringComparison.InvariantCultureIgnoreCase) && !subAccount.Properties.Any(value => value == true))
                                    {
                                        RemarkDetails += subAccount.InstanceNkey + ", ";
                                        if (subAccount.InstanceNkey.Length > 10)
                                        {
                                            BTA.Add(subAccount.InstanceNkey);
                                        }
                                        else
                                        {
                                            SubAccounts.Add(subAccount.InstanceNkey);
                                        }
                                    }
                                    subAccount.Properties = account.Properties;
                                }

                            }
                        }
                        else
                        {
                            //SC: nothing to handle
                        }
                    }
                    ////END - SUBACCOUNTS OF SINGLE ACCOUNT CHECK////

                    ////START - NO VALIDATION AS SUBACCOUNTS ALL ACCOUNTS CHECK DONE UP////
                    ///isAutoPorvisionApplicable = true;
                    ///foreach (var subAccount in account.Children)
                    ///{
                    ///    if (!RemarkDetails.Contains(subAccount.InstanceNkey + ", "))
                    ///        RemarkDetails += subAccount.InstanceNkey + ", ";
                    ///
                    ///    subAccount.Properties = account.Properties;
                    ///}
                    ////END - NO VALIDATION AS SUBACCOUNTS ALL ACCOUNTS CHECK DONE UP////

                }

            }
            catch (Exception ex)
            {
                logger.LogInformation("AutoProvision Exception UpdateSubAccountProps 2 :: " + ex.StackTrace);
            }
            return new UpdatedProvision { Account = account, IsAutoPorvisionApplicable = isAutoPorvisionApplicable };
        }

        public static bool CompareClients(TreeGridRowDTO<int> client, TreeGridRowDTO<int> revisedClientInfo)
        {
            bool isEqual = true;
            isEqual = !((client.Children.Count != revisedClientInfo.Children.Count || client.Children.Sum(account => account.Children.Count) != revisedClientInfo.Children.Sum(account => account.Children.Count))
                        || (client.Children.Select(account => account.InstanceNkey).Except(revisedClientInfo.Children.Select(revisedAccount => revisedAccount.InstanceNkey)).Any())
                        || (client.Children.SelectMany(account => account.Children).OfType<TreeGridRowDTO<int>>().Select(subAccount => subAccount.InstanceNkey)
                                .Except(revisedClientInfo.Children.SelectMany(revisedAccount => revisedAccount.Children).OfType<TreeGridRowDTO<int>>().Select(revisedSubAccount => revisedSubAccount.InstanceNkey)).Any()));

            return isEqual;
        }
    }

    public class UpdatedProvision
    {
        public bool IsAutoPorvisionApplicable { get; set; }
        public TreeGridRowDTO<int> Account { get; set; }
    }

    public class UpdatedClient
    {
        public bool IsAutoPorvisionApplicable { get; set; }
        public UserFeatureAccessPermissionDTO Client { get; set; }
    }
}
