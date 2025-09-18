namespace CoreLib.Application.Common.Models
{
    public class FeatureDTO
    {
        public FeatureDTO()
        {
            AllowedInstanceClassifiedSegmentCodes = [];
            FeatureSelections = [];
        }
        public int SystemPermissionGroupSetId { get; set; }
        public string Name { get; set; }
        public int? DisplayOrder { get; set; }
        public bool AllowsCustomAccounts { get; set; }
        public IList<string> AllowedInstanceClassifiedSegmentCodes { get; set; }
        public IList<FeatureSelectionDTO> FeatureSelections { get; set; }
    }
}
