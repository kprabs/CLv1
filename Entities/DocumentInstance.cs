using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreLib.Entities
{
    [Table("DocumentInstance", Schema = "Main")]
    public partial class DocumentInstance
    {
        [Key]
        public int DocumentInstanceId { get; set; }
        public string DocumentFilePath { get; set; } = null!;
        public int DocumentTypeId { get; set; }
        public string DocumentName { get; set; } = null!;
        public DateTime ReceiptDate { get; set; }
        public bool? ActiveFlag { get; set; }
        public string? LastUpdatedByUserNkey { get; set; }
        public string? DocumentContent { get; set; }
        public DateTime? DocumentEffDate { get; set; }
        public DateTime? DocumentTermDate { get; set; }
    }
}
