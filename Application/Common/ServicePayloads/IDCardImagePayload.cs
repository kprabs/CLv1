namespace CoreLib.Application.Common.ServicePayloads
{
    public class IDCardImagePayload
    {
        public string? clientId { get; set; }
        public string? accountId { get; set; }
        public string? subAccountId { get; set; }
        public string? memberId { get; set; }
        public string? firstName { get; set; }
        public string? dateOfBirth { get; set; }
        public string? groupNumber { get; set; }
        public string? effectiveDate { get; set; }
        public string? brand { get; set; }
        public string? memberPlatform { get; set; }
    }
}
