namespace CoreLib.Application.Common.DBQueries
{
    public partial class SqlQueries
    {
        public const string GetClassifiedInstancesQuery = @"select
				csi.ClassifiedAreaSegmentNKey as [BrandId],
				csi.ClassifiedAreaSegmentName as [BrandName],
				csic.ClassifiedAreaSegmentNKey as [ClientId],
				csic.ClassifiedAreaSegmentName as [ClientName],
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
				else '' end
		
			from Main.ClassifiedSegmentInstance csic
			left JOIN Main.ClassifiedSegmentInstance csi ON csi.ClassifiedAreaSegmentNKey=csic.ParentClassifiedAreaSegmentNKey and csi.ClassifiedAreaSegmentId=4
			left JOIN Main.ClassifiedSegmentInstance csia ON csia.ParentClassifiedAreaSegmentNKey=csic.ClassifiedAreaSegmentNKey and csia.ClassifiedAreaSegmentId=2
			left JOIN Main.ClassifiedSegmentInstance csisa ON csisa.ParentClassifiedAreaSegmentNKey=csia.ClassifiedAreaSegmentNKey and csisa.ClassifiedAreaSegmentId=3
		where 
		csic.ClassifiedAreaSegmentNKey='@ClientIdNKey' and csic.ClassifiedAreaSegmentId=1";

    }
}
