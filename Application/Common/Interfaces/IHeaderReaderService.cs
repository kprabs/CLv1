using CoreLib.Application.Common.Models;
using Microsoft.AspNetCore.Http;

namespace CoreLib.Application.Common.Interfaces
{
    public interface IHeaderReaderService
    {
        DebugResponseDTO GetUserHeaders(IHeaderDictionary Headers);
        DebugResponseDTO GetUserNameFromHeaders(IHeaderDictionary Headers);
        UserAccessInfoDTO CreateDummyAccessInfo(string userName);
        bool IsBroker(UserHeadersDTO userHeaders);
    }
}
