namespace CoreLib.Application.Common.DBQueries
{
    public partial class SqlQueries
    {
         public const string AuthGetSelectedleClientsQuery = @"
select
(
	select
		Tnnt.SegmentInstanceId 'SegInstId',
		Tnnt.AreaSegmentNKey 'AreaSegNKey',
		Tnnt.AreaSegmentName 'AreaSegNm',
		Tnnt.AreaCode 'AreaCd',
		Tnnt.AreaName 'AreaNm',
		Tnnt.SegmentCode 'SegCd',
		Tnnt.SegmentName 'SegNm',

		Clnt.SegmentInstanceId 'SegInstId',
		Clnt.AreaSegmentNKey 'AreaSegNKey',
		Clnt.AreaSegmentName 'AreaSegNm',
		Clnt.AreaCode 'AreaCd',
		Clnt.AreaName 'AreaNm',
		Clnt.SegmentCode 'SegCd',
		Clnt.SegmentName 'SegNm',

		Acct.SegmentInstanceId 'SegInstId',
		Acct.AreaSegmentNKey 'AreaSegNKey',
		Acct.AreaSegmentName 'AreaSegNm',
		Acct.AreaCode 'AreaCd',
		Acct.AreaName 'AreaNm',
		Acct.SegmentCode 'SegCd',
		Acct.SegmentName 'SegNm',

		Chld.SegmentInstanceId 'SegInstId',
		Chld.AreaSegmentNKey 'AreaSegNKey',
		Chld.AreaSegmentName 'AreaSegNm',
		Chld.AreaCode 'AreaCd',
		Chld.AreaName 'AreaNm',
		Chld.SegmentCode 'SegCd',
		Chld.SegmentName 'SegNm'

	from 
	(
		select
			Tenant.ClassifiedSegmentInstanceId SegmentInstanceId, 
			Tenant.ClassifiedAreaSegmentNKey AreaSegmentNKey,
			Tenant.ClassifiedAreaSegmentName AreaSegmentName,
			TenantClassifiedArea.Code as AreaCode,
			TenantClassifiedArea.[Name] as AreaName,
			TenantClassifiedSegment.[Code] as SegmentCode,
			TenantClassifiedSegment.[Name] as SegmentName
		from
			Main.ClassifiedSegmentInstance Tenant 
			join main.ClassifiedAreaSegment TenantClassifiedAreaSegment on Tenant.ClassifiedAreaSegmentId = TenantClassifiedAreaSegment.ClassifiedAreaSegmentId
			join code.ClassifiedArea TenantClassifiedArea on TenantClassifiedAreaSegment.ClassifiedAreaId=TenantClassifiedArea.ClassifiedAreaId
			join code.ClassifiedSegment TenantClassifiedSegment on TenantClassifiedAreaSegment.ClassifiedSegmentId=TenantClassifiedSegment.ClassifiedSegmentId
		where Tenant.ClassifiedSegmentInstanceId in
		(
			select lsgai.ClassifiedSegmentInstanceId
			from main.LogInSystemUser lsu
			join main.LogInSystemUserSystemAccess lsusa on lsu.LogInSystemUserId=lsusa.LogInSystemUserId
			join Main.UserLogInSystemGroup ulsg on lsu.LogInSystemUserId=ulsg.LogInSystemUserId
			join main.LogInSystemGroup lsg on ulsg.LogInSystemGroupId=lsg.LogInSystemGroupId
			join main.LogInSystemGroupAccessibleInstance lsgai on lsg.LogInSystemGroupId = lsgai.LogInSystemGroupId
			where 
				lsusa.SystemId = @SystemId and lsu.LogInSystemUserId = @UserId 
		)
		and Tenant.ClassifiedAreaSegmentNKey = '@BrandName'
	) Tnnt
	join
	(
		select
			Client.ParentClassifiedAreaSegmentNKey ParentAreaSegmentNKey,
			Client.ClassifiedSegmentInstanceId SegmentInstanceId,
			Client.ClassifiedAreaSegmentNKey AreaSegmentNKey,
			Client.ClassifiedAreaSegmentName AreaSegmentName,
			ClientClassifiedArea.Code as AreaCode,
			ClientClassifiedArea.[Name] as AreaName,
			ClientClassifiedSegment.[Code] as SegmentCode,
			ClientClassifiedSegment.[Name] as SegmentName
		from		
			main.ClassifiedSegmentInstance Client
			join main.ClassifiedAreaSegment ClientClassifiedAreaSegment on Client.ClassifiedAreaSegmentId = ClientClassifiedAreaSegment.ClassifiedAreaSegmentId
			join code.ClassifiedArea ClientClassifiedArea on ClientClassifiedAreaSegment.ClassifiedAreaId=ClientClassifiedArea.ClassifiedAreaId
			join code.ClassifiedSegment ClientClassifiedSegment on ClientClassifiedAreaSegment.ClassifiedSegmentId=ClientClassifiedSegment.ClassifiedSegmentId
	) Clnt
		on Tnnt.AreaSegmentNKey = Clnt.ParentAreaSegmentNKey 
	join 
	(
		select 
			Account.ParentClassifiedAreaSegmentNKey ParentAreaSegmentNKey,
			Account.ClassifiedSegmentInstanceId SegmentInstanceId,
			Account.ClassifiedAreaSegmentNKey AreaSegmentNKey,
			Account.ClassifiedAreaSegmentName AreaSegmentName,
			AccountClassifiedArea.Code as AreaCode,
			AccountClassifiedArea.[Name] as AreaName,
			AccountClassifiedSegment.[Code] as SegmentCode,
			AccountClassifiedSegment.[Name] as SegmentName
		from
			main.ClassifiedSegmentInstance Account 
			join main.ClassifiedAreaSegment AccountClassifiedAreaSegment on Account.ClassifiedAreaSegmentId = AccountClassifiedAreaSegment.ClassifiedAreaSegmentId
			join code.ClassifiedArea AccountClassifiedArea on AccountClassifiedAreaSegment.ClassifiedAreaId=AccountClassifiedArea.ClassifiedAreaId
			join code.ClassifiedSegment AccountClassifiedSegment on AccountClassifiedAreaSegment.ClassifiedSegmentId=AccountClassifiedSegment.ClassifiedSegmentId
	) Acct
		on Clnt.AreaSegmentNKey = Acct.ParentAreaSegmentNKey 
	join
	(
		select
			Child.ParentClassifiedAreaSegmentNKey ParentAreaSegmentNKey,
			Child.ClassifiedSegmentInstanceId SegmentInstanceId,
			Child.ClassifiedAreaSegmentNKey AreaSegmentNKey,
			Child.ClassifiedAreaSegmentName AreaSegmentName,
			ChildClassifiedArea.Code as AreaCode,
			ChildClassifiedArea.[Name] as AreaName,
			ChildClassifiedSegment.[Code] as SegmentCode,
			ChildClassifiedSegment.[Name] as SegmentName
		from
			main.ClassifiedSegmentInstance Child 
			join main.ClassifiedAreaSegment ChildClassifiedAreaSegment on Child.ClassifiedAreaSegmentId = ChildClassifiedAreaSegment.ClassifiedAreaSegmentId
			join code.ClassifiedArea ChildClassifiedArea on ChildClassifiedAreaSegment.ClassifiedAreaId=ChildClassifiedArea.ClassifiedAreaId
			join code.ClassifiedSegment ChildClassifiedSegment on ChildClassifiedAreaSegment.ClassifiedSegmentId=ChildClassifiedSegment.ClassifiedSegmentId
	) Chld
		on Acct.AreaSegmentNKey = Chld.ParentAreaSegmentNKey
		@Condition
	for json auto
) json
";
    }
}
