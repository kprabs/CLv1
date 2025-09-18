using CoreLib.Application.Common.Constants;

namespace CoreLib.Application.Common.ServicePayloads
{
    public class SummaryOfBenefitsAndCoveragePayload
    {
        public string? clientId { get; set; }
        public string? accountId { get; set; }
        public string? subAccountId { get; set; }
        public string? memberId { get; set; }
        public string? groupNumber { get; set; }
        public string? status { get; set; }
        public string? userId { get; set; }
        public string? brand { get; set; }
        public string? partnerName { get; set; } = ESApiRequestParamaterConstant.PartnerName;
        public string? channelName { get; set; } = ESApiRequestParamaterConstant.ChannelName;
        public string? planId { get; set; }
    }
}
