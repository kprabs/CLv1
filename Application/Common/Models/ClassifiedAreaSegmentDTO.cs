namespace CoreLib.Application.Common.Models
{
    public class ClassifiedAreaSegmentDTO
    {
        public ClassifiedAreaSegmentDTO()
        {
            Children = [];
        }
        public int ClassifiedSegmentInstanceId { get; set; }
        public string ClassifiedSegmentName { get; set; }
        public string ClassifiedSegmentCode { get; set; }
        public int ClassifiedAreaId { get; set; }
        public string ClassifiedAreaName { get; set; }
        public string ClassifiedAreaCode { get; set; }
        public string InstanceNKey { get; set; }
        public string InstanceName { get; set; }
        public bool SelectedForUser { get; set; }
        public IList<ClassifiedAreaSegmentDTO> Children { get; set; }
    }
}
