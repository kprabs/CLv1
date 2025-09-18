using AHA.IS.Common.Authorization.DTO.New;

namespace CoreLib.Application.Common.Models
{
    public class GetSelectedOptionIdForNameModel
    {
        public CoreLib.Application.Common.Models.FeatureDTO featureInfo { get; set; }
        public UserApplicationDetailsDTO userInfo { get; set; }
        public AHA.IS.Common.Authorization.DTO.New.UserFeatureSelectionDTO featureSelection { get; set; }
        public UserFeatureAccessPermissionDTO dto { get; set; }
        public int BrokerClientId { get; set; }
        public bool? IsMbrAvailable { get; set; }
        public bool? IsIndRptAvailable { get; set; }
        public bool? IsStlsRptAvailable { get; set; }
    }
}
