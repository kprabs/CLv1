using CoreLib.Application.Common.Utility;

namespace CoreLib.Application.Common.Enums
{
    public enum ClassifiedSegmentEnum
    {
        [TypeTableCode("TENANT", "Tenant")]
        Tenant,
        [TypeTableCode("CLIENT", "Client")]
        Client,
        [TypeTableCode("ACCOUNT", "Account")]
        Account,
        [TypeTableCode("SACCOUNT", "Sub Account")]
        SubAccount,
        [TypeTableCode("BGROUP", "Billing Group")]
        BillingGroup,
        [TypeTableCode("CLTR", "Claims Trigger")]
        ClaimsTrigger,
        [TypeTableCode("USRGRP", "User Group")]
        UserGroup,
        [TypeTableCode("TP", "Trading Partner")]
        TradingPartner,
        [TypeTableCode("BTACCOUNT", "Bill To Account")]
        BillToAccount
    }
}
