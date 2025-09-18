namespace CoreLib.Application.Common.Models
{
    public class GroupPortalCrossWalkDTO
    {
        public string LogInSystemUserId { get; set; }
        public string ClassifiedSegmentInstanceId { get; set; }
        public string ActiveFlag { get; set; }
        public string AllowAllBillToAccountsFlag { get; set; }
    }
    public class GroupPortalCrossWalkPartialDTO
    {
        public string MasterLoginSystemUserId { get; set; }
        public string MasterLoginSystemUserNKey { get; set; }
        public string AllowAllBillToAccountsFlag { get; set; }
        public string ClassifiedSegmentInstanceId { get; set; }
        public string ClassifiedAreaSegmentNKey { get; set; }
    }
}
