namespace CoreLib.Application.Common.Models
{
    public class UserFeatureSelectionDTO
    {
        public UserFeatureSelectionDTO()
        {
            UserInstanceSelections = [];
            SimpleSystemPermissionGroupSetGroupingIds = [];
        }
        public int SystemPermissionGroupSetId { get; set; }
        public int? SimpleSystemPermissionGroupSetGroupingId { get; set; }
        public IList<int> SimpleSystemPermissionGroupSetGroupingIds { get; set; }
        public int? PermissionGroupId { get; set; }
        public bool AreCustomInstancesSelected { get; set; }
        public IList<UserInstanceSelectionDTO> UserInstanceSelections { get; set; }
    }
}
