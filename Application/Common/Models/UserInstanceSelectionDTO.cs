namespace CoreLib.Application.Common.Models
{
    public class UserInstanceSelectionDTO
    {
        public UserInstanceSelectionDTO()
        {
            SelectedInstanceIds = [];
        }
        public int PermissionGroupId { get; set; }
        public int SystemPermissionGroupSetGroupingId { get; set; }
        public IList<int> SelectedInstanceIds { get; set; }

    }
}
