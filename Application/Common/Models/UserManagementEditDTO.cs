namespace CoreLib.Application.Common.Models
{
    public class UserManagementEditDTO
    {
        // System
        public int? SystemId { get; set; }
        public string SystemCode { get; set; }

        public int UserId { get; set; }

        public int? OAMRoleId { get; set; }
        public string OAMRoleCode { get; set; }

        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TenantName { get; set; }
        public bool AllClients { get; set; }

        public IList<int> SelectedClientIds { get; set; }
        public IList<LookupEntryDTO<int, string>> SelectableClients { get; set; }

        public IList<UserManagementEditFeatureDTO> Features { get; set; }
        public TreeGridDTO<int> SelectedClientsTreeData { get; set; }
    }
}
