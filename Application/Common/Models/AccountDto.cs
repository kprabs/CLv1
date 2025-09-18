using CoreLib.Application.Common.Utility;

namespace CoreLib.Application.Common.Models
{
    public class AccountDto
    {
        private string? accountId;
        private string? accountName;
        private string? accountPlatformName;


        public AccountDto()
        {
            Subaccounts = [];
        }
        public string? AccountId { get => accountId; set => accountId = value?.Trim() == "~" ? string.Empty : value?.Trim(); }
        public string? AccountName { get => accountName; set => accountName = IcpUtilities.ConvertDefaultDbNullValue(value); }
        public string? AccountPlatformName { get => accountPlatformName; set => accountPlatformName = value?.Trim() == "~" ? string.Empty : value; }

        public IList<SubaccountDto> Subaccounts { get; }
    }
}
