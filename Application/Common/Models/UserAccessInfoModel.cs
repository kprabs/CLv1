using CoreLib.Application.Common.SqlEntities;

namespace CoreLib.Application.Common.Models
{
    public class UserAccessInfoModel
    {
        public List<UserInfoEntity> userInfo { get; set; }
        public List<Tenant> tenants{ get; set; }
        public List<FeatureEntity> features { get; set; }
        public List<FeatureAssingedInstanceEntity> featureAssingedInstances { get; set; }
    }
}
