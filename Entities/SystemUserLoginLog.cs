using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CoreLib.Entities
{


    [Table("SystemUserLoginLog", Schema = "Main")]
    public partial class SystemUserLoginLog
    {
        [Key]
        public int SystemUserLoginLogId { get; set; }

        public string SystemUserName { get; set; }

        public string PreviousData { get; set; }

        public string NewData { get; set; }

        public string RemarkDetail { get; set; }

        public DateTime LastUpdateDate { get; set; }

        public string LastUpdateUserName { get; set; }
    }
}
