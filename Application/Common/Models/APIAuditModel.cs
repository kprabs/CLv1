namespace CoreLib.Application.Common.Models
{
    // Main API model combining all entities
    public class APIAuditModel
    {
        // APIAudit Details
        public int APIAuditID { get; set; }
        public string SessionNKey { get; set; }
        public int SystemActivityID { get; set; }
        public int ParentSystemActivityID { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUserNKey { get; set; }
        public string CreateForUserNKey { get; set; }
        public Int64? ClientKey { get; set; }
        public Int64? AccountKey { get; set; }
        public Int64? SubAccountKey { get; set; }
        public Int64? MemberKey { get; set; }
        public Int64? SubscriberKey { get; set; }
        public string MemberPlatformCode { get; set; }


        // APIAuditPayload Details
        public List<APIAuditPayloadModel> APIAuditPayloads { get; set; } = [];

        // APIAuditPCP Details
        public List<APIAuditPCPModel> APIAuditPCPs { get; set; } = [];

        // MemberPlanOrderIdcards Details
        public List<MemberPlanOrderIDCardModel> MemberPlanOrderIdcards { get; set; } = [];
    }

    // SystemActivity model
    public class SystemActivityModel
    {
        public int SystemActivityID { get; set; }
        public string SystemActivityName { get; set; }
        public string SystemActivityTypeNKey { get; set; }
        public bool IsActiveFlag { get; set; }
    }

    // APIAuditPayload model
    public class APIAuditPayloadModel
    {
        public int APIAuditPayloadID { get; set; }
        public int APIAuditID { get; set; }
        public string APIEndPointDetail { get; set; }
        public string RequestPayloadHTML { get; set; }
        public string ResponseResultHTML { get; set; }
        public string ResponseCode { get; set; }
    }

    // APIAuditPCP model
    public class APIAuditPCPModel
    {
        public int APIAuditPCPID { get; set; }
        public int APIAuditID { get; set; }
        public Int64 MemberKey { get; set; }
        public string CurrentSupplierLocationNKey { get; set; }
        public int CurrentPCPKey { get; set; }
        public string CurrentPCPName { get; set; }
        public string NewSupplierLocationNKey { get; set; }
        public int NewPCPKey { get; set; }
        public string NewPCPName { get; set; }
        public DateTime EffDate { get; set; }
        public bool IsProcessedFlag { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class MemberPlanOrderIDCardModel
    {
        public int MemberPlanOrderIDCardID { get; set; } // Unique identifier for MemberPlanOrderIDCard

        public int APIAuditID { get; set; } // Foreign key to APIAudit

        public Int64 MemberKey { get; set; } // Identifier for the member

        public int PlanKey { get; set; } // Identifier for the plan

        public DateTime CreateDate { get; set; } // Creation date of the record

        public DateTime EffDate { get; set; } // Effective date of the record
    }
}
