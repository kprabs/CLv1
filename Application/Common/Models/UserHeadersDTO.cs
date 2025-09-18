namespace CoreLib.Application.Common.Models
{
    public class UserHeadersDTO
    {
        public string BrandOrgID { get; set; }
        public string BrandName { get; set; }
        public string UserRole { get; set; }
        public string UserName { get; set; }
        public string Cookies { get; set; }
        public string SSOToken { get; set; }
        public string FirtName { get; set; }
        public string LastName { get; set; }
        public string LoginTimeStamp { get; set; }
        public string MVPUserName { get; set; }
        public string MVPRole { get; set; }
        public string MVPLoginId { get; set; }
        public string BrokerClientId { get; set; }
        public bool? IsMbrAvailable { get; set; }
        public string AuthSecret { get; set; }
        public string hmkfederatedas { get; set;}
        public string ibcgroup { get; set; }

        public string UserEmail { get; set; }

        public bool? IsIndRptAvailable { get; set; }

        public string Brandorgname { get; set; }

        public bool? IsStlsRptAvailable { get; set; }

        public string AllUserRole { get; set; }

    }
}
