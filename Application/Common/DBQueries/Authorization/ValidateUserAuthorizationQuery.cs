namespace CoreLib.Application.Common.DBQueries
{
    public partial class SqlQueries
    {
        public const string ValidateUserAuthorizationQuery = @"select 
			lsu.LogInSystemUserId
			, lsusa.EffDate
			,lsusa.TermDate
			,lsg.GroupName
			,lsg.SourceLogInSystemGroupName
		from main.LogInSystemUser lsu 
			join main.LogInSystemUserSystemAccess lsusa on lsu.LogInSystemUserId=lsusa.LogInSystemUserId
			join code.[System] cs on lsusa.SystemId=cs.SystemId
			join Main.UserLogInSystemGroup ulsg on lsu.LogInSystemUserId=ulsg.LogInSystemUserId
			join main.LogInSystemGroup lsg on ulsg.LogInSystemGroupId=lsg.LogInSystemGroupId

		where 
			lsu.SourceLogInSystemUserName='@UserName' and 
			cs.Code='@SystemCode' and 
			lsg.SourceLogInSystemGroupName='@UserRole' and
			(lsusa.TermDate is null or lsusa.TermDate >=GETDATE())";
    }
}
