namespace CoreLib.Application.Common.DBQueries
{
    public partial class SqlQueries
    {
        public const string AuthGetMasterLoginIdentifier =
            @"SELECT MasterLoginSystemUserID,
				MasterLoginSystemUserNKey,
				AllowAllBillToAccountsFlag, 
				gcw.ClassifiedSegmentInstanceId as ClassifiedSegmentInstanceId,
				csi.ClassifiedAreaSegmentNKey as ClassifiedAreaSegmentNKey
            FROM main.GroupUserCrossWalk gcw
			join main.ClassifiedSegmentInstance csi on 
				csi.ClassifiedSegmentInstanceId=GCW.ClassifiedSegmentInstanceId
	            JOIN main.LogInSystemUser lsu 
		            ON gcw.LogInSystemUserId = lsu.LogInSystemUserId AND lsu.SourceLogInSystemUserName='@username'";
    }
}
