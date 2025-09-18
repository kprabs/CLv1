namespace CoreLib.Application.Common.Models
{
    public class AccountAccessInfoDTO
    {
        public AccountAccessInfoDTO()
        {
            SubAccounts = [];
        }
        public string AccountId { get; set; }
        public IList<SubAccountAccessInfoDTO> SubAccounts { get; set; }
    }
}
