namespace CoreLib.Application.Common.DBQueries
{
    public partial class SqlQueries
    {
        public const string GetAutoProvisionConcentForUserQuery = @"SELECT [UserConsentTypeId]
              ,[LogInSystemUserId]
              ,[ClassifiedSegmentInstanceId]
              ,[ConsentTypeId]
              ,[ConsentFlag]
              ,[CreateDate]
              ,[CreateUserNKey]
              ,[LastUpdateDate]
              ,[LastUpdateUserNKey]
           FROM [Main].[UserConsentType] WHERE @UserId
           @ClassifiedSegmentInstanceId";
    }
}
