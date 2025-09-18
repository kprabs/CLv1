namespace CoreLib.Application.Common.DBQueries
{
    public partial class SqlQueries
    {
        public const string UserListBySearchCriteriaMainQuery = @"

SELECT DISTINCT RESULT.FirstName,
RESULT.LastName,
RESULT.UserName,
RESULT.Active, 
RESULT.LastLogin, 
RESULT.EmailAddress, 
RESULT.LoginSystemUserId,
COUNT(CASE WHEN RESULT.ClassifiedAreaSegmentId <> 4 THEN RESULT.ClientNKey END) as ClientCount,
RESULT.ClassifiedAreaSegmentId

FROM (
select	
		lsu.FirstName [FirstName],
						lsu.LastName [LastName],
						lsu.SourceLogInSystemUserName [UserName],
						[Active] = case  when lsu.ActiveFlag = 1 then 'true' else 'false' end,
						lsu.LastLogInDateTime [LastLogin],
						lsu.EmailAddress [EmailAddress],
						lsu.LoginSystemUserId [LoginSystemUserId],
						csic.ClassifiedAreaSegmentNKey as [ClientNKey],
						csic.ClassifiedAreaSegmentId as [ClassifiedAreaSegmentId]	
from Main.LogInSystemUser lsu 
	join main.LogInSystemUserSystemAccess lsusa on lsu.LogInSystemUserId=lsusa.LogInSystemUserId
	JOIN Main.LogInSystemUserSystemAccessibleInstance lsusai ON lsu.LogInSystemUserId =lsusai.LogInSystemUserId
	JOIN Code.[System] [sys] ON lsusa.SystemId=[sys].SystemId
	JOIN Main.ClassifiedSegmentInstance csic ON lsusai.ClassifiedSegmentInstanceId=csic.ClassifiedSegmentInstanceId 
where  lsusa.SystemId=@SystemId 
@UserNameCondition
@FirstNameCondition
@LastNameCondition
@EmailCondition
@ActiveFlagCondition

) RESULT GROUP BY RESULT.FirstName,
RESULT.LastName,
RESULT.UserName,
RESULT.Active, 
RESULT.LastLogin, 
RESULT.EmailAddress, 
RESULT.LoginSystemUserId,
RESULT.ClassifiedAreaSegmentId
						 ";
        public const string UserListBySearchCriteriaUserNameSubQuery = @" AND lsu.SourceLogInSystemUserName =  '@UserName'";
        public const string UserListBySearchCriteriaFirstNameSubQuery = @" AND lsu.FirstName like '@FirstName%'";
        public const string UserListBySearchCriteriaLastNameSubQuery = @" AND lsu.LastName like '@LastName%'";
        public const string UserListBySearchCriteriaEmailAddressSubQuery = @" AND lsu.EmailAddress =  '@Email' ";
        public const string UserListBySearchCriteriaActiveFlagSubQuery = @" AND lsu.ActiveFlag =  @Status,case when lsu.ActiveFlag='true' then 1 else 0 end) ";
		public const string ClientUserCondition = @" AND (csic.ClassifiedAreaSegmentId<>4)";
    }

}
