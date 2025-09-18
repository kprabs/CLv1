namespace CoreLib.Application.Common.SqlEntities
{
    public class AccountDetailEntity
    {
        public string? ClientId { get; set; }
        public string? AccountId { get; set; }
        public string? AccountName { get; set; }
        public string? EffectiveDate { get; set; }
        public string? TerminationDate { get; set; }
        public string? SubaccountId { get; set; }
        public string? SubaccountName { get; set; }
        public string? SnapshotDate { get; set; }
    }
}
