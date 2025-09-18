namespace CoreLib.Application.Common.Models
{
    public class UserManagementEditFeatureDTO
    {
        public UserManagementEditFeatureDTO()
        {
            Selections = [];
            CustomTreeViewOptions = new();
        }
        public int? FeatureId { get; set; }
        public string DisplayName { get; set; }
        public bool MultiSelectable { get; set; } = false;
        public int? SelectedOptionId { get; set; }
        public int?[] SelectedOptionIds { get; set; }
        public bool AllowsCustomOption { get; set; }
        public string CustomButtonLabel { get; set; }
        public bool IsDefault { get; set; } = false;
        public IList<UserManagementEditFeatureSelectionDTO> Selections { get; set; }
        public TreeGridDTO<int> CustomTreeViewOptions { get; set; }
    }
}
