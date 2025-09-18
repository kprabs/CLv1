using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreLib.Entities
{
    [Table("APIAudit", Schema = "Main")]
    public class APIAudit
    {
        [Key]
        public int APIAuditID { get; set; }

        public string SessionNKey { get; set; }

        public int SystemActivityID { get; set; } // Foreign key to SystemActivity
        public int ParentSystemActivityID { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreateUserNKey { get; set; }

        public string CreateForUserNKey { get; set; }

        public Int64? ClientKey { get; set; }

        public Int64? AccountKey { get; set; }

        public Int64? SubAccountKey { get; set; }

        public Int64? MemberKey { get; set; }

        public Int64? SubscriberKey { get; set; }

        public string MemberPlatformCode { get; set; }

        // Navigation properties
        [ForeignKey("SystemActivityID")]
        public virtual SystemActivity SystemActivity { get; set; }

        public virtual ICollection<APIAuditPayload> APIAuditPayloads { get; set; } = [];

        public virtual ICollection<APIAuditPCP> APIAuditPCPs { get; set; } = [];

        public virtual ICollection<MemberPlanOrderIDCard> MemberPlanOrderIDCards { get; set; } = [];

    }
}
