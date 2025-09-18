using CoreLib.Application.Common.Constants;
using CoreLib.Application.Common.DBQueries;
using CoreLib.Application.Common.Interfaces;
using CoreLib.Application.Common.Models;
using CoreLib.Application.Common.SqlEntities;
using CoreLib.Application.Common.Utility;
using CoreLib.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;

namespace CoreLib.Application.Common.UserAuthService
{
    public class ClientInformationService(ILogger<ClientInformationService> logger) : IClientInformationService
    {
        private const string ClientIdParameter = "@ClientId";
        private const string AccountIdParameter = "@AccountId";
        private const string SubaccountIdParameter = "@SubAccountId";
        private const string SubaccountEffDateParameter = "@SubAccountEffectiveDt";
        private const string SubaccountTermDateParameter = "@SubAccountTerminationDt";
        private readonly string connString = ConfigurationHelper.config.GetSection("ConnectionStrings:ICPEntities").Value;
        private readonly string connString_gp = ConfigurationHelper.config.GetSection("ConnectionStrings:GroupPortalEntities").Value;

        public async Task<List<AccountDetailEntity>> GetAccount(string clientId, string accountId)
        {
            Dictionary<string, string> objParams = new()
            {
                { ClientIdParameter, clientId },
                { AccountIdParameter, accountId }
            };

            var dataResult = SqlWrapper.ExecuteDataTable(connString, SqlQueries.ICPGetAccountDetailQuery, objParams, logger);

            if (dataResult == null || dataResult.Rows.Count == 0)
            {
                return [];
            }
            List<AccountDetailEntity> result = DBUtilities.ConvertDataTable<AccountDetailEntity>(dataResult);

            return result;
        }

        public async Task<List<AccountSubaccountEntity>> GetAccountAndSubaccounts(string clientId, string? accountId = null, string? subaccountId = null,
                                                            DateTime? subaccountEffDate = null, DateTime? subaccountTermDate = null)
        {
            Dictionary<string, string> objParams = new()
            {
                { ClientIdParameter, clientId }
            };

            string query = SqlQueries.ICPGetAccountAndSubaccountQuery;
            string effDate = Utilities.FormatDate(subaccountEffDate);
            string terminationDate = Utilities.FormatDate(subaccountTermDate);
            query = query.Replace(AccountIdParameter, string.IsNullOrEmpty(accountId)
                                                         ? string.Empty
                                                         : $" And  B.SRC_GRP_ORG_ID='{accountId}'", StringComparison.OrdinalIgnoreCase);
            query = query.Replace(SubaccountIdParameter, string.IsNullOrEmpty(subaccountId)
                                                         ? string.Empty
                                                         : $" And  B.SRC_GRP_NO='{subaccountId}'", StringComparison.OrdinalIgnoreCase);
            query = query.Replace(SubaccountEffDateParameter, string.IsNullOrEmpty(effDate)
                                                         ? string.Empty
                                                         : $" And B.GRP_EFF_DT='{effDate}'", StringComparison.OrdinalIgnoreCase);
            query = query.Replace(SubaccountTermDateParameter, string.IsNullOrEmpty(terminationDate)
                                                         ? string.Empty
                                                         : $" And B.GRP_EXP_DT='{terminationDate}'", StringComparison.OrdinalIgnoreCase);

            var dataResult = SqlWrapper.ExecuteDataTable(connString, query, objParams, logger);

            if (dataResult == null || dataResult.Rows.Count == 0)
            {
                return [];
            }
            List<AccountSubaccountEntity> result = DBUtilities.ConvertDataTable<AccountSubaccountEntity>(dataResult);
            return result;
        }

