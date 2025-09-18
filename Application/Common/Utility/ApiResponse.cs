namespace CoreLib.Application.Common.Utility
{
    public class ApiResponse<T>
    {
        public T? data { get; set; }
        public int statusCode { get; set; }
        public bool status { get; set; }
        public IList<string>? errors { get; set; }
        public string? timeStamp { get; set; }
        public string? id { get; set; }
    }
}
