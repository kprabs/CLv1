namespace CoreLib.Application.Common.SqlEntities
{
    public class AccountSubaccountEntity
    {
        public string? ClientId { get; set; }
        public string? AccountId { get; set; }
        public string? AccountName { get; set; }
        public string? SubaccountId { get; set; }
        public string? SubaccountName { get; set; }
        public string? SubaccountEffectiveDate { get; set; }
        public string? SubaccountTerminationDate { get; set; }
        public string? SubaccountPlatformName { get; set; }
        public string? SnapshotDate { get; set; }
    }
}
