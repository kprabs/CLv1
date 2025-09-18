namespace CoreLib.Application.Common.SqlEntities
{
    public class UserInfoEntity
    {
        public string LogInSystemUserId { get; set; }
        public string SourceLogInSystemUserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string SystemCode { get; set; }
        public string LoginSystemCode { get; set; }
        public string SourceLogInSystemGroupSetName { get;set; }
        public string ActiveFlag { get; set; }
        public string LogInSystemGroupSetId { get; set; }
        public string SystemId { get; set; }
        public string EffDate { get; set; }
        public string TermDate { get; set; }
        public string LogInSystemUserSystemAccessId { get; set; }
        public string AssignableClassifiedSegmentInstanceId { get; set; }
        public string GroupName { get; set; }
        public string LogInSystemGroupId { get; set; }
        public string SourceLogInSystemGroupName { get; set; }
        public string AssignedClassifiedSegmentInstanceId { get; set; }
    }
}
