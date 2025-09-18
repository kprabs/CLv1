using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreLib.Entities
{
    [Table("DocumentType", Schema = "Code")]
    public partial class DocumentType
    {
        [Key]
        public int DocumentTypeId { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Desc { get; set; }
        public int DocumentCategoryId { get; set; }
        public int DocumentMetaDataProfileId { get; set; }
        public short? SortOrderNum { get; set; }
        public virtual DocumentCategory DocumentCategory { get; set; } = null!;
    }
}
