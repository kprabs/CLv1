using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreLib.Entities
{
    [Table("DocumentCategory", Schema = "Code")]
    public partial class DocumentCategory
    {
        [Key]
        public int DocumentCategoryId { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Desc { get; set; }
        public int DocumentSystemId { get; set; }
        public short? SortOrderNum { get; set; }
        public virtual DocumentSystem DocumentSystem { get; set; } = null!;
    }
}
