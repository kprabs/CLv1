using CoreLib.Application.Common.Constants;

namespace CoreLib.Application.Common.ServicePayloads
{
    public class BenefitsAtAGlancePayload
    {
        public string? clientId { get; set; }
        public string? accountId { get; set; }
        public string? subAccountId { get; set; }
        public string? planId { get; set; }
        public string? planType { get; set; }
        public string? companyCode { get; set; }
        public string? groupNumber { get; set; }
        public string? effectiveDate { get; set; }
        public string? brand { get; set; }
        public string? partnerName { get; set; } = ESApiRequestParamaterConstant.PartnerName;
        public string? channelName { get; set; } = ESApiRequestParamaterConstant.ChannelName;
        public string? memberPlatform { get; set; }
    }
}
