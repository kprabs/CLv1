using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreLib.Entities
{
    [Table("APIAuditPayload", Schema = "Main")]
    public partial class APIAuditPayload
    {
        [Key]
        public int APIAuditPayloadID { get; set; }

        public int APIAuditID { get; set; }

        public string APIEndPointDetail { get; set; }

        public string RequestPayloadHTML { get; set; }
        public string ResponseResultHTML { get; set; }

        public string ResponseCode { get; set; }

        // Navigation property for the relationship
        [ForeignKey("APIAuditID")]
        public virtual APIAudit APIAudit { get; set; }
    }
}
