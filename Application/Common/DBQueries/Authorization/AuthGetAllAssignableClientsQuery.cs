namespace CoreLib.Application.Common.DBQueries
{
    public partial class SqlQueries
    {
        public const string AuthGetAllAssignableClientsQuery = @"
WITH recursiveClassfiedAreaSegment
AS (
	SELECT c.ClassifiedAreaSegmentNKey AS ChildNKey
		,c.ParentClassifiedAreaSegmentNKey AS AccountNKey
		,CAST(NULL AS NVARCHAR) AS ClientNKey
		,CAST(NULL AS NVARCHAR) AS TenantNKey
		,c.ClassifiedAreaSegmentName AS childAreaSegmentName
		,CAST(NULL AS NVARCHAR) AS accountAreaSegmentName
		,cast(NULL AS NVARCHAR) AS clientAreaSegmentName
		,cast(NULL AS NVARCHAR) AS tenantAreaSegmentName
		,c.ClassifiedSegmentInstanceId AS childSegmentId
		,NULL AS accountSegmentId
		,NULL AS clientSegmentID
		,NULL AS tenantSegemntID
		,c.ClassifiedAreaSegmentId AS childAreaSegmentId
		,NULL AS accountAreaSegmentId
		,NULL AS clientAreaSegmentID
		,NULL AS tenantAreaSegemntID
	FROM main.ClassifiedSegmentInstance c
		@JoinCondition
	WHERE c.ParentClassifiedAreaSegmentNKey = '@BrandName' @Condition
	
	UNION ALL
	
	SELECT c.ClassifiedAreaSegmentNKey AS ChildNKey
		,c.ParentClassifiedAreaSegmentNKey AS AccountNKey
		,CAST(r.AccountNKey AS NVARCHAR) AS ClientNKey
		,CAST(r.ClientNKey AS NVARCHAR) AS TenantNKey
		,c.ClassifiedAreaSegmentName AS childAreaSegmentName
		,CAST(r.childAreaSegmentName AS NVARCHAR) AS accountAreaSegmentName
		,CAST(r.accountAreaSegmentName AS NVARCHAR) AS clientAreaSegmentName
		,CAST(r.clientAreaSegmentName AS NVARCHAR) AS tenantAreaSegmentName
		,c.ClassifiedSegmentInstanceId AS childSegmentId
		,r.childSegmentId AS accountSegmentId
		,r.accountSegmentId AS clientSegmentID
		,r.childSegmentId AS tenantSegemntID
		,c.ClassifiedAreaSegmentId AS childAreaSegmentId
		,r.childAreaSegmentId AS accountAreaSegmentId
		,r.accountAreaSegmentId AS clientAreaSegmentId
		,r.clientAreaSegmentID AS tenantAreaSegmentID
	FROM main.ClassifiedSegmentInstance c
	INNER JOIN recursiveClassfiedAreaSegment r ON c.ParentClassifiedAreaSegmentNKey = r.ChildNKey
	)
SELECT (
		SELECT Tnnt.SegInstId
			,Tnnt.AreaSegNKey
			,Tnnt.AreaSegNm
			,Tnnt.AreaCd
			,Tnnt.AreaNm
			,Tnnt.SegCd
			,Tnnt.SegNm
			,Clnt.SegInstId
			,Clnt.AreaSegNKey
			,Clnt.AreaSegNm
			,Clnt.AreaCd
			,Clnt.AreaNm
			,Clnt.SegCd
			,Clnt.SegNm
			,Acct.SegInstId
			,Acct.AreaSegNKey
			,Acct.AreaSegNm
			,Acct.AreaCd
			,Acct.AreaNm
			,Acct.SegCd
			,Acct.SegNm
			,Chld.SegInstId
			,Chld.AreaSegNKey
			,Chld.AreaSegNm
			,Chld.AreaCd
			,Chld.AreaNm
			,Chld.SegCd
			,Chld.SegNm
			,Chld.AccountNKey
		FROM (
			SELECT DISTINCT tenant.tenantSegmentId 'SegInstId'
				,rcas.TenantNKey 'AreaSegNKey'
				,tenant.tenantAreaSegmentName 'AreaSegNm'
				,TenantClassifiedArea.Code 'AreaCd'
				,TenantClassifiedArea.Name 'AreaNm'
				,TenantClassifiedSegment.Code 'SegCd'
				,TenantClassifiedSegment.Name 'SegNm'
			FROM recursiveClassfiedAreaSegment rcas
			JOIN (
				SELECT lsgai.ClassifiedSegmentInstanceId tenantSegmentId
					,csi.ClassifiedAreaSegmentName tenantAreaSegmentName
					,csi.ClassifiedAreaSegmentNKey tenantNKey
					,csi.ClassifiedAreaSegmentId tenantAreaSegmentId
				FROM main.LogInSystemUser lsu
				JOIN main.LogInSystemUserSystemAccess lsusa ON lsu.LogInSystemUserId = lsusa.LogInSystemUserId
				JOIN Main.UserLogInSystemGroup ulsg ON lsu.LogInSystemUserId = ulsg.LogInSystemUserId
				JOIN main.LogInSystemGroup lsg ON ulsg.LogInSystemGroupId = lsg.LogInSystemGroupId
				JOIN main.LogInSystemGroupAccessibleInstance lsgai ON lsg.LogInSystemGroupId = lsgai.LogInSystemGroupId
				JOIN main.ClassifiedSegmentInstance csi ON lsgai.ClassifiedSegmentInstanceId = csi.ClassifiedSegmentInstanceId
				WHERE lsusa.SystemId = @SystemId
					AND lsu.LogInSystemUserId = @UserId
				) tenant ON tenant.tenantNKey = rcas.TenantNKey
			JOIN main.ClassifiedAreaSegment TenantClassifiedAreaSegment ON tenant.tenantAreaSegmentId = TenantClassifiedAreaSegment.ClassifiedAreaSegmentId
			JOIN code.ClassifiedArea TenantClassifiedArea ON TenantClassifiedAreaSegment.ClassifiedAreaId = TenantClassifiedArea.ClassifiedAreaId
			JOIN code.ClassifiedSegment TenantClassifiedSegment ON TenantClassifiedAreaSegment.ClassifiedSegmentId = TenantClassifiedSegment.ClassifiedSegmentId
			) Tnnt
		JOIN (
			SELECT DISTINCT rcas.clientSegmentID 'SegInstId'
				,rcas.ClientNKey 'AreaSegNKey'
				,rcas.clientAreaSegmentName 'AreaSegNm'
				,ClientClassifiedArea.Code 'AreaCd'
				,ClientClassifiedArea.Name 'AreaNm'
				,ClientClassifiedSegment.Code 'SegCd'
				,ClientClassifiedSegment.Name 'SegNm'
				,rcas.TenantNKey 'TenantNKey'
			FROM recursiveClassfiedAreaSegment rcas
			JOIN main.ClassifiedAreaSegment ClientClassifiedAreaSegment ON rcas.clientAreaSegmentID = ClientClassifiedAreaSegment.ClassifiedAreaSegmentId
			JOIN code.ClassifiedArea ClientClassifiedArea ON ClientClassifiedAreaSegment.ClassifiedAreaId = ClientClassifiedArea.ClassifiedAreaId
			JOIN code.ClassifiedSegment ClientClassifiedSegment ON ClientClassifiedAreaSegment.ClassifiedSegmentId = ClientClassifiedSegment.ClassifiedSegmentId
			) Clnt ON Clnt.TenantNKey = Tnnt.AreaSegNKey
		JOIN (
			SELECT DISTINCT rcas.accountSegmentId 'SegInstId'
				,rcas.AccountNKey 'AreaSegNKey'
				,rcas.accountAreaSegmentName 'AreaSegNm'
				,AccountClassifiedArea.Code 'AreaCd'
				,AccountClassifiedArea.Name 'AreaNm'
				,AccountClassifiedSegment.Code 'SegCd'
				,AccountClassifiedSegment.Name 'SegNm'
				,rcas.ClientNKey 'ClientNkey'
			FROM recursiveClassfiedAreaSegment rcas
			JOIN main.ClassifiedAreaSegment AccountClassifiedAreaSegment ON rcas.accountAreaSegmentId = AccountClassifiedAreaSegment.ClassifiedAreaSegmentId
			JOIN code.ClassifiedArea AccountClassifiedArea ON AccountClassifiedAreaSegment.ClassifiedAreaId = AccountClassifiedArea.ClassifiedAreaId
			JOIN code.ClassifiedSegment AccountClassifiedSegment ON AccountClassifiedAreaSegment.ClassifiedSegmentId = AccountClassifiedSegment.ClassifiedSegmentId
			) Acct ON Clnt.AreaSegNKey = Acct.ClientNkey
		JOIN (
			SELECT DISTINCT rcas.childSegmentId 'SegInstId'
				,rcas.ChildNKey 'AreaSegNKey'
				,rcas.childAreaSegmentName 'AreaSegNm'
				,ChildClassifiedArea.Code 'AreaCd'
				,ChildClassifiedArea.Name 'AreaNm'
				,ChildClassifiedSegment.Code 'SegCd'
				,ChildClassifiedSegment.Name 'SegNm'
				,rcas.AccountNKey 'AccountNKey'
			FROM recursiveClassfiedAreaSegment rcas
			JOIN main.ClassifiedAreaSegment ChildClassifiedAreaSegment ON rcas.childAreaSegmentId = ChildClassifiedAreaSegment.ClassifiedAreaSegmentId
			JOIN code.ClassifiedArea ChildClassifiedArea ON ChildClassifiedAreaSegment.ClassifiedAreaId = ChildClassifiedArea.ClassifiedAreaId
			JOIN code.ClassifiedSegment ChildClassifiedSegment ON ChildClassifiedAreaSegment.ClassifiedSegmentId = ChildClassifiedSegment.ClassifiedSegmentId
			) Chld ON Acct.AreaSegNKey = Chld.AccountNKey
		FOR JSON AUTO
		) JSON

";
    }
}
