namespace CoreLib.Application.Common.Models
{
    public class UserConsentType
    {
        public string UserConsentTypeId { get; set; }
        public string LogInSystemUserId { get; set; }
        public string ClassifiedSegmentInstanceId { get; set; }
        public string ConsentTypeId { get; set; }
        public string ConsentFlag { get; set; }
        public string CreateDate { get; set; }
        public string CreateUserNKey { get; set; }
        public string LastUpdateDate { get; set; }
        public string LastUpdateUserNKey { get; set; }
    }
}
