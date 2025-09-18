namespace CoreLib.Application.Common.Models
{
    public class SubFeatureAccessInfoDTO
    {
        public SubFeatureAccessInfoDTO()
        {
            Clients = [];
        }
        public int? SubFeatureId { get; set; }
        public string? SubFeatureName { get; set; }
        public string? PermissionCode { get; set; }
        public bool? HasAccess { get; set; }
        public List<ClientHasAccess>? ClientHasAccess { get; set; }
        public string? Version { get; set; }
        public bool IsPHI { get; set; } = false;
        public IList<ClientAccessInfoDTO> Clients { get; set; }
    }
    public class ClientHasAccess
    {
        public string clientId {  get; set; }
        public bool? HasAccess { get; set; }
    }
}
