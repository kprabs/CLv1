using CoreLib.Application.Common.Utility;

namespace CoreLib.Application.Common.Enums
{
    public enum SystemPermissionGroupSet
    {
        [TypeTableCode("MES", "MES Application")]
        MES,
        [TypeTableCode("EDIPortal", "EDI Portal")]
        EDI,
        [TypeTableCode("MemberEligibility", "Member Eligibility")]
        MemberEligibility,
        [TypeTableCode("Claims", "Claims")]
        Claims,
        [TypeTableCode("INDEXReports", "INDEX Reports")]
        INDEXReports,
        [TypeTableCode("FinancialReports", "Financial Reports")]
        FinancialReports,
        [TypeTableCode("ReferralAccess", "Referral Access")]
        ReferralAccess,
        [TypeTableCode("ClaimsWorkbaskets", "Claims Workbaskets")]
        ClaimsWorkbaskets,
        [TypeTableCode("ClientReports", "Client Reports")]
        ClientReports,
        [TypeTableCode("UserAdmin", "UserAdmin")]
        UserAdmin,
        [TypeTableCode("ClientInformation", "Client Information")]
        ClientInformation,
        [TypeTableCode("AccountProfile", "Account Profile")]
        AccountProfile,
        [TypeTableCode("AlacritiBilling", "EBPP (Alacriti Billing)")]
        AlacritiBilling,
        [TypeTableCode("MembershipReports", "Membership Reports")]
        MembershipReports,
        [TypeTableCode("StoplossReports", "Stoploss Reports")]
        StoplossReports,
        [TypeTableCode("Resources", "Resources")]
        Resources,
        [TypeTableCode("Providers", "Providers")]
        Providers,
    }
}
