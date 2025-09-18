namespace CoreLib.Application.Common.DBQueries
{
    public partial class SqlQueries
    {
        public const string ICPGetSubaccountDetailQuery = @"
			SELECT DISTINCT 
				OT.SRC_CUST_ID AS ClientId,
				OT.SRC_GRP_ORG_ID AS AccountId,
				OT.SRC_GRP_NO AS SubaccountId,
				OT.GRP_NM AS SubaccountName,
				OT.GRP_EFF_DT AS EffectiveDate,
				OT.GRP_EXP_DT AS TerminationDate,
				MIN(OT.SNPSHT_DT) AS SnapshotDate,
				CASE 
				WHEN OT.CUST_GRP_STS_TP_CD = 'A'
					THEN 'ACTIVE' 
				WHEN OT.CUST_GRP_STS_TP_CD = 'C' 
					THEN 'TERMINATED' 
				ELSE OT.CUST_GRP_STS_TP_CD 
				END AS Status
			FROM (SELECT DISTINCT	
					 A.SRC_CUST_ID,
					 B.SRC_GRP_ORG_ID,
					 B.SRC_GRP_NO,
					 B.GRP_NM,
					 B.GRP_EFF_DT,
					 B.GRP_EXP_DT,
					 B.SNPSHT_DT,
					 B.CUST_GRP_STS_TP_CD
				  FROM [dbo].[ICP_CUST] A
				  JOIN
						(SELECT SRC_CUST_ID, MIN(SNPSHT_DT) SNPSHT_DT, DATEADD(month, 1, MIN(SNPSHT_DT)) SNPSHT_DT2 
						 FROM [dbo].ICP_GRP G WHERE G.SRC_CUST_ID = '@ClientId' 
							AND G.SNPSHT_DT >= DATEADD(month, DATEDIFF(month, 0, GETDATE()), 0) 
							AND G.DATA_SRC_CD = 'FOS' GROUP BY G.SRC_CUST_ID) DT
					ON DT.SRC_CUST_ID = A.SRC_CUST_ID 
				  LEFT JOIN [dbo].[ICP_GRP] B 
					ON A.SRC_CUST_ID = B.SRC_CUST_ID 
					  AND B.DATA_SRC_CD='FOS' 
					  AND (DT.SNPSHT_DT = B.SNPSHT_DT OR DT.SNPSHT_DT2 = B.SNPSHT_DT)
				  LEFT JOIN [dbo].[ICP_GRP_PRPKG_CVG] C  
					ON C.SRC_CUST_ID =  DT.SRC_CUST_ID 
						AND C.DATA_SRC_CD = 'FOS' 
						AND B.SRC_GRP_NO = C.SRC_GRP_NO 
						AND (DT.SNPSHT_DT = B.SNPSHT_DT OR DT.SNPSHT_DT2 = B.SNPSHT_DT) 
				  WHERE
					(A.SNPSHT_DT = CAST(DT.SNPSHT_DT AS datetime) OR A.SNPSHT_DT = CAST(DT.SNPSHT_DT2 AS datetime))
					AND A.SRC_CUST_ID =  DT.SRC_CUST_ID 
					AND B.SRC_GRP_ORG_ID = @AccountId
					AND B.SRC_GRP_NO = @SubAccountId) OT
			GROUP BY OT.SRC_CUST_ID, OT.SRC_GRP_ORG_ID, OT.SRC_GRP_NO, OT.GRP_NM, OT.GRP_EFF_DT, OT.GRP_EXP_DT, OT.CUST_GRP_STS_TP_CD";
    }
}
