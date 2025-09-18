namespace CoreLib.Application.Common.Models
{
    public class TreeGridHeaderDTO
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsShown { get; set; } = true;
        public string DataKey { get; set; }
        public bool IsPHI { get; set; } = false;
        public string PermissionCode { get; set; }
    }
}
