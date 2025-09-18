namespace CoreLib.Application.Common.Models
{
    public class UserSystemAccessDTO
    {
        public int SystemId { get; set; }
        public string SystemCode { get; set; }
        public string Application { get; set; }
        public string Role { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
