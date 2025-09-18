using CoreLib.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreLib.Infrastructure.Persistence
{
    public partial class GroupPortalDbContext(DbContextOptions<GroupPortalDbContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ArgumentNullException.ThrowIfNull(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
        public DbSet<DocumentInstance> DocumentInstances { get; set; }
        public DbSet<UserDocumentAction> UserDocumentAction { get; set; }
        public DbSet<DocumentType> DocumentTypeAction { get; set; }
        public DbSet<EmployerPortalUserPermission> EmployerPortalUserPermission { get; set; }
        public DbSet<AutoProvisionLog> AutoProvisionLogs { get; set; }
        public DbSet<SystemUserLoginLog> SystemUserLoginLogs { get; set; }
        public DbSet<APIAudit> APIAudits { get; set; }
        public DbSet<APIAuditPayload> APIAuditPayloads { get; set; }
        public DbSet<APIAuditPCP> APIAuditPCPs { get; set; }
    }
}
