namespace CoreLib.Application.Common.Models
{
    public class UserManagementEditFeatureSelectionDTO : LookupEntryDTO<int, string>
    {
        public bool ShowsCustomButton { get; set; } = false;
        public int? RestrictCustomId { get; set; }
    }
}
