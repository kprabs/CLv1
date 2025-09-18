using CoreLib.Application.Common.Models;

namespace CoreLib.Application.Common.Interfaces
{
    public interface IAuthPayloadService
    {
        AuthPayloadDataDto ReadPayloadByService(string serviceName, string requestPayload);
        bool IsValidJson(string jsonString);
    }
}
