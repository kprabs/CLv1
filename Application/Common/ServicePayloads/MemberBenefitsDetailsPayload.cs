using CoreLib.Application.Common.Constants;

namespace CoreLib.Application.Common.ServicePayloads
{
    public class MemberBenefitsDetailsPayload
    {
        public string? clientId { get; set; }
        public string? accountId { get; set; }
        public string? subAccountId { get; set; }
        public string? brand { get; set; }
        public string? companyCode { get; set; }
        public string? productiId { get; set; }
        public string? coverageType { get; set; }
        public string? effectiveDate { get; set; }
        public string? groupNumber { get; set; }
        public string? channelName { get; set; } = ESApiRequestParamaterConstant.ChannelName;
        public string? partnerName { get; set; } = ESApiRequestParamaterConstant.PartnerName;
        public string? memberPlatform { get; set; }
    }
}
