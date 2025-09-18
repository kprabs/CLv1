using CoreLib.Application.Common.Models;
using CoreLib.Application.Common.SqlEntities;
using CoreLib.Application.Common.Utility;

namespace CoreLib.Application.Common.Interfaces
{
    public interface IClientInformationService
    {
        Task<List<AccountDetailEntity>> GetAccount(string clientId, string accountId);
        Task<List<AccountSubaccountEntity>> GetAccountAndSubaccounts(string clientId, string? accountId = null, string? subaccountId = null,
                                                            DateTime? subaccountEffDate = null, DateTime? subaccountTermDate = null);
        Task<ClientDetailEntity> GetClient(string clientId);
        Task<string> GetClientPlatform(string clientId);
        Task<List<ProductDetailEntity>> GetProducts(string clientId, string accountId, string subaccountId);
        Task<List<PackageDetailEntity>> GetPackages(string clientId, string accountId, string subaccountId);
        Task<SubaccountDetailEntity> GetSubaccount(string clientId, string accountId, string subaccountId);
        Task<ApiResponse<List<AccountDto>>> SearchAccountAndSubaccount(SearchAccountAndSubaccountRequestDto request);
        ApiResponse<T> SetClientIdMissingResponse<T>(ApiResponse<T> result);
        ApiResponse<T> SetAccountIdMissingResponse<T>(ApiResponse<T> result);
        ApiResponse<T> SetSubccountIdMissingResponse<T>(ApiResponse<T> result);
        ApiResponse<T> SetMissingParametersResponse<T>(ApiResponse<T> result, string error);
        string? PadClientId(string clientId);
        ApiResponse<T> SetNotFoundResponse<T>(ApiResponse<T> result);
        Task<List<ClientDetailEntity>> GetMultipleClients(List<string> clientIds);
    }
}
