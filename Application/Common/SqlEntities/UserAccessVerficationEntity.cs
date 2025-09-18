namespace CoreLib.Application.Common.SqlEntities
{
    public class UserAccessVerficationEntity
    {
        public string LogInSystemUserId { get; set; }
        public string EffDate { get; set; }
        public string TermDate { get; set; }
        public string GroupName { get; set; }
    }
}
