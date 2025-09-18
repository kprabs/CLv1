using CoreLib.Application.Common.Models;

namespace CoreLib.Application.Common.Interfaces
{
    public interface IFeatureService
    {
        void UpdateSubFeatureAccessInfo(FeatureAccessInfoDTO featureAccessInfo, UserManagementEditFeatureDTO feature);
        FeatureAccessInfoDTO UpdateFeatureAccessInfo(UserManagementEditFeatureDTO feature, int BrokerClientId);
    }
}
