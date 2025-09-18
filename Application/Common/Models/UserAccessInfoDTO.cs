namespace CoreLib.Application.Common.Models
{
    public class UserAccessInfoDTO
    {
        public UserAccessInfoDTO()
        {
            Features = [];
            SelectedClientIds = [];
        }
        public bool AllClients { get; set; }
        public string CurrentVersion { get; set; }
        public IList<FeatureAccessInfoDTO> Features { get; set; }
        public IList<SelectedClientIdsDTO> SelectedClientIds { get; set; }
        public string UserName { get; set; }
        public string? RID { get; set; }
        public bool financialManager { get; set; }

    }
}