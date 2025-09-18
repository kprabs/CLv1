using CoreLib.Application.Common.Enums;
using CoreLib.Application.Common.Models;
using Microsoft.Extensions.Configuration;
#pragma warning disable S2360
namespace CoreLib.Application.Common.Utility
{
    public static class VersionBased
    {

        //private static List<string> phiSubFeature = new List<string>
        //    {
        //        "EBPP_INV_PHI", "EBPP_MAKE_PMT", "CLM_SD","CLM_SDE","CLM_SDEL","CLWB_VIEW","CLWB_EDIT", "IDX_MEM_RPT","INV_SUMDTL","REP_PHI","REP_PHIPLS","REP_INTL","REF_VIEW","REF_EDIT"
        //    };
        public static UserFeatureAccessPermissionDTO GetVersionizedFeatures(this UserFeatureAccessPermissionDTO dto, IConfiguration configuration, bool isStopLoss = false)
        {
            List<UserManagementEditFeatureDTO> featureDTOs = [];
            foreach (var feature in dto.Features)
            {
                UserManagementEditFeatureDTO featureDTO = new()
                {
                    AllowsCustomOption = feature.AllowsCustomOption,
                    CustomButtonLabel = feature.CustomButtonLabel,
                    DisplayName = feature.DisplayName,
                    FeatureId = feature.FeatureId,
                    MultiSelectable = feature.MultiSelectable,
                    SelectedOptionId = !isStopLoss ? feature.SelectedOptionId : StopLossSetProvision(feature.DisplayName, feature.Selections, configuration),
                    SelectedOptionIds = feature.SelectedOptionIds,
                    IsDefault = IsDefault(feature.DisplayName, configuration),
                    Selections = updatePHIInfo(feature.Selections.Where(x => IsPermissionAvailable(x.PermissionCode, configuration)).ToList(), configuration),

                    CustomTreeViewOptions = new TreeGridDTO<int>
                    {
                        LevelHeaders = feature.CustomTreeViewOptions.LevelHeaders,
                        Rows = feature.CustomTreeViewOptions.Rows,
                        ValueHeaders = UpdatePHIInfo(feature.CustomTreeViewOptions.ValueHeaders.Where(x => IsPermissionAvailable(x.PermissionCode, configuration)).ToList(), configuration),
                    }
                };
                if (featureDTO.Selections.Count > 0)
                {
                    featureDTOs.Add(featureDTO);
                }
            }
            dto.Features = featureDTOs;
            return dto;
        }

        private static List<string> GetPhiSubFeatures(IConfiguration configuration)
        {
            return configuration.GetValue<string>("PHI_SUB_FEATURES").Split(",").ToList();
        }

        private static int? StopLossSetProvision(string featureName, IList<UserManagementEditFeatureSelectionDTO> valueHeaders, IConfiguration configuration)
        {
            List<string> defaultFeatures = configuration.GetValue<string>("STOPLOSS_USER_DEFAULT_FEATURES").Split(",").ToList();
            if (defaultFeatures.Contains(featureName))
            {
                return valueHeaders[0].Id;
            }

            return null;
        }

        private static bool IsDefault(string feature, IConfiguration configuration)
        {
            List<string> defaultFeatures = configuration.GetValue<string>("CLIENTUSER_DEFAULT_FEATURES").Split(",").ToList();
            return defaultFeatures.Contains(feature);
        }
        private static bool IsPermissionAvailable(string permissionCode, IConfiguration configuration)
        {
            var featureVerionMapping = new Dictionary<AppVerions, List<string>>();
            List<string> featureList = configuration.GetValue<string>("ALL_AVAILABLE_PERMISSIONS").Split(",").ToList();
            featureVerionMapping.Add(AppVerions.Ver1, featureList);

            foreach (var key in featureVerionMapping.Keys)
            {
                featureVerionMapping.TryGetValue(key, out List<string> features);
                if (features.Contains(permissionCode))
                {
                    return true;
                }
            }
            return false;
        }

        private static IList<UserManagementEditFeatureSelectionDTO> updatePHIInfo(IList<UserManagementEditFeatureSelectionDTO> selectionDTOs, IConfiguration configuration)
        {
            foreach (var selection in selectionDTOs)
            {
                if (GetPhiSubFeatures(configuration).Contains(selection.PermissionCode))
                {
                    selection.IsPHI = true;
                }
            }
            return selectionDTOs;
        }

        private static IList<TreeGridHeaderDTO> UpdatePHIInfo(IList<TreeGridHeaderDTO> selectionDTOs, IConfiguration configuration)
        {

            foreach (var selection in selectionDTOs)
            {
                if (GetPhiSubFeatures(configuration).Contains(selection.PermissionCode))
                {
                    selection.IsPHI = true;
                }
            }
            return selectionDTOs;
        }
    }
}
