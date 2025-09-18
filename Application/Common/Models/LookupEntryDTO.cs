namespace CoreLib.Application.Common.Models
{
    public class LookupEntryDTO<IdType, ValueType>
    {
        public IdType Id { get; set; }
        public ValueType Value { get; set; }
        public string Code { get; set; }
        public bool IsPHI { get; set; } = false;
        public Object ExtraData { get; set; }
        public string PermissionCode { get; set; }
    }
}
