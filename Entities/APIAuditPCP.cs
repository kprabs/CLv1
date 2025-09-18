using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreLib.Entities
{
    [Table("APIAuditPCP", Schema = "Main")]
    public partial class APIAuditPCP
    {
        [Key]
        public int APIAuditPCPID { get; set; }

        public int APIAuditID { get; set; }

        public Int64 MemberKey { get; set; }

        public string CurrentSupplierLocationNKey { get; set; }
        public int CurrentPCPKey { get; set; }
        public string CurrentPCPName { get; set; }

        public string NewSupplierLocationNKey { get; set; }
        public int NewPCPKey { get; set; }
        public string NewPCPName { get; set; }

        public DateTime EffDate { get; set; }

        public bool IsProcessedFlag { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // Navigation property for the relationship
        [ForeignKey("APIAuditID")]
        public virtual APIAudit APIAudit { get; set; }
    }
}
