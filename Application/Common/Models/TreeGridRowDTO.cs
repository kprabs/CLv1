namespace CoreLib.Application.Common.Models
{
    public class TreeGridRowDTO<T>
    {
        public TreeGridRowDTO()
        {
            Properties = [];
            ShowID = false;
        }
        public T Id { get; set; }
        public string Text { get; set; }
        public int LevelIndex { get; set; }
        public bool Visible { get; set; }
        public bool Expanded { get; set; }
        public bool ShowID { get; set; } = false;
        public string InstanceNkey { get; set; }
        public string ClassifiedAreaName { get; set; }
        public string ClassifiedSegmentName { get; set; }
        public string ParentClassifiedSegmentName { get; set; }
        public string? ClientStatus { get; set; }
        public IList<bool?> Properties { get; set; }
        public IList<TreeGridRowDTO<T>> Children { get; set; }
        public int AccountCount { get; set; }
        public int SubAccountCount { get; set; }
        public string PlatformIndicator { get; set; }
        public IList<AHA.IS.Common.Authorization.DTO.New.SecurityAssignableItemDTO> Accounts { get; set; }
        public bool IsAllSelected { get; set; }
    }
}
