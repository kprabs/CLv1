using CoreLib.Application.Common.Constants;

namespace CoreLib.Application.Common.ServicePayloads
{
    public class PrimaryCareDetailsPayload
    {
        public string? channelName { get; set; } = ESApiRequestParamaterConstant.ChannelName;
        public string? partnerName { get; set; } = ESApiRequestParamaterConstant.PartnerName;
        public string? brand { get; set; }
        public string? memberId { get; set; }
        public string? dateOfBirth { get; set; }
        public string? firstName { get; set; }
        public string? clientId { get; set; }
        public string? isSubscriber { get; set; }
        public string? internalContractId { get; set; }
        //public DbEnum sourceSystem { get; set; }
        public string sourceSystem { get; set; }
        public string? accountId { get; set; }
        public string? subAccountId { get; set; }
    }
}
