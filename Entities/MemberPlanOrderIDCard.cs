using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreLib.Entities
{
    [Table("MemberPlanOrderIDCard", Schema = "Main")]
    public class MemberPlanOrderIDCard
    {
        [Key]
        public int MemberPlanOrderIDCardID { get; set; }

        public int APIAuditID { get; set; } // Foreign key to APIAudit

        public Int64 MemberKey { get; set; }

        public int PlanKey { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime EffDate { get; set; }
    }
}
