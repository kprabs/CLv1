using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreLib.Entities
{
    [Table("UserDocumentAction", Schema = "Main")]
    public partial class UserDocumentAction
    {
        [Key]
        public int UserDocumentActionId { get; set; }
        [ForeignKey("DocumentInstanceId")]
        public int DocumentInstanceId { get; set; }
        public string ExternalSystemUserNkey { get; set; } = null!;
        public DateTime DecisionDate { get; set; }
        public bool? AcceptanceFlag { get; set; }
    }
}
