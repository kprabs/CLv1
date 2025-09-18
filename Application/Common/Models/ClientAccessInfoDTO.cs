namespace CoreLib.Application.Common.Models
{
    public class ClientAccessInfoDTO
    {
        public ClientAccessInfoDTO()
        {
            Accounts = [];
        }
        public string ClientId { get; set; }
        public IList<AccountAccessInfoDTO> Accounts { get; set; }
    }
}
