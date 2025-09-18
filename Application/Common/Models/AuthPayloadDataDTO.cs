namespace CoreLib.Application.Common.Models
{
    public class AuthPayloadDataDto
    {
        public string clientId { get; set; }
        public List<string> accounts { get; set; } = [];
        public List<string> subAccounts { get; set; } = [];
        public bool onlyClientFilter { get; set; }
    }
}
