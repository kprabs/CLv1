namespace CoreLib.Application.Common.Models
{
    public class SelectedClientIdsDTO
    {
        public string? clientId { get; set; }
        public string? clientName { get; set; }
        public string? clientStatus { get; set; }
        public string? clientBrand { get; set; }
        public string? platformIndicator { get; set; }
        public List<AccountDto?>? icpClientInfo { get; set; }
    }
}
