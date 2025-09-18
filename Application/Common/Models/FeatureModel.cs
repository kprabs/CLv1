namespace CoreLib.Application.Common.Models
{
    #region main/core models
    public class FeatureModel
    {
        public string FeatureName { get; set; }
        public FeatureConfiguration? Configuration { get; set; }
    }

    public class FeatureConfiguration
    {
        public string Value { get; set; }
        public List<Brand>? Brand { get; set; } = [];
    }

    public class Brand
    {
        public string? BrandName { get; set; }
        public List<FeatureClient>? Client { get; set; } = [];
    }

    public class FeatureClient
    {
        public string? ClientId { get; set; }
        public List<PlatformModel>? Platform { get; set; } = [];
    }

    public class PlatformModel
    {
        public string? PlatformIndicator { get; set; }
        public List<UserRole>? UserRole { get; set; } = [];
    }

    public class UserRole
    {
        public string? Role { get; set; }
        public List<Users>? Users { get; set; } = [];
    }

    public class Users
    {
        public List<string>? UserId { get; set; } = [];
    }
    #endregion

    #region request/response models
    public class FeatureResponse
    {
        public string FeatureName { get; set; }
        public bool? Value { get; set; } = null;
    }

    public class FeatureEvalResponse : FeatureResponse
    {
        public List<ReasonLogItem> ReasonLog { get; set; } = new List<ReasonLogItem>();
    }


    public class FeatureRequest 
    {
        public string BrandName { get; set; }
        public string ClientId { get; set; }
        public string PlatformIndicator { get; set; }
        public string UserRole { get; set; }
        public List<string> UserIds { get; set; } = new List<string>();

        public string FeatureName { get; set; } = "*";
    }

    public class FeatureEvalRequest
    {
        public FeatureRequest Request { get; set; }
        public FeatureModel Feature { get; set; }
    }

    public class FeatureEvalResponse<T>
    {
        //for single matches
        public T? Value { get; set; }
        public int? Count { get; set; }

        //for multiple matches
        public List<T> Values { get; set; }
        public int Counts { get; set; }
    }

    public class FeatureEvalResultCheck
    {
        public FeatureEvalResultCheck(int result, List<ReasonLogItem> reasonLog, int wildcardCount, int finalResult)
        {
            Counts = result;
            ReasonLog = reasonLog;
            Wildcards = wildcardCount;
            FinalCounts = finalResult;
        }

        public int Counts { get; set; }
        public List<ReasonLogItem> ReasonLog { get; set; } = new List<ReasonLogItem>();

        // used for internal result checks
        public int Wildcards { get; set; }
        public int FinalCounts { get; set; }
    }
    #endregion

    #region misc
    public class ReasonLogItem
    {
        public ReasonLogItem() { }
        public ReasonLogItem(string reason) 
        {
            Details = reason;
        }
        public ReasonLogItem(string step, string reason)
        {
            Step = step;
            Details = reason;
        }
        public ReasonLogItem(int line, string step, string reason)
        {
            Line = line;
            Step = step;
            Details = reason;
        }

        public int Line {  get; set; }
        string? Step { get; set; }
        string? Details { get; set; }
    }
    #endregion
}
