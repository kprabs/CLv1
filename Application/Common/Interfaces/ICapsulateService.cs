using CoreLib.Application.Common.Models;

namespace CoreLib.Application.Common.Interfaces
{
    public interface ICapsulateService
    {
        Task<string> EncryptAsync(string clearText);
        Task<UserAccessInfoDTO?> DecryptAsync(string encrypted);
    }
}
