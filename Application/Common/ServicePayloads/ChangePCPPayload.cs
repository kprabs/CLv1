using Newtonsoft.Json;

namespace CoreLib.Application.Common.ServicePayloads
{
    public class ChangePCPPayload
    {
        public Member member { get; set; }
        public PCP pcp { get; set; }
        public string? clientId { get; set; }
        public string? accountId { get; set; }
        public string? subAccountId { get; set; }
    }

    public class Member
    {
        public string memberId { get; set; }
        public string memberPlatform { get; set; }
        public string channelName { get; set; }
        public string brand { get; set; }
        public string partnerName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string dateOfBirth { get; set; }
        public string agreementId { get; set; }
        public string groupNumber { get; set; }
        public string planId { get; set; }

    }



    public class PCP
    {
        public CurrentPCP current { get; set; }

        [JsonProperty("new")]
        public NewPCP NEW { get; set; }

    }

    public class CurrentPCP
    {
        public string id { get; set; }
        public string effectiveDate { get; set; }
        public string supplierLocationId { get; set; }
        public string practitionerRoleName { get; set; }
        public string practitionerId { get; set; }

    }

    public class NewPCP
    {
        public string id { get; set; }
        public string effectiveDate { get; set; }
        public string isCurrentPatient { get; set; }
        public string supplierLocationId { get; set; }
        public string practitionerRoleName { get; set; }
        public string practitionerId { get; set; }

    }
}
