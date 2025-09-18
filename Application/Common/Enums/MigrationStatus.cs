using CoreLib.Application.Common.Utility;

namespace CoreLib.Application.Common.Enums
{
    public enum MigrationStatus
    {
        [TypeTableCode("NotMigrated", "NotMigrated")]
        NotMigrated,
        [TypeTableCode("InProgress", "InProgress")]
        Inprogress,
        [TypeTableCode("Migrated", "Migrated")]
        Migrated
    }
}
