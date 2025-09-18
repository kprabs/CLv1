namespace CoreLib.Application.Common.DBQueries
{
    public partial class SqlQueries
    {
        public const string ICPGetPackagesQuery = @"
			SELECT DISTINCT
			OT.SUPER_PRPKG_CD AS PackageId,
			OT.GRP_PRPKG_EFF_DT AS EffectiveDate,
			OT.GRP_PRPKG_EXP_DT AS TerminationDate,
			MIN(OT.SNPSHT_DT) AS SnapshotDate 
		FROM (SELECT DISTINCT	
				C.SUPER_PRPKG_CD,
				C.GRP_PRPKG_EFF_DT,
				C.GRP_PRPKG_EXP_DT,
				C.SNPSHT_DT
			FROM [dbo].[ICP_CUST] A
			JOIN
				(SELECT SRC_CUST_ID, MIN(SNPSHT_DT) SNPSHT_DT, DATEADD(month, 1, MIN(SNPSHT_DT)) SNPSHT_DT2 
					FROM [dbo].ICP_GRP G WHERE G.SRC_CUST_ID = '@ClientId' 
					AND G.SNPSHT_DT >= DATEADD(month, DATEDIFF(month, 0, GETDATE()), 0) 
					AND G.DATA_SRC_CD = 'FOS' GROUP BY G.SRC_CUST_ID) DT
				ON DT.SRC_CUST_ID = A.SRC_CUST_ID 
			LEFT JOIN  [dbo].[ICP_GRP] B 
				ON A.SRC_CUST_ID = B.SRC_CUST_ID 
					AND B.DATA_SRC_CD='FOS' 
					AND (DT.SNPSHT_DT = B.SNPSHT_DT OR DT.SNPSHT_DT2 = B.SNPSHT_DT)
			LEFT JOIN [dbo].[ICP_GRP_PRPKG_CVG] C  
				 ON C.SRC_CUST_ID =  DT.SRC_CUST_ID
				 AND C.DATA_SRC_CD = 'FOS' 
				 AND B.SRC_GRP_NO = C.SRC_GRP_NO 
				 AND (DT.SNPSHT_DT = C.SNPSHT_DT OR DT.SNPSHT_DT2 = C.SNPSHT_DT)
				 AND C.GRP_PRPKG_EXP_DT > C.GRP_PRPKG_EFF_DT	
			WHERE
				(A.SNPSHT_DT = CAST(DT.SNPSHT_DT AS datetime) OR A.SNPSHT_DT = CAST(DT.SNPSHT_DT2 AS datetime))
				AND A.SRC_CUST_ID =  DT.SRC_CUST_ID
				AND B.SRC_GRP_ORG_ID = @AccountId                                                                                                                                  
				AND B.SRC_GRP_NO = @SubAccountId
				AND GETDATE() <= C.GRP_PRPKG_EXP_DT
			) OT
		GROUP BY OT.SUPER_PRPKG_CD, OT.GRP_PRPKG_EFF_DT, OT.GRP_PRPKG_EXP_DT";
    }
}
