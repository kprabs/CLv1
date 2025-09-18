namespace CoreLib.Application.Common.Models
{
    public class UserAccessDetailsDTO
    {
        public UserAccessDetailsDTO()
        {
            SourceLogInSystemGroupNames = [];
            UserFeatureSelections = [];
        }
        public int SystemId { get; set; }
        public int UserId { get; set; }
        public IList<string> SourceLogInSystemGroupNames { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool Active { get; set; }
        public string UserName { get; set; }
        public string TenantName { get; set; }
        public string SystemCode { get; set; }
        public string LogInSystemCode { get; set; }
        public string SourceLogInSystemGroupSetName { get; set; }
        public ClassifiedAreaSegmentHeaderDTO ClassifiedAreaSegmentHeader { get; set; }
        public IList<UserFeatureSelectionDTO> UserFeatureSelections { get; set; }
    }
}
