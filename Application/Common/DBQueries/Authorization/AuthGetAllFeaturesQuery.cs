namespace CoreLib.Application.Common.DBQueries
{
    public partial class SqlQueries
    {
        public const string AuthGetAllFeaturesQuery = @"WITH cte AS (
			  SELECT
				ca.Code as AreaCode,
				cs.Code as SegmentCode,
				ClassifiedAreaSegmentId
			  FROM main.ClassifiedAreaSegment cas 
				join code.ClassifiedArea ca on cas.ClassifiedAreaId=ca.ClassifiedAreaId
				join code.ClassifiedSegment cs on cas.ClassifiedSegmentId = cs.ClassifiedSegmentId
  
			  UNION ALL
 
			  SELECT
			   rc.AreaCode,
			   rc.SegmentCode,
				t.ClassifiedAreaSegmentId
			  FROM cte rc
			  INNER JOIN main.ClassifiedAreaSegment t
				ON t.ParentClassifiedAreaSegmentId=rc.ClassifiedAreaSegmentId
				join code.ClassifiedArea ca on t.ClassifiedAreaId=ca.ClassifiedAreaId
				join code.ClassifiedSegment cs on t.ClassifiedSegmentId = cs.ClassifiedSegmentId
			WHERE t.ParentClassifiedAreaSegmentId is not null)

			select distinct spgs.DisplayName, spgs.DisplayOrderNum, spgs.SystemPermissionGroupSetId,spgs.SystemId,
				STUFF((select ','+  cast(spg.CustomLabelName as varchar) from Main.SystemPermissionGroupSetGrouping spg 
					where spgs.SystemPermissionGroupSetId=spg.SystemPermissionGroupSetId FOR XML PATH('')),1,1,'') as CustomLabelName,
				STUFF((select ','+  cast(spg.LabelName as varchar) from Main.SystemPermissionGroupSetGrouping spg 
					where spgs.SystemPermissionGroupSetId=spg.SystemPermissionGroupSetId FOR XML PATH('')),1,1,'') as LabelName,
				STUFF((select '***'+  concat (
												cast(spg.DisplayOrderNum as varchar),',',
												cast(spg.LabelName as varchar),',',
												cast(spg.CustomLabelName as varchar),',',
												cast(spg.SystemPermissionGroupSetGroupingId as varchar),',',
												cast(pg.PermissionGroupNKey as varchar)
											) from Main.SystemPermissionGroupSetGrouping spg 
					join Main.PermissionGroup pg on spg.PermissionGroupId=pg.PermissionGroupId 
						where spgs.SystemPermissionGroupSetId=spg.SystemPermissionGroupSetId FOR XML PATH('')),1,3,'') as FeatureSelection,
				STUFF((select distinct ','+  cast(c.SegmentCode as varchar) from cte c 
					where spg.LowestAssignableClassifiedAreaSegmentId=c.ClassifiedAreaSegmentId FOR XML PATH('')),1,1,'') as SegmentCode
				
				
				

			from 
			main.SystemPermissionGroupSet spgs 
			join Main.SystemPermissionGroupSetGrouping spg on spgs.SystemPermissionGroupSetId=spg.SystemPermissionGroupSetId
			where spgs.SystemId=@SystemId";
    }
}
