namespace CoreLib.Application.Common.DBQueries
{
    public partial class SqlQueries
    {
        public const string GetUserCrossWalkInfoQuery = @"
                SELECT [LogInSystemUserId]
                  ,[ClassifiedSegmentInstanceId]
                  ,[ActiveFlag]
                  ,[AllowAllBillToAccountsFlag]
              FROM [Authorization].[Main].[GroupUserCrossWalk] where LogInSystemUserId=@LOGINUSERID";
    }
}
