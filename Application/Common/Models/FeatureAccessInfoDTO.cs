namespace CoreLib.Application.Common.Models
{
    public class FeatureAccessInfoDTO
    {
        public FeatureAccessInfoDTO()
        {
            SubFeatures = [];
        }
        public int? FeatureId { get; set; }

        public string? FeatureName { get; set; }

        public IList<SubFeatureAccessInfoDTO> SubFeatures { get; set; }
    }
}
