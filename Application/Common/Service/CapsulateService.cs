using CoreLib.Application.Common.Interfaces;
using CoreLib.Application.Common.Models;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace CoreLib.Application.Common.Service
{
    public class CapsulateService : ICapsulateService
    {
        public async Task<string> EncryptAsync(string clearText)
        {
            using Aes aes = Aes.Create();
            using MemoryStream output = new();
            using CryptoStream cryptoStream = new(output, aes.CreateEncryptor(), CryptoStreamMode.Write);
            await cryptoStream.WriteAsync(Encoding.Unicode.GetBytes(clearText));
            await cryptoStream.FlushFinalBlockAsync();
            var returnValue = Convert.ToBase64String(output.ToArray());
            return returnValue;
        }

        public async Task<UserAccessInfoDTO?> DecryptAsync(string encrypted)
        {
            var encryptedByte = Convert.FromBase64String(encrypted);

            using Aes aes = Aes.Create();
            using MemoryStream input = new(encryptedByte);
            using CryptoStream cryptoStream = new(input, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using MemoryStream output = new();
            await cryptoStream.CopyToAsync(output);
            return JsonConvert.DeserializeObject<UserAccessInfoDTO>(Encoding.Unicode.GetString(output.ToArray()));
        }
    }
}
