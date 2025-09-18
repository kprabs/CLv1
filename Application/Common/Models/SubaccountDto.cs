namespace CoreLib.Application.Common.Models
{
    public class SubaccountDto
    {
        private string? subaccountName;
        private string? subaccountId;
        private string? platformName;

        public string? SubaccountName { get => subaccountName; set => subaccountName = value?.Trim() == "~" ? string.Empty : value; }
        public string? SubaccountId { get => subaccountId; set => subaccountId = value?.Trim() == "~" ? string.Empty : value?.Trim(); }
        public string? PlatformName { get => platformName; set => platformName = value?.Trim() == "~" ? string.Empty : value; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? TerminationDate { get; set; }
    }
}
