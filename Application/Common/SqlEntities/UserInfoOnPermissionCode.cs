namespace CoreLib.Application.Common.SqlEntities
{
    public class UserInfoOnPermissionCode
    {
        public string BrandId { get; set; }
        public string BrandName { get; set; }
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public string SubAccountId { get; set; }
        public string SubAccountName { get; set; }
        public string SegmentType { get; set; }
        public string PermissionCode { get; set; }

        public string FeatureId { get; set; }
        public string FeatureName { get; set; }
        public string SubFeatureId { get; set; }
        public string SubFeatureName { get; set; }
        public string HasAccess { get; set; }
    }
}
