namespace CoreLib.Application.Common.DBQueries
{
    public partial class SqlQueries
    {
        public const string ICPGetClientDetailQuery = @"
			SELECT DISTINCT
				OT.SRC_CUST_ID AS ClientId,
				OT.CUST_NM AS ClientName,
				--OT.NAICS_CD AS NaicsCode,
				--OT.NAICS_DSC AS NaicsDescription,
				OT.CUST_EFF_DT AS EffectiveDate,
				OT.CUST_EXP_DT AS TerminationDate,
				OT.SALES_MKT_REP_FULL_NM AS SalesRepresentative,
				CASE 
					WHEN OT.CUST_STS_CD = 'R' OR OT.CUST_STS_CD = 'S' 
						THEN 'ACTIVE' 
					WHEN OT.CUST_STS_CD = 'C' 
						THEN 'TERMINATED' 
					ELSE OT.CUST_STS_CD 
				END AS Status,
				MIN(OT.SNPSHT_DT) AS SnapshotDate,
				OT.EIN_ID AS TaxIdentificationNumber,
				OT.CUST_MBR_CNT AS ApproximateEmployeeCount
			FROM (SELECT DISTINCT	
					 A.SRC_CUST_ID,
					 A.CUST_NM,
					 --A.NAICS_CD,
					 --A.NAICS_DSC,
					 A.CUST_EFF_DT,
					 A.CUST_EXP_DT,
					 A.SALES_MKT_REP_FULL_NM,
					 A.CUST_STS_CD,
					 A.SNPSHT_DT,
					 A.EIN_ID,
					 A.CUST_MBR_CNT
				FROM [dbo].[ICP_CUST] A
				JOIN
					(SELECT SRC_CUST_ID, MIN(SNPSHT_DT) SNPSHT_DT,DATEADD(month, 1, MIN(SNPSHT_DT)) SNPSHT_DT2 
						FROM [dbo].ICP_GRP G WHERE G.SRC_CUST_ID = '@ClientId' 
						AND G.SNPSHT_DT >= DATEADD(month, DATEDIFF(month, 0, GETDATE()), 0) 
						AND G.DATA_SRC_CD = 'FOS' GROUP BY G.SRC_CUST_ID) DT
					ON DT.SRC_CUST_ID = A.SRC_CUST_ID  
				WHERE
						(A.SNPSHT_DT = CAST(DT.SNPSHT_DT AS datetime) OR A.SNPSHT_DT = CAST(DT.SNPSHT_DT2 AS datetime))
						AND A.SRC_CUST_ID =  DT.SRC_CUST_ID) OT
			GROUP BY OT.SRC_CUST_ID, OT.CUST_NM, --OT.NAICS_CD, OT.NAICS_DSC, 
					 OT.CUST_EFF_DT, OT.CUST_EXP_DT,
					 OT.SALES_MKT_REP_FULL_NM, OT.CUST_STS_CD, OT.EIN_ID, OT.CUST_MBR_CNT";
    }
}
