namespace CoreLib.Application.Common.Constants
{
    public static class ApplicationConstant
    {
        public static readonly string SYSTEM_SUPPORTED_ROLES = "Clientuser,StopLossCarrier,GP_SuperAdmin,RoamBroker,RoamBrokerLevel1";
        public static readonly string DUMMY_BROKER = "brokertest1";
        public static readonly string ActiveWorkbasketNames = "GB- Travelers,CA Member Not Found";
        public static readonly string SUPER_ADMIN_PERMISSION = "GP_USERADMIN,PR_FACCESS,GP_RESOURCE";
        public static readonly string BROKER_PERMISSION = "EBPP_INV_NPHI,GP_CLIENTINFO,GP_ACCPROFILE,PR_FACCESS,GP_RESOURCE";
        public static readonly string BROKER_MEMBER_ACCECC_CHECK = "MBR_VIEW,MET_EDIT";
        public static readonly string BROKER_INDEX_REPORT_ACCECC_CHECK = "IDX_MEM_RPT,IDX_RPT,IDX_SL_RPT";
        public static readonly string AUTH_SESSION_PROCESSING_USER_NAME = "SystemUser";
        public static readonly string AUTH_REFRESH_MILLIS = "3600000";
        public static readonly string DUMMY_STOP_LOSS = "stploss";
        public static readonly string FULLY_QUALIFIED_ADMIN_ROLE_IBX = "ou=IBX,cn=GP_SuperAdmin";
        public static readonly string FULLY_QUALIFIED_ADMIN_ROLE_AH = "ou=AH,cn=GP_SuperAdmin";
        public static readonly string FULLY_QUALIFIED_BROKER_ROLE_IBX = "ou=IBX,cn=RoamBroker|ou=IBX,cn=RoamBrokerLevel1";
        public static readonly string FULLY_QUALIFIED_BROKER_ROLE_AH = "ou=AH,cn=RoamBroker|ou=AH,cn=RoamBrokerLevel1";
        public static readonly string FULLY_QUALIFIED_STOPLOSS_ROLE_AH = "ou=AH,cn=StopLossCarrier";
        public static readonly string BROKER_ROLE = "RoamBroker,RoamBrokerLevel1";
        public static readonly int SYSTEM_ID = 6;
        public static readonly int BROKER_CLIENT_ID_DEFAULT = -1;
        public static readonly string DUMMY_ADMIN = "admintest";
        public static readonly string STOPLOSS_ROLE = "StopLossCarrier";
        public static readonly string SYSTEM_MANAGED_USER = "RoamBroker,RoamBrokerLevel1,StopLossCarrier";
        public static readonly string FULLY_QUALIFIED_STOPLOSS_ROLE_IBX = "ou=IBX,cn=StopLossCarrier";
        public static readonly string ORGANIZATION_CODE = "ou=";
        public static readonly string AUTH_LOGIN_SYSTEM_CODE_FORGEROCK = "FORGEROCK";
        public static readonly string ROLE_CODE = ",cn=";
        public static readonly string ADMIN_ROLE = "GP_SuperAdmin";
        public static readonly int CLIENT_ID_PADDING_LENGHT = 10;
        public static readonly string CLIENT_USER = "Clientuser";
        public static readonly string CLIENT_ADMIN = "Clientadmin";

    }
}
