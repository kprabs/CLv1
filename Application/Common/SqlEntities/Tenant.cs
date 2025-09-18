using System.Text.Json.Serialization;

namespace CoreLib.Application.Common.SqlEntities
{
    public class Tenant
    {
        [JsonPropertyName("SegInstId")]
        public int ClassifiedSegmentInstanceId { get; set; }
        [JsonPropertyName("AreaSegNKey")]
        public string ClassifiedAreaSegmentNKey{ get; set; }
        [JsonPropertyName("AreaSegNm")]
        public string ClassifiedAreaSegmentName { get; set; }
        [JsonPropertyName("AreaCd")]
        public string AreaCode { get; set; }
        [JsonPropertyName("AreaNm")]
        public string AreaName { get; set; }
        [JsonPropertyName("SegCd")]
        public string SegmentCode { get; set; }
        [JsonPropertyName("SegNm")]
        public string SegmentName { get; set; }
        [JsonPropertyName("Clnt")]
        public Client[] Clients { get; set; }

        public class Client
        {
            [JsonPropertyName("SegInstId")]
            public int ClassifiedSegmentInstanceId { get; set; }
            [JsonPropertyName("AreaSegNKey")]
            public string ClassifiedAreaSegmentNKey { get; set; }
            [JsonPropertyName("AreaSegNm")]
            public string ClassifiedAreaSegmentName { get; set; }
            [JsonPropertyName("AreaCd")]
            public string AreaCode { get; set; }
            [JsonPropertyName("AreaNm")]
            public string AreaName { get; set; }
            [JsonPropertyName("SegCd")]
            public string SegmentCode { get; set; }
            [JsonPropertyName("SegNm")]
            public string SegmentName { get; set; }
            [JsonPropertyName("Acct")]
            public Account[] Accounts { get; set; }

            public class Account
            {
                [JsonPropertyName("SegInstId")]
                public int ClassifiedSegmentInstanceId { get; set; }
                [JsonPropertyName("AreaSegNKey")]
                public string ClassifiedAreaSegmentNKey { get; set; }
                [JsonPropertyName("AreaSegNm")]
                public string ClassifiedAreaSegmentName { get; set; }
                [JsonPropertyName("AreaCd")]
                public string AreaCode { get; set; }
                [JsonPropertyName("AreaNm")]
                public string AreaName { get; set; }
                [JsonPropertyName("SegCd")]
                public string SegmentCode { get; set; }
                [JsonPropertyName("SegNm")]
                public string SegmentName { get; set; }
                [JsonPropertyName("Chld")]
                public Child[] Children { get; set; }
                public class Child
                {
                    [JsonPropertyName("SegInstId")]
                    public int ClassifiedSegmentInstanceId { get; set; }
                    [JsonPropertyName("AreaSegNKey")]
                    public string ClassifiedAreaSegmentNKey { get; set; }
                    [JsonPropertyName("AreaSegNm")]
                    public string ClassifiedAreaSegmentName { get; set; }
                    [JsonPropertyName("AreaCd")]
                    public string AreaCode { get; set; }
                    [JsonPropertyName("AreaNm")]
                    public string AreaName { get; set; }
                    [JsonPropertyName("SegCd")]
                    public string SegmentCode { get; set; }
                    [JsonPropertyName("SegNm")]
                    public string SegmentName { get; set; }
                }
            }
        }
    }
}
