namespace CoreLib.Application.Common.Models
{
    public class UserFeatureAccessPermissionDTO
    {
        public UserFeatureAccessPermissionDTO()
        {
            SelectedClientIds = [];
            Features = [];
            SelectedClientsTreeData = new();
        }
        public int SystemId { get; set; }
        public int UserId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OAMRoleCode { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool Active { get; set; }
        public string UserName { get; set; }
        public string TenantName { get; set; }
        public string SystemCode { get; set; }
        public bool AllClients { get; set; }
        public IList<ClientAccountSelection> IsAllAcountSubAccount { get; set; }
        public IList<int> SelectedClientIds { get; set; }
        public IList<LookupEntryDTO<int, string>> SelectableClients { get; set; }
        public IList<UserManagementEditFeatureDTO> Features { get; set; }
        public TreeGridDTO<int>? SelectedClientsTreeData { get; set; }
        public string? RID { get; set; }
        public Dictionary<string, List<AccountDto?>>? ICPClientInfo { get; set; }
        public bool financialManager { get; set; }

    }

    public class ClientAccountSelection
    {
        public string clientNkey { get; set; }
        public bool isAllAccountsSubAccountsSelected { get; set; }
    }
}
