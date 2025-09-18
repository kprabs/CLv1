using CoreLib.Application.Common.Models;

namespace CoreLib.Application.Common.Interfaces
{
    public interface IFeatureConfigurationService
    {
        bool CheckForHeaders(FeatureRequest request);

        Task<List<CoreLib.Entities.Feature>> GetFeatureConfiguration(string name);
        Task<List<FeatureEvalResponse>> GetFeatureConfigurationEvals(List<FeatureEvalRequest> requests);
    }
}