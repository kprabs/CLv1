using CoreLib.Application.Common.Utility;

namespace CoreLib.Application.Common.Enums
{
    public enum AppSystemEnum
    {
        [TypeTableCode("CP", "Client Portal")]
        ClientPortal,
        [TypeTableCode("ME", "Member Enrollment")]
        MemberEnrollment,
        [TypeTableCode("EDIP", "EDI Portal")]
        EDIPortal
    }
}
