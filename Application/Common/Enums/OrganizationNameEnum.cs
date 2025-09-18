using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CoreLib.Application.Common.Enums
{
    public enum OrganizationNameEnum
    {
        [Display(Name = "IBC")]
        [Description("32eae766-d432-45cb-ad60-6a6a2ebf0438")]
        DEVIBC = 1,

        [Display(Name = "IBC")]
        [Description("3fe964a2-2c64-4363-89d5-9f4d7b2e6108")]
        QAIBC = 2,

        [Display(Name = "AH")]
        [Description("e17b1e28-c332-4814-ada2-a00b6d750d1d")]
        DEVAH = 3,

        [Display(Name = "AH")]
        [Description("15cba54b-7a1e-4de1-92ed-d2cc16fe55b1")]
        QAAH = 4,

        [Display(Name = "QCC")]
        [Description("211c55c7-7806-4117-a3a4-ac30db4af11f")]
        DEVQCC = 5,

        [Display(Name = "QCC")]
        [Description("33b4fe18-2390-4363-8d3f-3b469fad0bb5")]
        QAQCC = 6
    }
}