        public async Task<ClientDetailEntity> GetClient(string clientId)
        {
            Dictionary<string, string> objParams = new()
            {
                { ClientIdParameter, clientId }
            };

            var dataResult = SqlWrapper.ExecuteDataTable(connString, SqlQueries.ICPGetClientDetailQuery, objParams, logger);

            if (dataResult == null || dataResult.Rows.Count == 0)
            {
                return new ClientDetailEntity();
            }
            ClientDetailEntity result = DBUtilities.ConvertDataTable<ClientDetailEntity>(dataResult)[0];

            return result;
        }

        public async Task<List<ClientDetailEntity>> GetMultipleClients(List<string> clientIds)
        {

            List<ClientDetailEntity> clientDetailEntities = [];
            List<string?> clientIdList = PadClientIds(clientIds);
            string clientIdParam = string.Join(",",clientIdList);
            Dictionary<string, string> objParams = new()
            {
                { ClientIdParameter, clientIdParam }
            };

            var dataResult = SqlWrapper.ExecuteDataTable(connString, SqlQueries.ICPGetClientDetailQuery, objParams, logger);

            if (dataResult == null || dataResult.Rows.Count == 0)
            {
                return clientDetailEntities;
            }
            clientDetailEntities = DBUtilities.ConvertDataTable<List<ClientDetailEntity>>(dataResult)[0];

            return clientDetailEntities;
        }

        public async Task<string> GetClientPlatform(string clientId)
        {
            Dictionary<string, string> objParams = new()
            {
                { ClientIdParameter, clientId }
            };

            var dataResult = SqlWrapper.ExecuteDataTable(connString, SqlQueries.ICPGetClientPlatformQuery, objParams, logger);

            if (dataResult == null || dataResult.Rows.Count == 0)
            {
                return string.Empty;
            }
            return dataResult.Rows[0][0].ToString();
        }

        public async Task<List<ProductDetailEntity>> GetProducts(string clientId, string accountId, string subaccountId)
        {
            Dictionary<string, string> objParams = new()
            {
                { ClientIdParameter, clientId },
                { AccountIdParameter, accountId },
                { SubaccountIdParameter, subaccountId }
            };

            var dataResult = SqlWrapper.ExecuteDataTable(connString, SqlQueries.ICPGetProductsQuery, objParams, logger);

            if (dataResult == null || dataResult.Rows.Count == 0)
            {
                return [];
            }
            List<ProductDetailEntity> result = DBUtilities.ConvertDataTable<ProductDetailEntity>(dataResult);

            return result;
        }

        public async Task<List<PackageDetailEntity>> GetPackages(string clientId, string accountId, string subaccountId)
        {
            Dictionary<string, string> objParams = new()
            {
                { ClientIdParameter, clientId },
                { AccountIdParameter, accountId },
                { SubaccountIdParameter, subaccountId }
            };

            var dataResult = SqlWrapper.ExecuteDataTable(connString, SqlQueries.ICPGetPackagesQuery, objParams, logger);

            if (dataResult == null || dataResult.Rows.Count == 0)
            {
                return [];
            }
            List<PackageDetailEntity> result = DBUtilities.ConvertDataTable<PackageDetailEntity>(dataResult);

            return result;
        }

        public async Task<SubaccountDetailEntity> GetSubaccount(string clientId, string accountId, string subaccountId)
        {
            Dictionary<string, string> objParams = new()
            {
                { ClientIdParameter, clientId },
                { AccountIdParameter, accountId },
                { SubaccountIdParameter, subaccountId }
            };

            var dataResult = SqlWrapper.ExecuteDataTable(connString, SqlQueries.ICPGetSubaccountDetailQuery, objParams, logger);

            if (dataResult == null || dataResult.Rows.Count == 0)
            {
                return new SubaccountDetailEntity();
            }
            var result = DBUtilities.ConvertDataTable<SubaccountDetailEntity>(dataResult)[0];
            return result;
        }

