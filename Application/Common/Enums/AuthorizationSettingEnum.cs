using CoreLib.Application.Common.Utility;

namespace CoreLib.Application.Common.Enums
{
    public enum AuthorizationSettingEnum
    {
        [TypeTableCode("AUTH_SESSION_CREATE_TOKEN")]
        SessionCreateToken,
        [TypeTableCode("AUTH_SESSION_PROCESSING_USER_SYSTEM")]
        SessionProcessingUserSystem,
        [TypeTableCode("AUTH_SESSION_PROCESSING_USER_NAME")]
        SessionProcessingUserName,

        [TypeTableCode("AUTH_REFRESH_MILLIS")]
        RefreshMillis,
        [TypeTableCode("AUTH_MODE")]
        AuthorizationMode,
        [TypeTableCode("AUTH_APP_CODE")]
        ApplicationCode,
        [TypeTableCode("AUTH_LOGIN_SYSTEM_CODE")]
        LoginSystemCode,

        [TypeTableCode("AUTH_OAM_HEADER_USERNAME")]
        OAMHeaderUserName,
        [TypeTableCode("AUTH_OAM_HEADER_GROUPS")]
        OAMHeaderGroups,
        [TypeTableCode("AUTH_OAM_HEADER_GROUPSDN")]
        OAMHeaderGroupsDN,
        [TypeTableCode("AUTH_OAM_HEADER_OU")]
        OAMHeaderOrganizationUnit,
        [TypeTableCode("AUTH_OAM_HEADER_FIRSTNAME")]
        OAMHeaderFirstName,
        [TypeTableCode("AUTH_OAM_HEADER_LASTNAME")]
        OAMHeaderLastName,
        [TypeTableCode("AUTH_OAM_HEADER_EMAIL")]
        OAMHeaderEmail,

        [TypeTableCode("AUTH_HARDCODE_USERNAME")]
        HardCodeValueUserName,
        [TypeTableCode("AUTH_HARDCODE_GROUPS")]
        HardCodeValueGroups,
        [TypeTableCode("AUTH_HARDCODE_GROUPSET")]
        HardCodeValueGroupSet,
        [TypeTableCode("AUTH_HARDCODE_FIRSTNAME")]
        HardCodeValueFirstName,
        [TypeTableCode("AUTH_HARDCODE_LASTNAME")]
        HardCodeValueLastName,
        [TypeTableCode("AUTH_HARDCODE_EMAIL")]
        HardCodeValueEmail
    }
}
