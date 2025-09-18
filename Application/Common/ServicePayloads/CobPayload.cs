using CoreLib.Application.Common.Constants;

namespace CoreLib.Application.Common.ServicePayloads
{
    public class CobPayload
    {
        public string? memberId { get; set; }
        public string? groupNumber { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? dateOfbirth { get; set; }
        public string? brand { get; set; }
        public string? channelName { get; set; } = ESApiRequestParamaterConstant.ChannelName;
        public string? partnerName { get; set; } = ESApiRequestParamaterConstant.PartnerName;
        public string? memberPlatform { get; set; }
        public string? clientId { get; set; }
        public string? accountId { get; set; }
        public string? subAccountId { get; set; }

    }
}
