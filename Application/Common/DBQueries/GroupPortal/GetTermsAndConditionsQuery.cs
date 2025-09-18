namespace CoreLib.Application.Common.DBQueries
{
    public partial class SqlQueries
    {
        public const string GetTermsAndConditionsQuery = @"SELECT doc.DocumentInstanceId
                      ,doc.DocumentFilePath
                      ,doc.DocumentTypeId
                      ,doc.DocumentName
                      ,doc.ReceiptDate
                      ,doc.ActiveFlag
                      ,doc.LastUpdatedByUserNKey
                      ,doc.DocumentContent
                      ,doc.DocumentEffDate
                      ,doc.DocumentTermDate
                  FROM Main.DocumentInstance doc
                    JOIN Code.DocumentType dtype on doc.DocumentTypeId = dtype.DocumentTypeId
                  WHERE dtype.Code = 'GP_TCS' AND doc.ActiveFlag = 1
                    AND @date >= doc.DocumentEffDate AND @date < doc.DocumentTermDate";
    }
}