        public async Task<ApiResponse<List<AccountDto>>> SearchAccountAndSubaccount(SearchAccountAndSubaccountRequestDto request)
        {
            ApiResponse<List<AccountDto>> result = new();

            if (string.IsNullOrEmpty(request.ClientId))
            {
                return SetClientIdMissingResponse(result);
            }

            List<AccountSubaccountEntity> list = await GetAccountAndSubaccounts(PadClientId(request.ClientId), request.AccountId,
                                                                                              request.SubaccountId, request.SubaccountEffDate, request.SubaccountTermDate);

            if (list == null || list.Count == 0)
            {
                var response = SetNotFoundResponse(result);
                response.data = [];
                return response;
            }

            List<AccountDto> responseData = [];
            var accountResult = list.GroupBy(x => x.AccountId);

            foreach (var accounts in accountResult)
            {
                var accountDetail = new AccountDto
                {
                    AccountId = accounts.First().AccountId,
                    AccountName = accounts.First().AccountName,
                    AccountPlatformName = accounts.Select(x => x.SubaccountPlatformName).Distinct().ToList().Count > 1
                                                ? Platform.Mixed
                                                : accounts.Select(x => x.SubaccountPlatformName).Distinct().FirstOrDefault()
                };
                responseData.Add(accountDetail);
                var subAccountsResult = list.Where(x => x.AccountId == accounts.First().AccountId).GroupBy(x => x.SubaccountId);
                List<SubaccountDto> subaccountList = [];
                foreach (var subAccs in subAccountsResult)
                {
                    SubaccountDto subAccountDetail = new()
                    {
                        SubaccountId = subAccs.First().SubaccountId,
                        SubaccountName = subAccs.First().SubaccountName,
                        PlatformName = subAccs.First().SubaccountPlatformName,
                        EffectiveDate = Utilities.DateConvertion(subAccs.First().SubaccountEffectiveDate),
                        TerminationDate = Utilities.DateConvertion(subAccs.First().SubaccountTerminationDate)
                    };
                    subaccountList.Add(subAccountDetail);
                }

                responseData[^1].Subaccounts.AddRange(subaccountList.OrderBy(x =>
                {
                    return int.TryParse(x.SubaccountId, CultureInfo.InvariantCulture, out int i) ? i : int.MinValue;
                }).ToList());
            }
            result.data = responseData;
            result.statusCode = StatusCodes.Status200OK;
            return result;
        }

        public ApiResponse<T> SetClientIdMissingResponse<T>(ApiResponse<T> result)
        {
            return SetMissingParametersResponse(result, "ClientId not provided");
        }

        public ApiResponse<T> SetAccountIdMissingResponse<T>(ApiResponse<T> result)
        {
            return SetMissingParametersResponse(result, "AccountId not provided");
        }

        public ApiResponse<T> SetSubccountIdMissingResponse<T>(ApiResponse<T> result)
        {
            return SetMissingParametersResponse(result, "SubaccountId not provided");
        }

        public ApiResponse<T> SetMissingParametersResponse<T>(ApiResponse<T> result, string error)
        {
            result.data = default;
            result.statusCode = StatusCodes.Status400BadRequest;
            result.errors = [error];
            return result;
        }
        public string? PadClientId(string clientId)
        {
            return clientId == null ? clientId : clientId.PadLeft(ApplicationConstant.CLIENT_ID_PADDING_LENGHT, '0');
        }

        public List<string?> PadClientIds(List<string> clientIds)
        {
            List<string?> returnClientIds = [];
            foreach (string clientId in clientIds)
            {
                returnClientIds.Add(clientId == null ? clientId : "'" + clientId.PadLeft(ApplicationConstant.CLIENT_ID_PADDING_LENGHT, '0') + "'");
            }
            return returnClientIds;
        }

        public ApiResponse<T> SetNotFoundResponse<T>(ApiResponse<T> result)
        {
            result.data = default;
            result.statusCode = StatusCodes.Status404NotFound;
            result.errors = ["NOT FOUND"];
            return result;
        }
    }
}
