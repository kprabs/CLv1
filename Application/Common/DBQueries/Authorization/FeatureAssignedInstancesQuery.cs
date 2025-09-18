namespace CoreLib.Application.Common.DBQueries
{
    public partial class SqlQueries
    {
        public const string FeatureAssignedInstancesQuery = @"select 
				up.PermissionGroupId [UP_PermissionGroupId],
				up.CustomAccessFlag [UP_CustomAccessFlag],
				up.UserPermissionId [UP_UserPermissionId],
				up.TermDate [UP_TermDate],
				up.EffDate [UP_EffDate],
				up.LogInSystemUserId [UP_LogInSystemUserId],
				lsu.SourceLogInSystemUserName [LSU_LogInUserName],
				lsu.LogInSystemGroupSetId [LSU_LogInSystemGroupSetId],
				lsu.ActiveFlag [LSU_ACTIVEFLAG],		
				upid.ClassifiedSegmentInstanceId [UPID_ClassifiedSegmentInstanceId],
				upid.ExcludeFlag [UPID_ExcludeFlag],
				upid.UserPermissionId [UPID_UserPermissionId],
				upid.UserPermissionInstanceDetailId [UPID_UserPermissionInstanceDetailId],
				pg.PermissionGroupName [PG_PermissionGroupName],
				pg.PermissionGroupNKey [PG_PermissionGroupNKey],
				spgsg.SystemPermissionGroupSetId [SPGSG_SystemPermissionGroupSetId],
				spgsg.LabelName [SPGSG_LabelName],
				spgsg.CustomLabelName [SPGSG_CustomLabelName],
				spgsg.DisplayOrderNum [SPGSG_DisplayOrderNum],
				spgsg.LowestAssignableClassifiedAreaSegmentId [SPGSG_LowestAssignableClassifiedAreaSegmentId],
				spgsg.PermissionGroupId [SPGSG_PermissionGroupId],
				spgsg.SystemPermissionGroupSetGroupingId [SPGSG_SystemPermissionGroupSetGroupingId],
				spgs.DisplayName [SPGS_DisplayName],
	spgs.DisplayOrderNum [SPGS_DisplayOrderNum],
	spgs.SystemId [SPGS_SystemId],
	spgs.SystemPermissionGroupSetId [SPGS_SystemPermissionGroupSetId],
				upid.ClassifiedSegmentInstanceId
			from main.LogInSystemUser lsu 
				join Main.LogInSystemUserSystemAccess lsusa on lsu.LogInSystemUserId=lsusa.LogInSystemUserId
				join Main.UserPermission up on lsu.LogInSystemUserId=up.LogInSystemUserId
				Join Main.PermissionGroup pg on up.PermissionGroupId = pg.PermissionGroupId
				join Main.SystemPermissionGroupSetGrouping spgsg on up.PermissionGroupId = spgsg.PermissionGroupId
				join Main.SystemPermissionGroupSet spgs on spgsg.SystemPermissionGroupSetId=spgs.SystemPermissionGroupSetId and lsusa.SystemId=spgs.SystemId
				join Main.UserPermissionInstanceDetail upid on up.UserPermissionId=upid.UserPermissionId
			where lsu.LogInSystemUserId=@UserId and lsusa.SystemId=@SystemId";
    }
}
