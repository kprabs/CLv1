namespace CoreLib.Application.Common.DBQueries
{
    public partial class SqlQueries
    {
        public const string GetActiveTermsAndConditionsIdQuery = @"SELECT doc.DocumentInstanceId
                  FROM Main.DocumentInstance doc
                    JOIN Code.DocumentType dtype on doc.DocumentTypeId = dtype.DocumentTypeId
                  WHERE dtype.Code = 'GP_TCS' AND doc.ActiveFlag = 1
                    AND @date >= doc.DocumentEffDate AND @date < doc.DocumentTermDate";
    }
}
