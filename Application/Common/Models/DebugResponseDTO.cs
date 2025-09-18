namespace CoreLib.Application.Common.Models
{
    public class DebugResponseDTO
    {
        public dynamic responseObj { get; set; }
        public bool IsError { get; set; }
        public bool IsDebugMode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
