using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreLib.Entities
{
    [Table("DocumentMetaDataProfile", Schema = "Code")]
    public partial class DocumentMetaDataProfile
    {
        [Key]
        public int DocumentMetaDataProfileId { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Desc { get; set; }
    }
}
