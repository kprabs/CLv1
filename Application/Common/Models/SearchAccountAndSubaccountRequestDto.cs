namespace CoreLib.Application.Common.Models
{
    public class SearchAccountAndSubaccountRequestDto
    {
        public string? ClientId { get; set; }
        public string? AccountId { get; set; }
        public string? SubaccountId { get; set; }
        public DateTime? SubaccountEffDate { get; set; }
        public DateTime? SubaccountTermDate { get; set; }
    }
}
