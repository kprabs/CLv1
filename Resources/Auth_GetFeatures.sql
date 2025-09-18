WITH cte AS (
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
	STUFF((select ','+  cast(spg.CustomLabelName as varchar) from Main.SystemPermissionGroupSetGrouping spg where spgs.SystemPermissionGroupSetId=spg.SystemPermissionGroupSetId FOR XML PATH('')),1,1,'') as CustomLabelName,
	STUFF((select ','+  cast(spg.LabelName as varchar) from Main.SystemPermissionGroupSetGrouping spg where spgs.SystemPermissionGroupSetId=spg.SystemPermissionGroupSetId FOR XML PATH('')),1,1,'') as LabelName,
	STUFF((select ','+  cast(spg.PermissionGroupId as varchar) from Main.SystemPermissionGroupSetGrouping spg where spgs.SystemPermissionGroupSetId=spg.SystemPermissionGroupSetId FOR XML PATH('')),1,1,'') as PermissionGroupId,
	STUFF((select ','+  cast(pg.PermissionGroupNKey as varchar) from Main.PermissionGroup pg where spg.PermissionGroupId=pg.PermissionGroupId FOR XML PATH('')),1,1,'') as PermissionGroupNKey,
	STUFF((select distinct ','+  cast(c.AreaCode as varchar) from cte c where spg.LowestAssignableClassifiedAreaSegmentId=c.ClassifiedAreaSegmentId FOR XML PATH('')),1,1,'') AreaCode,
	STUFF((select distinct ','+  cast(c.SegmentCode as varchar) from cte c where spg.LowestAssignableClassifiedAreaSegmentId=c.ClassifiedAreaSegmentId FOR XML PATH('')),1,1,'') as SegmentCode
from 
main.SystemPermissionGroupSet spgs 
join Main.SystemPermissionGroupSetGrouping spg on spgs.SystemPermissionGroupSetId=spg.SystemPermissionGroupSetId
where spgs.SystemId=@SystemId