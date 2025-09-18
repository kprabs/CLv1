namespace CoreLib.Application.Common.DBQueries
{
    public partial class SqlQueries
    {
        public const string ShowTermsAndConditionsQuery = @"SELECT Top 1 udoc.AcceptanceFlag
                      FROM Main.UserDocumentAction udoc
                      WHERE udoc.ExternalSystemUserNkey = @userName AND udoc.DocumentInstanceId = @documentId
                      ORDER BY udoc.DecisionDate DESC";
    }
}
