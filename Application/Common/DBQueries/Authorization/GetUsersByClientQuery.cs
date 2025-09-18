namespace CoreLib.Application.Common.DBQueries
{
    public partial class SqlQueries
    {
        public const string UsersByClientQuery = @"select seg.ClassifiedAreaSegmentNKey as Clientid,usr.SourceLogInSystemUserName as UserId,
            usr.FirstName, usr.LastName, lg.GroupName as GroupRole,lgs.GroupSetName as BrandName 
            from main.ClassifiedSegmentInstance seg,
            main.LogInSystemUserSystemAccessibleInstance cl,
            main.LogInSystemUser usr,
            main.UserLogInSystemGroup ulg,
            main.LogInSystemGroup lg,
            main.LogInSystemGroupSet lgs
            where seg.ClassifiedSegmentInstanceId=cl.ClassifiedSegmentInstanceId
            and cl.LogInSystemUserId=usr.LogInSystemUserId
            and seg.ClassifiedAreaSegmentNKey='@CID'
            and seg.ClassifiedAreaSegmentId=1
            and usr.LogInSystemUserId=ulg.LogInSystemUserId
            and ulg.LogInSystemGroupId=lg.LogInSystemGroupId
            and lg.LogInSystemGroupSetId=lgs.LogInSystemGroupSetId";
        public const string UserInfoBrandNameSubQuery = @" and lgs.GroupSetName= '@BrandName'";
        public const string UserInfoUserTypeSubQuery = @" and lg.GroupName = '@userType'";
    }
}