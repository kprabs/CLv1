namespace CoreLib.Application.Common.DBQueries
{
    public partial class SqlQueries
    {
        public const string GetAllClientNKeyAssociatedToUserQuery = @"
                select DISTINCT lsu.SourceLogInSystemUserName, up.LogInSystemUserId,  upi.ClassifiedSegmentInstanceId, csi.ClassifiedAreaSegmentNKey
                    from Main.UserPermissionInstanceDetail upi
                                    join main.UserPermission up on up.UserPermissionId=upi.UserPermissionId
                                    join main.ClassifiedSegmentInstance csi on csi.ClassifiedSegmentInstanceId = upi.ClassifiedSegmentInstanceId
                                    join main.LogInSystemUser lsu on up.LogInSystemUserId=lsu.LogInSystemUserId
                    where lsu.SourceLogInSystemUserName = '@LOGINUSERNAME' and csi.ClassifiedAreaSegmentId=1";
    }
}
