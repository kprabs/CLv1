namespace CoreLib.Application.Common.DBQueries
{
    public partial class SqlQueries
    {
        public const string ICPGetProductsQuery = @"
			SELECT DISTINCT
				OT.PRPKG_NM AS ProductName,
				OT.BRAND_PROD_LOB_DSC AS LineOfBusiness,
				OT.RNEW_MO_NO AS RenewalMonth,
				OT.CVG_CTG_CD AS CoverageCategory,
				MIN(OT.SNPSHT_DT) AS SnapshotDate 
			FROM (SELECT DISTINCT	
					CASE 
						WHEN (C.CVG_CTG_CD <> 'OT' AND BP.PLAN_MKT_NM IS NOT NULL) THEN BP.PLAN_MKT_NM  
						ELSE C.PRPKG_NM END 
					AS PRPKG_NM,
					C.BRAND_PROD_LOB_DSC,
					C.RNEW_MO_NO,
					CASE 
						WHEN C.CVG_CTG_CD = 'ME'  THEN 'Medical'
						WHEN C.CVG_CTG_CD = 'RX'  THEN 'Prescription Drug'
						WHEN C.CVG_CTG_CD = 'DE'  THEN 'Dental'
						WHEN C.CVG_CTG_CD = 'VI'  THEN 'Vision'
						WHEN C.CVG_CTG_CD = 'SP'  THEN 'Spending Account'
						WHEN C.CVG_CTG_CD = 'OT'  THEN 'Other'
						ELSE 'Other' END 
					AS CVG_CTG_CD,
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
				LEFT JOIN
				   (SELECT INFOCUS_ID, PLAN_MKT_NM 
					FROM [dbo].[ICP_BENE_PLAN] IBP 
					WHERE IBP.PLAN_STS_CD IN ('Production', 'Archived') 
						AND PLAN_MKT_NM IS NOT NULL AND PLAN_MKT_NM <> '~') BP
					ON BP.INFOCUS_ID = C.INFOCUS_ID 
						AND C.CVG_CTG_CD <> 'OT' 
				WHERE
					(A.SNPSHT_DT = CAST(DT.SNPSHT_DT AS datetime) OR A.SNPSHT_DT = CAST(DT.SNPSHT_DT2 AS datetime))
					AND A.SRC_CUST_ID =  DT.SRC_CUST_ID
					AND B.SRC_GRP_ORG_ID = @AccountId                                                                                                                                  
					AND B.SRC_GRP_NO = @SubAccountId
				) OT
			GROUP BY OT.PRPKG_NM, OT.BRAND_PROD_LOB_DSC, OT.RNEW_MO_NO, OT.CVG_CTG_CD
			HAVING OT.PRPKG_NM IS NOT NULL";
    }
}
