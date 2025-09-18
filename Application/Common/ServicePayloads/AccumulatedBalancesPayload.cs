using CoreLib.Application.Common.Constants;

namespace CoreLib.Application.Common.ServicePayloads
{
    public class AccumulatedBalancesPayload(string? clientId, string? accountId, string? subAccountId, string? memberId, string? agreementId, string? benefitStartDate, string? benefitEndDate, string? brand, IList<BenefitAccumulator_Group>? groups, IList<string>? products, string? channelName, string? partnerName)
    {
        public string? clientId { get; set; } = clientId;
        public string? accountId { get; set; } = accountId;
        public string? subAccountId { get; set; } = subAccountId;
        public string? memberId { get; set; } = memberId;
        public string? agreementId { get; set; } = agreementId;
        public string? benefitStartDate { get; set; } = benefitStartDate;
        public string? benefitEndDate { get; set; } = benefitEndDate;
        public string? brand { get; set; } = brand;
        public IList<BenefitAccumulator_Group>? groups { get; } = groups;
        public IList<string>? products { get; } = products;
        public string? channelName { get; set; } = string.IsNullOrWhiteSpace(channelName) ? ESApiRequestParamaterConstant.ChannelName : channelName;
        public string? partnerName { get; set; } = string.IsNullOrWhiteSpace(partnerName) ? ESApiRequestParamaterConstant.PartnerName : partnerName;
    }

    public class BenefitAccumulator_Group
    {
        public string? number { get; set; }
        public string? benefitType { get; set; }
    }
}
