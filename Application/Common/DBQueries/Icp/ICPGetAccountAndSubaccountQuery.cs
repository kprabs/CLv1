namespace CoreLib.Application.Common.DBQueries
{
    public partial class SqlQueries
    {
        public const string ICPGetAccountAndSubaccountQuery = @"
			Declare @thisMonthStart Date = DATEADD(month, DATEDIFF(month, 0, GETDATE()), 0);

WITH SNAPSHOTCTE AS(
SELECT SRC_CUST_ID, 
		MIN(SNPSHT_DT) SNPSHT_DT, 
		DATEADD(month, 1, MIN(SNPSHT_DT)) SNPSHT_DT2, 
		G.DATA_SRC_CD
	FROM [dbo].ICP_GRP G 
	WHERE G.SRC_CUST_ID = '@ClientId' 
		AND G.SNPSHT_DT >= @thisMonthStart
		AND G.DATA_SRC_CD = 'FOS'
	GROUP BY G.SRC_CUST_ID, G.DATA_SRC_CD
),
--Filter ICP_GRP Early(only recent Snapshots)
Filtered_GRP AS (
	SELECT * 
	FROM dbo.ICP_GRP 
	WHERE SNPSHT_DT >= @thisMonthStart
),

-- Filter ICP_GRP_PRPKG early (limit by platform and snapshot)
Filtered_GRP_PRPKG AS (
	SELECT * 
	FROM dbo.ICP_GRP_PRPKG_CVG
	WHERE ENRL_PLATFORM_CD IN ('CoreSF','CoreFI')
)

SELECT DISTINCT 
	OT.SRC_CUST_ID AS ClientId,
	OT.SRC_GRP_ORG_ID AS AccountId,
	OT.PARNT_GRP_NM AS AccountName,
	OT.SRC_GRP_NO AS SubaccountId,
	OT.GRP_NM AS SubaccountName,
	OT.GRP_EFF_DT AS SubaccountEffectiveDate,
	OT.GRP_EXP_DT AS SubaccountTerminationDate,
	CASE 
		WHEN OT.ENRL_PLATFORM_NM='CoreSF' THEN 'CSF'
		WHEN OT.ENRL_PLATFORM_NM='CoreFI' THEN 'CFI'
	END AS SubaccountPlatformName,
	MIN(OT.SNPSHT_DT) AS SnapshotDate
	FROM (SELECT DISTINCT	 
		A.SRC_CUST_ID,
		B.SRC_GRP_ORG_ID,
		B.PARNT_GRP_NM,
		B.SRC_GRP_NO ,
		B.GRP_NM,
		B.GRP_EFF_DT,
		B.GRP_EXP_DT,
		C.ENRL_PLATFORM_NM,
		B.SNPSHT_DT
		FROM [dbo].[ICP_CUST] A
			JOIN SNAPSHOTCTE DT
				ON DT.SRC_CUST_ID = A.SRC_CUST_ID 
			LEFT JOIN Filtered_GRP B
				ON A.SRC_CUST_ID = B.SRC_CUST_ID 
				AND B.DATA_SRC_CD = DT.DATA_SRC_CD
				AND B.SNPSHT_DT = DT.SNPSHT_DT

			LEFT JOIN Filtered_GRP_PRPKG C
				ON C.SRC_CUST_ID = B.SRC_CUST_ID
				AND C.DATA_SRC_CD = B.DATA_SRC_CD
				AND C.SRC_GRP_NO = B.SRC_GRP_NO
				AND C.SNPSHT_DT = B.SNPSHT_DT
						
		WHERE (A.SNPSHT_DT = DT.SNPSHT_DT OR A.SNPSHT_DT = DT.SNPSHT_DT2)
		AND A.SRC_CUST_ID =  DT.SRC_CUST_ID
		@AccountId
		@SubAccountId
		@SubAccountEffectiveDt
		@SubAccountTerminationDt
		) OT
	GROUP BY OT.SRC_CUST_ID, 
			OT.SRC_GRP_ORG_ID, 
			OT.PARNT_GRP_NM, 
			OT.SRC_GRP_NO, 
			OT.GRP_NM, 
			OT.GRP_EFF_DT,
			OT.GRP_EXP_DT, 
			OT.ENRL_PLATFORM_NM
";
    }
}
