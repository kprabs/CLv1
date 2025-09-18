namespace CoreLib.Application.Common.Models
{
    public class ClassifiedAreaSegmentHeaderDTO
    {
        public ClassifiedAreaSegmentHeaderDTO()
        {
            ClassifiedAreaSegments = [];
        }
        public int LoginSystemUserId { get; set; }
        public int SystemId { get; set; }
        public IList<ClassifiedAreaSegmentDTO> ClassifiedAreaSegments { get; set; }
    }
}
