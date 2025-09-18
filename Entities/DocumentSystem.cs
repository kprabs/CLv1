using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreLib.Entities
{
    [Table("DocumentSystem", Schema = "Code")]
    public partial class DocumentSystem
    {
        [Key]
        public int DocumentSystemId { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Desc { get; set; }
    }
}
