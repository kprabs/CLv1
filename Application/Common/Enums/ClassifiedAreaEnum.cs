using CoreLib.Application.Common.Utility;

namespace CoreLib.Application.Common.Enums
{
    public enum ClassifiedAreaEnum
    {
        [TypeTableCode("CAS", "Client Account Structure")]
        CAS,
        [TypeTableCode("HR", "HealthRules Structure")]
        HR,
        [TypeTableCode("INEN", "Inbound Enrollment")]
        INEN
    }
}
