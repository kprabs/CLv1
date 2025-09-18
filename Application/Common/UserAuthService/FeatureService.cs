using CoreLib.Application.Common.Interfaces;
using CoreLib.Application.Common.Models;
using CoreLib.Application.Common.Utility;

namespace CoreLib.Application.Common.UserAuthService
{
    public class FeatureService(IProvisionHandlerService _provisionHandlerService) : IFeatureService
    {
        public FeatureAccessInfoDTO UpdateFeatureAccessInfo(UserManagementEditFeatureDTO feature, int BrokerClientId)
        {
            var featureAccessInfo = new FeatureAccessInfoDTO
            {
                FeatureId = feature.FeatureId,
                FeatureName = feature.DisplayName
            };
            if (feature.Selections.Count > 0)
            {
                UpdateSubFeatureAccessInfo(featureAccessInfo, feature);
            }
            return featureAccessInfo;
        }

        public void UpdateSubFeatureAccessInfo(FeatureAccessInfoDTO featureAccessInfo, UserManagementEditFeatureDTO feature)
        {
            foreach (var subFeature in feature.Selections)
            {
                var clientHasAccess = new List<ClientHasAccess>();
                //foreach(var client in feature.CustomTreeViewOptions.Rows)
                //{
                //    clientHasAccess.Add(new ClientHasAccess { clientId = client.InstanceNkey, HasAccess = false });
                //}
                if (subFeature.Id < 0) //Removed custom sub features
                {
                    continue;
                }
                var subFeatureAccessInfo = new SubFeatureAccessInfoDTO
                {
                    SubFeatureId = subFeature.Id,
                    SubFeatureName = subFeature.Value,
                    HasAccess = false,
                    ClientHasAccess = clientHasAccess,
                    PermissionCode = subFeature.PermissionCode,
                    Version = _provisionHandlerService.GetAppVersionForPermissionCode(subFeature.PermissionCode).GetTypeTableDisplayName()
                };
                featureAccessInfo.SubFeatures.Add(subFeatureAccessInfo);
            }
            for (int i = 0; i < featureAccessInfo.SubFeatures.Count; i++)
            {
                SubFeatureAccessInfoDTO subFeature = featureAccessInfo.SubFeatures[i];
                if (feature.SelectedOptionId == -1 ||
                    (feature.SelectedOptionIds != null && feature.SelectedOptionIds.Length > 0 && feature.SelectedOptionIds.Any(x => x == -1)))
                {

                    subFeature.ClientHasAccess = new List<ClientHasAccess>();
                    foreach (var client in feature.CustomTreeViewOptions.Rows)
                    {
                        #region Need to check for next release
                        //&& client.Properties != null && client.Properties.Count > i && (bool)client.Properties[i])



                        #endregion
                        if (client.Visible)
                        {
                            subFeature.HasAccess = ((client.Properties.Count > 0 && client.Properties[i] == true)
                                || (client.Children.Where(x => x.Visible).Any(y => y.Properties.Count > 0 && y.Properties[i] == true))
                                || (client.Children.Where(x => x.Visible).Any(y => y.Children.Where(z => z.Visible).Any(q => q.Properties.Count > 0 && q.Properties[i] == true))));

                            subFeature.ClientHasAccess.Add(new ClientHasAccess { clientId = client.InstanceNkey, HasAccess = subFeature.HasAccess });


                            var accountInfo = UpdateClientAccessInfo(client, i);
                            if (accountInfo.Accounts.Count > 0)
                            {
                                subFeature.Clients.Add(accountInfo);
                            }
                            else if (client.Properties != null && client.Properties.Count > i && (bool)client.Properties[i])
                            {
                                subFeature.Clients.Add(accountInfo);
                            }
                            else
                            {
                                //SC: nothing to handle
                            }
                        }
                    }
                }
                else
                {
                    if (feature.SelectedOptionId > -1 && subFeature.SubFeatureId == feature.SelectedOptionId)
                    {
                        subFeature.ClientHasAccess = new List<ClientHasAccess>();

                        foreach (var client in feature.CustomTreeViewOptions.Rows)
                        {
                            if (client.Visible)
                            {
                                subFeature.Clients.Add(UpdateClientAccessInfo(client, i, false));
                            }
                            subFeature.ClientHasAccess.Add(new ClientHasAccess { clientId = client.InstanceNkey, HasAccess = true });
                        }
                        subFeature.HasAccess = true;
                    }
                    else if (feature.SelectedOptionIds != null && feature.SelectedOptionIds.Length > 0)
                    {
                        foreach (var selectedOption in feature.SelectedOptionIds)
                        {
                            if (selectedOption > -1 && selectedOption == feature.SelectedOptionId)
                            {
                                subFeature.ClientHasAccess = new List<ClientHasAccess>();
                                foreach (var client in feature.CustomTreeViewOptions.Rows)
                                {
                                    if (client.Visible)
                                    {
                                        subFeature.Clients.Add(UpdateClientAccessInfo(client, i, false));
                                    }
                                    subFeature.ClientHasAccess.Add(new ClientHasAccess { clientId = client.InstanceNkey, HasAccess = false });

                                }
                                subFeature.HasAccess = true;
                            }
                        }
                    }
                    else
                    {
                        //SC: nothing to handle
                    }
                }
            }
        }

