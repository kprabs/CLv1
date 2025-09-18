using CoreLib.Application.Common.Constants;

namespace CoreLib.Application.Common.ServicePayloads
{
    public class BenefitsBookletPayload
    {
        public string? clientId { get; set; }
        public string? accountId { get; set; }
        public string? subAccountId { get; set; }
        public string? brand { get; set; }
        public string? groupNumber { get; set; }
        public string? coverageEffectiveDate { get; set; }
        public string? coverageExpiryDate { get; set; }
        public string? channelName { get; set; } = ESApiRequestParamaterConstant.ChannelName;
        public string? partnerName { get; set; } = ESApiRequestParamaterConstant.PartnerName;
        public string? planId { get; set; }
        public string? medicalCoverage { get; set; }
        public string? visionCoverage { get; set; }
        public string? drugCoverage { get; set; }
        public string? dentalCoverage { get; set; }
        public string? memberPlatform { get; set; }
        public string? memberId { get; set; }
        public string? firstName { get; set; }
        public string? dateOfBirth { get; set; }
        public string? medInfocusId { get; set; }
        public string? rxInfocusId { get; set; }
        public string? visInfocusId { get; set; }
        public string? denInfocusId { get; set; }
    }
}
