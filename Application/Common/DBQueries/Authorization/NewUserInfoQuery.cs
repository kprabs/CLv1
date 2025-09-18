namespace CoreLib.Application.Common.DBQueries
{
    public partial class SqlQueries
    {
        public const string NewUserInfoQuery = @"select DISTINCT
				lsu.LogInSystemUserId
				,lsu.SourceLogInSystemUserName
				,lsu.FirstName
				,lsu.LastName
				,lsu.EmailAddress
				,[sys].Code [SystemCode]
				,ls.Code [LoginSystemCode]
				,lsgs.SourceLogInSystemGroupSetName
				,lsgs.GroupSetName
				,lsu.ActiveFlag
				,lsu.LogInSystemGroupSetId
				,lsusa.SystemId
				,lsusa.EffDate
				,lsusa.TermDate
				,lsusa.LogInSystemUserSystemAccessId
				,lsgai.ClassifiedSegmentInstanceId [AssignableClassifiedSegmentInstanceId]
				,lsg.GroupName
				,lsg.LogInSystemGroupId
				,lsg.SourceLogInSystemGroupName
				,lsusai.ClassifiedSegmentInstanceId [AssignedClassifiedSegmentInstanceId]
			from main.LogInSystemUser lsu 
					join Main.LogInSystemUserSystemAccess lsusa on lsu.LogInSystemUserId=lsusa.LogInSystemUserId
					join Code.[System] [sys] on lsusa.SystemId=[sys].SystemId
					join Main.UserLogInSystemGroup ulsg on lsu.LogInSystemUserId= ulsg.LogInSystemUserId
					join Main.LogInSystemGroup lsg on ulsg.LogInSystemGroupId=lsg.LogInSystemGroupId
					join Main.LogInSystemGroupSet lsgs on lsg.LogInSystemGroupSetId=lsgs.LogInSystemGroupSetId
					join Code.LogInSystem ls on lsgs.LogInSystemId=ls.LogInSystemId
					join Main.LogInSystemGroupAccessibleInstance  lsgai on lsg.LogInSystemGroupId=lsgai.LogInSystemGroupId
					join Main.LogInSystemUserSystemAccessibleInstance lsusai on lsu.LogInSystemUserId=lsusai.LogInSystemUserId and lsusa.SystemId = lsusai.SystemId
		where lsu.LogInSystemUserId=@UserId and lsusa.SystemId=@SystemId and lsgs.GroupSetName='@BrandName'";
    }
}
