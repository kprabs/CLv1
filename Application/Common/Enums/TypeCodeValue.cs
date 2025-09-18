namespace CoreLib.Application.Common.Enums
{
    public static class TypeCodeValue
    {
        public static readonly KeyValuePair<string, string> ApplicationCode = new KeyValuePair<string, string>("AUTH_APP_CODE", "CP");
        public static readonly KeyValuePair<string, string> MenuMember = new KeyValuePair<string, string>("MemberMenuAccessTypeCode", "MNU_MEMBER");
        public static readonly KeyValuePair<string, string> MemberView = new KeyValuePair<string, string>("MemberViewAccessTypeCode", "MBR_VIEW");
        public static readonly KeyValuePair<string, string> ReferralView = new KeyValuePair<string, string>("ReferralViewAccessTypeCode", "REF_VIEW");
        public static readonly KeyValuePair<string, string> ClaimDetails = new KeyValuePair<string, string>("ClaimDetailsAccessTypeCode", "CLM_DETAIL");
        public static readonly KeyValuePair<string, string> ClaimLines = new KeyValuePair<string, string>("ClaimLineAccessTypeCode", "CLM_LINES");
        public static readonly KeyValuePair<string, string> ClaimHistory = new KeyValuePair<string, string>("ClaimHistoryAccessTypeCode", "CLM_HIST");
        public static readonly KeyValuePair<string, string> ClaimEOB = new KeyValuePair<string, string>("ClaimEOBAccessTypeCode", "CLM_EOB");
        public static readonly KeyValuePair<string, string> WorkBasketsEdit = new KeyValuePair<string, string>("WorkBasketsEditAccessTypeCode", "CLMWB_EDIT");
        public static readonly KeyValuePair<string, string> WorkBasketsView = new KeyValuePair<string, string>("WorkBasketsViewAccessTypeCode", "CLMWB_VIEW");
        public static readonly KeyValuePair<string, string> InvoiceSummer = new KeyValuePair<string, string>("INVOICE_SUMMARY_ACCESS", "INV_SUM");
        public static readonly KeyValuePair<string, string> InvoiceDetails = new KeyValuePair<string, string>("INVOICE_DETAIL_ACCESS", "INV_DTL");

        public static readonly KeyValuePair<string, string> CabsSummary = new KeyValuePair<string, string>("CABS_SUMMARY", "CABS_SUM");
        public static readonly KeyValuePair<string, string> CabsDetails = new KeyValuePair<string, string>("CABS_DETAIL", "CABS_DTL");
        public static readonly KeyValuePair<string, string> PRCEligibleUDTName = new KeyValuePair<string, string>("PRCEligibleUDTName", "PRC Eligibility");
    }
}
