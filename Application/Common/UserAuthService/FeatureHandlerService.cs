using AHA.IS.Common.Authorization.DTO.New;
using CoreLib.Application.Common.Constants;
using CoreLib.Application.Common.Enums;
using CoreLib.Application.Common.Interfaces;
using CoreLib.Application.Common.Models;
using CoreLib.Application.Common.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace CoreLib.Application.Common.UserAuthService
{
    public class FeatureHandlerService(IConfiguration configuration, ILogger<FeatureHandlerService> logger) : IFeatureHandlerService
    {       
        public List<TreeGridRowDTO<int>> GetTreeGridRows(List<SecurityAssignableItemDTO> items,
            AHA.IS.Common.Authorization.DTO.New.UserFeatureSelectionDTO selections,
            List<int> valueHeaderIds, IList<string> allowedSegmentCodes, string brandName, int level = 0,
            int parentId = 0)
        {
            return
                items.Where(y => allowedSegmentCodes.Contains(y.ClassifiedSegmentCode)).Select(
                x =>
                    new TreeGridRowDTO<int>
                    {
                        Id = x.ClassifiedSegmentInstanceId,
                        Text = x.InstanceName,
                        ShowID = x.ClassifiedSegmentCode == ClassifiedSegmentEnum.TradingPartner.GetTypeTableCode(),
                        InstanceNkey = x.InstanceNKey,
                        Expanded = false,
                        Visible = (level == 0),
                        LevelIndex = level,
                        Children = x.Children.Count > 0 ? GetTreeGridRows(x.Children, selections, valueHeaderIds, allowedSegmentCodes,brandName, level + 1) : [],
                        Properties = GetRowProperties(x, selections, valueHeaderIds, x.ClassifiedSegmentInstanceId),
                        //AccountCount = x.Children.Count,
                        //SubAccountCount = GetAllSubAccountsCount(x.Children),
                        ParentClassifiedSegmentName = brandName
                    }
            ).ToList();
        }

        public int GetAllSubAccountsCount(List<SecurityAssignableItemDTO> accounts)
        {
            int subAccountCount = 0;
            foreach (SecurityAssignableItemDTO account in accounts)
            {
                subAccountCount += account.Children.Count;
            }
            return subAccountCount;
        }

        public List<bool?> GetRowProperties(SecurityAssignableItemDTO item, AHA.IS.Common.Authorization.DTO.New.UserFeatureSelectionDTO selections,
                                            List<int> valueHeaderIds, int parentId = 0)
        {
            if (!selections.AreCustomInstancesSelected)
            {
                return [];
            }

            List<bool?> properties = [];
            foreach (var valueHeaderId in valueHeaderIds)
            {
                var column = selections.CustomSelectedInstances.SingleOrDefault(y => y.SystemPermissionGroupSetGroupingId == valueHeaderId);

                if (column != null)
                {
                    var selected = column.SelectedInstanceIds.Contains(item.ClassifiedSegmentInstanceId);
                    properties.Add(selected);
                }
                else
                {
                    properties.Add(false);
                }
            }
            return properties;
        }

        public ICollection<string> GetConfiguredWorkbaskets()
        {
            var activeConfigureWorkbaskets = ApplicationConstant.ActiveWorkbasketNames.Split(',')
                                                            .Select(x => x.Trim()).ToList();
            return activeConfigureWorkbaskets;
        }

        public int? GetSelectedOptionId(UserApplicationDetailsDTO userInfo, Models.FeatureDTO featureInfo, AHA.IS.Common.Authorization.DTO.New.UserFeatureSelectionDTO featureSelection, bool allClients)
        {
            int? selectedOptionId = null;
            if (featureSelection.AreCustomInstancesSelected && !allClients)
            {
                var items = userInfo.UserFeatureSelections.Where(x => x.SystemPermissionGroupSetId == featureInfo.SystemPermissionGroupSetId)
                                                          .Select(x => x.CustomSelectedInstances).ToList();
                foreach (var item in items)
                {
                    if (item.Count > 0)
                    {
                        selectedOptionId = item.Find(x => x.SelectedInstanceIds.Count > 0)?.SystemPermissionGroupSetGroupingId;
                    }
                }
                return selectedOptionId == 0 ? null : selectedOptionId;
            }
            else
            {
                return featureSelection.SimpleSystemPermissionGroupSetGroupingId;
            }
        }

        public int? GetSelectedOptionId(AHA.IS.Common.Authorization.DTO.New.UserFeatureSelectionDTO featureSelection)
        {
            return featureSelection.SimpleSystemPermissionGroupSetGroupingId ?? (featureSelection.AreCustomInstancesSelected ? (int?)-1 : null);
        }

        public int? GetSelectedOptionIdForName(Models.FeatureDTO featureInfo, UserApplicationDetailsDTO userInfo,
                                                AHA.IS.Common.Authorization.DTO.New.UserFeatureSelectionDTO featureSelection,
                                                UserFeatureAccessPermissionDTO dto, bool isStopLoss)
        {
            Models.FeatureSelectionDTO? featureSelectionCode = featureInfo.FeatureSelections
                    .FirstOrDefault(x => configuration.GetValue<string>("STOP_LOSS_PERMISSION").Split(",").Contains(x.PermissionCode));

            return featureSelectionCode?.SystemPermissionGroupSetGroupingId;
        }

        public int? GetSelectedOptionIdForName(GetSelectedOptionIdForNameModel selectedOptionIdForName)
        {
            if (selectedOptionIdForName.dto.AllClients)
            {
                Models.FeatureSelectionDTO? featureSelectionCode = selectedOptionIdForName.featureInfo.FeatureSelections
                     .FirstOrDefault(x => ApplicationConstant.SUPER_ADMIN_PERMISSION.Split(",").Contains(x.PermissionCode));
                if (featureSelectionCode != null)
                {
                    return featureSelectionCode.SystemPermissionGroupSetGroupingId;
                }
            }

            if (selectedOptionIdForName.BrokerClientId != ApplicationConstant.BROKER_CLIENT_ID_DEFAULT)
            {
                Models.FeatureSelectionDTO? featureSelectionCode = selectedOptionIdForName.featureInfo.FeatureSelections
                    .FirstOrDefault(x => ApplicationConstant.BROKER_PERMISSION.Split(",").Contains(x.PermissionCode));

                if (selectedOptionIdForName.IsMbrAvailable == true && featureSelectionCode == null)
                {
                    featureSelectionCode = selectedOptionIdForName.featureInfo.FeatureSelections.FirstOrDefault(x =>
                            ApplicationConstant.BROKER_MEMBER_ACCECC_CHECK.Split(",").Contains(x.PermissionCode));
                }

                if ((selectedOptionIdForName.IsIndRptAvailable == true ||
                    selectedOptionIdForName.IsMbrAvailable == true ||
                    selectedOptionIdForName.IsStlsRptAvailable == true) && featureSelectionCode == null)
                {
                    featureSelectionCode = selectedOptionIdForName.featureInfo.FeatureSelections.FirstOrDefault(x =>
                            ApplicationConstant.BROKER_INDEX_REPORT_ACCECC_CHECK.Split(",").Contains(x.PermissionCode));
                }

                if (featureSelectionCode != null)
                {
                    return featureSelectionCode.SystemPermissionGroupSetGroupingId;
                }
            }

            if (selectedOptionIdForName.featureInfo.Name == SystemPermissionGroupSet.AccountProfile.GetTypeTableDisplayName()
                && selectedOptionIdForName.BrokerClientId != ApplicationConstant.BROKER_CLIENT_ID_DEFAULT)
            {
                return selectedOptionIdForName.featureInfo.FeatureSelections.FirstOrDefault().SystemPermissionGroupSetGroupingId;
            }

            if (selectedOptionIdForName.featureInfo.Name == SystemPermissionGroupSet.INDEXReports.GetTypeTableDisplayName())
            {
                return GetSelectedOptionId(selectedOptionIdForName.userInfo, selectedOptionIdForName.featureInfo, selectedOptionIdForName.featureSelection, selectedOptionIdForName.dto.AllClients);
            }

            if (selectedOptionIdForName.featureInfo.Name != SystemPermissionGroupSet.ClientReports.GetTypeTableDisplayName())
            {
                return GetSelectedOptionId(selectedOptionIdForName.featureSelection);
            }
            return null;
        }

        public int?[] GetSelectedOptionIds(AHA.IS.Common.Authorization.DTO.New.UserFeatureSelectionDTO featureSelection)
        {
            if (featureSelection.SimpleSystemPermissionGroupSetGroupingIds != null)
            {
                return featureSelection.SimpleSystemPermissionGroupSetGroupingIds.Select(x => (int?)x).ToArray();
            }

            if (featureSelection.AreCustomInstancesSelected)
            {
                return [-1];
            }
            return [];
        }

        public UserManagementEditFeatureDTO UpdateHardcodeFeature(Models.FeatureDTO featureDto, UserManagementEditFeatureDTO userFeatureDto)
        {
            Guard.NotNull(featureDto, nameof(featureDto));
            Guard.NotNull(userFeatureDto, nameof(userFeatureDto));

            if (featureDto.Name == SystemPermissionGroupSet.INDEXReports.GetTypeTableDisplayName())
            {
                userFeatureDto.CustomButtonLabel = "Select Accounts";
            }
            else if (featureDto.Name == SystemPermissionGroupSet.ClaimsWorkbaskets.GetTypeTableDisplayName())
            {
                userFeatureDto.CustomButtonLabel = "Select Workbaskets";
            }
            else if (featureDto.Name == SystemPermissionGroupSet.EDI.GetTypeTableDisplayName())
            {
                userFeatureDto.CustomButtonLabel = "Select Trading Parners";
            }
            else
            {
                // SQ: default?
            }

            if (featureDto.Name == SystemPermissionGroupSet.ClientReports.GetTypeTableDisplayName())
            {
                userFeatureDto.MultiSelectable = true;
            }
            return userFeatureDto;
        }
        public List<SecurityAssignableItemDTO> RecursiveGetAssignableItems(string classifiedSegmentName, ICollection<SecurityAssignableItemDTO> items)
        {
            try
            {
                Guard.NotNull(items, nameof(items));

                var returnItems = items.Where(x => x.ClassifiedSegmentName == classifiedSegmentName).ToList();

                foreach (var item in items)
                {
                    if (item.Children?.Count > 0)
                    {
                        var childItems = RecursiveGetAssignableItems(classifiedSegmentName, item.Children);
                        returnItems.AddRange(childItems);
                    }
                }
                return returnItems;
            }
            catch (Exception ex)
            {
                logger.Exception(ex);
                return null;
            }
        }
        public IList<SelectedClientIdsDTO> GetSelectedClient(UserAccessInfoDTO userAccessInfo, UserFeatureAccessPermissionDTO userInfo, int BrokeyClientId)
        {
            if (userInfo.SelectedClientIds != null && userInfo.SelectedClientIds.Count > 0)
            {
                foreach (int Id in userInfo.SelectedClientIds)
                {
                    var client = userInfo.Features.SelectMany(f => f.CustomTreeViewOptions.Rows)
                            .FirstOrDefault(c => c.Id == Id);
                    if (client == null && BrokeyClientId != ApplicationConstant.BROKER_CLIENT_ID_DEFAULT)
                    {
                        client = userInfo.Features.SelectMany(f => f.CustomTreeViewOptions.Rows)
                            .FirstOrDefault(c => c.InstanceNkey == Id.ToString(CultureInfo.InvariantCulture));
                    }
                    if (client != null)
                    {
                        userAccessInfo.SelectedClientIds.Add(new SelectedClientIdsDTO()
                        {
                            clientId = client.InstanceNkey,
                            clientName = client.Text
                        });
                    }
                }
            }
            return userAccessInfo.SelectedClientIds;
        }

        public void AddCustomFeatures(IList<FeatureAccessInfoDTO> features, string userRole)
        {
            var customFeature = new FeatureAccessInfoDTO
            {
                FeatureId = 0,
                FeatureName = "Custom Feature"
            };
            features.Add(customFeature);

            customFeature.SubFeatures.Add(new SubFeatureAccessInfoDTO
            {
                SubFeatureId = 1,
                SubFeatureName = "Client Switch",
                PermissionCode = PermissioCodeConstant.ClientSwitch,
                HasAccess = false,
                ClientHasAccess = new List<ClientHasAccess>(),
                Version = AppVerions.NotRelease.GetTypeTableDisplayName()
            });
            customFeature.SubFeatures.Add(new SubFeatureAccessInfoDTO
            {
                SubFeatureId = 2,
                SubFeatureName = "Print Summary",
                PermissionCode = PermissioCodeConstant.PrintSummary,
                HasAccess = false,
                ClientHasAccess = new List<ClientHasAccess>(),
                Version = AppVerions.NotRelease.GetTypeTableDisplayName()
            });
            customFeature.SubFeatures.Add(new SubFeatureAccessInfoDTO
            {
                SubFeatureId = 3,
                SubFeatureName = "Order Card(s) by Mail",
                PermissionCode = PermissioCodeConstant.OrderCardByEmail,
                HasAccess = false,
                ClientHasAccess = new List<ClientHasAccess>(),
                Version = AppVerions.NotRelease.GetTypeTableDisplayName()
            });
            customFeature.SubFeatures.Add(new SubFeatureAccessInfoDTO
            {
                SubFeatureId = 4,
                SubFeatureName = "Email Card(s)",
                PermissionCode = PermissioCodeConstant.EmailCard,
                HasAccess = false,
                ClientHasAccess = new List<ClientHasAccess>(),
                Version = AppVerions.NotRelease.GetTypeTableDisplayName()
            });
            customFeature.SubFeatures.Add(new SubFeatureAccessInfoDTO
            {
                SubFeatureId = 5,
                SubFeatureName = "User Access Reports",
                PermissionCode = PermissioCodeConstant.UserAccessReports,
                HasAccess = false,
                ClientHasAccess = new List<ClientHasAccess>(),
                Version = AppVerions.NotRelease.GetTypeTableDisplayName()
            });
            customFeature.SubFeatures.Add(new SubFeatureAccessInfoDTO
            {
                SubFeatureId = 6,
                SubFeatureName = "T&C Version History",
                PermissionCode = PermissioCodeConstant.TnCVersionHistory,
                HasAccess = false,
                ClientHasAccess = new List<ClientHasAccess>(),
                Version = AppVerions.NotRelease.GetTypeTableDisplayName()
            });
            customFeature.SubFeatures.Add(new SubFeatureAccessInfoDTO
            {
                SubFeatureId = 7,
                SubFeatureName = "Privacy Policy",
                PermissionCode = PermissioCodeConstant.PrivacyPolicy,
                HasAccess = false,
                ClientHasAccess = new List<ClientHasAccess>(),
                Version = AppVerions.NotRelease.GetTypeTableDisplayName()
            });
            customFeature.SubFeatures.Add(new SubFeatureAccessInfoDTO
            {
                SubFeatureId = 8,
                SubFeatureName = "Message Center",
                PermissionCode = PermissioCodeConstant.MessageCenter,
                HasAccess = false,
                ClientHasAccess = new List<ClientHasAccess>(),
                Version = AppVerions.NotRelease.GetTypeTableDisplayName()
            });
            customFeature.SubFeatures.Add(new SubFeatureAccessInfoDTO
            {
                SubFeatureId = 9,
                SubFeatureName = "Help Center",
                PermissionCode = PermissioCodeConstant.HelpCenter,
                HasAccess = false,
                ClientHasAccess = new List<ClientHasAccess>(),
                Version = AppVerions.NotRelease.GetTypeTableDisplayName()
            });
            customFeature.SubFeatures.Add(new SubFeatureAccessInfoDTO
            {
                SubFeatureId = 10,
                SubFeatureName = "Temporary link to Employer Portal",
                PermissionCode = PermissioCodeConstant.LinkEmpPortal,
                HasAccess = HasAccessByUserRole(PermissioCodeConstant.LinkEmpPortal, userRole),
                ClientHasAccess = new List<ClientHasAccess>(),
                Version = AppVerions.Ver1.GetTypeTableDisplayName()
            });
            customFeature.SubFeatures.Add(new SubFeatureAccessInfoDTO
            {
                SubFeatureId = 11,
                SubFeatureName = "Temporary link to Ebill",
                PermissionCode = PermissioCodeConstant.LinkEBill,
                HasAccess = HasAccessByUserRole(PermissioCodeConstant.LinkEBill, userRole),
                ClientHasAccess = new List<ClientHasAccess>(),
                Version = AppVerions.Ver1.GetTypeTableDisplayName()
            });
            customFeature.SubFeatures.Add(new SubFeatureAccessInfoDTO
            {
                SubFeatureId = 12,
                SubFeatureName = "Client Id & Name",
                PermissionCode = PermissioCodeConstant.ClientInfo,
                HasAccess = HasAccessByUserRole(PermissioCodeConstant.ClientInfo, userRole),
                ClientHasAccess = new List<ClientHasAccess>(),
                Version = AppVerions.Ver1.GetTypeTableDisplayName()
            });
            customFeature.SubFeatures.Add(new SubFeatureAccessInfoDTO
            {
                SubFeatureId = 13,
                SubFeatureName = "SideNav For Reports",
                PermissionCode = PermissioCodeConstant.SideNavForReports,
                HasAccess = false,
                ClientHasAccess = new List<ClientHasAccess>(),
                Version = AppVerions.NotRelease.GetTypeTableDisplayName()
            });
        }
        public bool HasAccessByUserRole(string permissionCode, string userRole)
        {
            if (string.IsNullOrWhiteSpace(userRole))
            {
                return false;
            }
            List<string>? userRoleList = [];
            var dicUserRoleByCodeList = UserRoleConstant.GetUserRolesByPermissionCode();
            var dicUserRoleByCode = dicUserRoleByCodeList.Find(d => d.ContainsKey(permissionCode));
            dicUserRoleByCode?.TryGetValue(permissionCode, out userRoleList);
            var hasUserAccess = userRoleList
                .Exists(x => x.Contains(userRole, StringComparison.OrdinalIgnoreCase));
            return hasUserAccess;
        }
    }
}
