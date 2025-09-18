namespace CoreLib.Application.Common.DBQueries
{
    public partial class SqlQueries
    {
        public const string UserInfoOnPermissionCodeQuery = @"select
				csi.ClassifiedAreaSegmentNKey as [BrandId],
				csi.ClassifiedAreaSegmentName as [BrandName],
				csic.ClassifiedAreaSegmentNKey as [ClientId],
				csic.ClassifiedAreaSegmentName as [ClientName],
				--csia.ClassifiedAreaSegmentNKey as AccountId,
				[AccountId] = case
				when csisa.ClassifiedAreaSegmentId = 3 then csisa.ParentClassifiedAreaSegmentNKey
				when csia.ClassifiedAreaSegmentId = 2 then csia.ClassifiedAreaSegmentNKey
				else '0' end,
				[AccountName] = case
				when csisa.ClassifiedAreaSegmentId = 3 then (select top 1 ClassifiedAreaSegmentName from Main.ClassifiedSegmentInstance where ClassifiedAreaSegmentNKey=csisa.ParentClassifiedAreaSegmentNKey and ClassifiedAreaSegmentId=2)
				when csia.ClassifiedAreaSegmentId = 2 then csia.ClassifiedAreaSegmentName
				else '' end,
				[SubAccountId] = case
				when csisa.ClassifiedAreaSegmentId=3 then csisa.ClassifiedAreaSegmentNKey
				else '0' end,
				[SubAccountName] = case
				when csisa.ClassifiedAreaSegmentId=3 then csisa.ClassifiedAreaSegmentName
				else '' end,
				--csia.ClassifiedAreaSegmentNKey as [SegmentNKey],
				--csia.ClassifiedAreaSegmentName as [SegmentName],
				--csic.ClassifiedAreaSegmentId as [ClientAreaSegmentId],
				--csia.ClassifiedAreaSegmentId as [SegmentId],
				[SegmentType] = case
							when csic.ClassifiedAreaSegmentId=1 and ISNULL(csia.ClassifiedAreaSegmentId,0)=0 and ISNULL(csisa.ClassifiedAreaSegmentId,0)=0 then 'Client'
							when csia.ClassifiedAreaSegmentId=2 then 'Account'
							when csisa.ClassifiedAreaSegmentId=3 then 'SubAccount'
							else 'Invalid' end,
				--csia.ParentClassifiedAreaSegmentNKey as [ParentNKey],
				pg.PermissionGroupNKey as [PermissionCode],
				spgs.SystemPermissionGroupSetId as [FeatureId],
				spgs.DisplayName as [FeatureName],
				spgsg.SystemPermissionGroupSetGroupingId as [SubFeatureId],
				spgsg.CustomLabelName as [SubFeatureName],
				[HasAccess] = Case 
						WHEN up.CustomAccessFlag=0 THEN 'false' 
						WHEN up.CustomAccessFlag=1 THEN 'true' 
						ELSE 'Invalid' 
					END	
		from Main.UserPermission up -- User Permission
			JOIN Main.LogInSystemUser lsu ON up.LogInSystemUserId=lsu.LogInSystemUserId
			JOIN Main.PermissionGroup pg ON up.PermissionGroupId=pg.PermissionGroupId
			JOIN Main.LogInSystemUserSystemAccessibleInstance lsusai ON up.LogInSystemUserId =lsusai.LogInSystemUserId
			JOIN Main.UserPermissionInstanceDetail upid ON up.UserPermissionId=upid.UserPermissionId
			JOIN Main.SystemPermissionGroupSetGrouping spgsg ON pg.PermissionGroupId=spgsg.PermissionGroupId
			JOIN Main.SystemPermissionGroupSet spgs ON spgsg.SystemPermissionGroupSetId = spgs.SystemPermissionGroupSetId 
			JOIN Main.LogInSystemUserSystemAccess  lsusa ON up.LogInSystemUserId=lsusa.LogInSystemUserId and lsusa.SystemId=spgs.SystemId
			JOIN Main.UserLogInSystemGroup ulsg ON up.LogInSystemUserId=ulsg.LogInSystemUserId
			JOIN Code.[System] [sys] ON lsusa.SystemId=[sys].SystemId
			JOIN Main.LogInSystemGroup lsg ON ulsg.LogInSystemGroupId=lsg.LogInSystemGroupId
			JOIN Main.LogInSystemGroupAccessibleInstance lsgai ON lsg.LogInSystemGroupId=lsgai.LogInSystemGroupId 
			left JOIN Main.ClassifiedSegmentInstance csic ON lsusai.ClassifiedSegmentInstanceId=csic.ClassifiedSegmentInstanceId and csic.ClassifiedAreaSegmentId=1
			left JOIN Main.ClassifiedSegmentInstance csi ON lsgai.ClassifiedSegmentInstanceId=csi.ClassifiedSegmentInstanceId and csi.ClassifiedAreaSegmentId=4
			left JOIN Main.ClassifiedSegmentInstance csia ON upid.ClassifiedSegmentInstanceId=csia.ClassifiedSegmentInstanceId and csia.ClassifiedAreaSegmentId=2
			left JOIN Main.ClassifiedSegmentInstance csisa ON upid.ClassifiedSegmentInstanceId=csisa.ClassifiedSegmentInstanceId and csisa.ClassifiedAreaSegmentId=3
		where lsu.SourceLogInSystemUserName = '@UserName' and spgs.SystemId=@SystemId and pg.PermissionGroupNKey= '@PermissionCode' and lsusai.SystemId=@SystemId
		ORDER BY up.LogInSystemUserId;";

    }
}
