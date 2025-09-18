namespace CoreLib.Application.Common.ServicePayloads
{
    public class EsCrossWalkRequest
    {
        public string? providerId { get; set; }
        public string? hrProviderLocationId { get; set; }
        public string? channelName { get; set; }
        public string? partnerName { get; set; }
        public string? clientId { get; set; }
        public string? accountId { get; set; }
        public string? subAccountId { get; set; }
    }
}
