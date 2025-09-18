namespace CoreLib.Application.Common.Models
{
    public class FeatureSelectionDTO
    {
        public int SystemPermissionGroupSetGroupingId { get; set; }
        public string LabelName { get; set; }
        public string CustomLabelName { get; set; }
        public int? DisplayOrderNum { get; set; }
        public string PermissionCode { get; set; }
    }
}
