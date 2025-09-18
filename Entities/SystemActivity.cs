using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreLib.Entities
{
    [Table("SystemActivity", Schema = "Main")]
    public class SystemActivity
    {
        [Key]
        public int SystemActivityID { get; set; }

        public string SystemActivityName { get; set; }

        public string SystemActivityTypeNKey { get; set; }

        public bool IsActiveFlag { get; set; }

        // Navigation property for related APIAudit
        public virtual ICollection<APIAudit> APIAudits { get; set; } = [];
    }
}
