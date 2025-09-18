namespace CoreLib.Application.Common.Constants
{
    public static class UserRoleConstant
    {
        public static readonly string SuperAdmin = "GP_SuperAdmin";
        public static readonly string ClientAdmin = "ClientAdmin";
        public static readonly string ClientUser = "Clientuser";
        public static readonly string BrokerConsultant = "RoamBroker";
        public static readonly string BrokerConsultant1 = "RoamBrokerLevel1";
        public static readonly string StopLossCarrier = "StopLossCarrier";

        public static List<Dictionary<string, List<string>>> GetUserRolesByPermissionCode()
        {
            List<Dictionary<string, List<string>>> dicUserRoleByCode =
            [
                new Dictionary<string, List<string>>
                {
                    { PermissioCodeConstant.LinkEmpPortal, new List<string> { ClientUser } }
                },
                new Dictionary<string, List<string>>
                {
                    { PermissioCodeConstant.LinkEBill, new List<string> { ClientUser } }
                },
                new Dictionary<string, List<string>>
                {
                    {
                        PermissioCodeConstant.ClientInfo,
                        new List<string>
                        {
                            ClientUser, BrokerConsultant, BrokerConsultant1
                        }
                    }
                }
            ];
            return dicUserRoleByCode;
        }
    }
}
