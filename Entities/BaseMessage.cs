namespace CoreLib.Entities
{
    public class BaseMessage
    {
        public string? Id { get; set; }
        public string? Timestamp { get; set; }
        public string? ReturnCode { get; set; }
        public string? ReturnCodeDescription { get; set; }
    }
}