        private ClientAccessInfoDTO UpdateClientAccessInfo(TreeGridRowDTO<int> client, int SubFeaturesPosition, bool IsCustom = true)
        {
            var clientAccessInfo = new ClientAccessInfoDTO
            {
                ClientId = client.InstanceNkey
            };

            foreach (var account in client.Children)
            {
                if (IsCustom)
                {
                    if (account.Visible && account.Properties != null && account.Properties.Count > SubFeaturesPosition && (bool)account.Properties[SubFeaturesPosition])
                    {
                        var subAccountInfo = UpdateAccountAccessInfo(account, SubFeaturesPosition);
                        if (subAccountInfo.SubAccounts.Count > 0)
                        {
                            clientAccessInfo.Accounts.Add(UpdateAccountAccessInfo(account, SubFeaturesPosition));
                        }
                    }
                    else if (account.Visible && account.Properties != null && account.Properties.Count > SubFeaturesPosition)
                    {
                        var subAccountInfo = UpdateAccountAccessInfo(account, SubFeaturesPosition);
                        if (subAccountInfo.SubAccounts.Count > 0)
                        {
                            clientAccessInfo.Accounts.Add(UpdateAccountAccessInfo(account, SubFeaturesPosition, accountSelected: false));
                        }
                    }
                    else
                    {
                        //SC: nothing to handle
                    }
                }
                else
                {
                    var subAccountInfo = UpdateAccountAccessInfo(account, SubFeaturesPosition, false);
                    if (subAccountInfo.SubAccounts.Count > 0)
                    {
                        clientAccessInfo.Accounts.Add(UpdateAccountAccessInfo(account, SubFeaturesPosition, false));
                    }
                }
            }
            return clientAccessInfo;
        }

        private AccountAccessInfoDTO UpdateAccountAccessInfo(TreeGridRowDTO<int> account, int SubFeaturesPosition, bool IsCustom = true, bool accountSelected = true)
        {
            var accountAccessInfo = new AccountAccessInfoDTO
            {
                AccountId = account.InstanceNkey
            };
            if (account.Children.Count > 0)
            {
                foreach (var subAccount in account.Children)
                {
                    if (IsCustom)
                    {
                        if (subAccount.Visible && subAccount.Properties != null && subAccount.Properties.Count > SubFeaturesPosition && (bool)subAccount.Properties[SubFeaturesPosition])
                        {
                            accountAccessInfo.SubAccounts.Add(new SubAccountAccessInfoDTO
                            {
                                SubAccountId = subAccount.InstanceNkey
                            });
                        }
                    }
                    else
                    {
                        if (accountSelected)
                        {
                            accountAccessInfo.SubAccounts.Add(new SubAccountAccessInfoDTO
                            {
                                SubAccountId = subAccount.InstanceNkey
                            });
                        }
                    }
                }
            }
            return accountAccessInfo;
        }
    }
}
