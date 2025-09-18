using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreLib.Entities;

[Table("AutoProvisionLog", Schema = "Main")]
public partial class AutoProvisionLog
{
    [Key]
    public int AutoProvisionLogId { get; set; }
    public string SystemUserName { get; set; } = null!;
    public string? RemarkDetail { get; set; }
    public string? ClientNkey { get; set; }
    public DateTime CreateDate { get; set; }
    public string? AutoProvisionStatusCode { get; set; }
    public string? RequestUserIdNKey { get; set; }
    public DateTime? LastUpdateDate { get; set; }
}
