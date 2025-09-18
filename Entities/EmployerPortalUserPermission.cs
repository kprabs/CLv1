using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreLib.Entities
{
    [Table("EmployerPortalUserPermission", Schema = "CNV")]
    public partial class EmployerPortalUserPermission
    {
        [Key]
        public int EmployerPortalUserPermissionId { get; set; }
        public string? VendorName { get; set; }
        public string? ExternalSystemUserNKey { get; set; } = null!;
        public string? EmailAddress { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? GroupNKey { get; set; }
        public string? ClientNKey { get; set; }
        public bool ActiveFlag { get; set; }
        public string? PermissionDetail { get; set; }
        public string? SourceFileTypeCode { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? CreateUserNKey { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string LastUpdateUserNKey { get; set; }
        public string? StatusCode { get; set; }
        public string? ClientIdNKey { get; set; }
    }
}
