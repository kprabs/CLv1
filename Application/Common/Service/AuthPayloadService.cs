using CoreLib.Application.Common.Interfaces;
using CoreLib.Application.Common.Models;
using CoreLib.Application.Common.ServicePayloads;
using Newtonsoft.Json;

namespace CoreLib.Application.Common.Service
{
    internal class AuthPayloadService() : IAuthPayloadService
    {
        public AuthPayloadDataDto ReadPayloadByService(string serviceName, string requestPayload)
        {
            AuthPayloadDataDto authData = new();

            switch (serviceName.ToUpperInvariant())
            {
                case "GETMEMBERSBYSEARCHCRITERIA":
                    authData.onlyClientFilter = false;
                    MemberSearchCriteriaPayload? MemberSearchCriteriaPayloadRequestBody;
                    MemberSearchCriteriaPayloadRequestBody = JsonConvert.DeserializeObject<MemberSearchCriteriaPayload>(requestPayload);

                    authData.clientId = MemberSearchCriteriaPayloadRequestBody?.ClientId;
                    if (!string.IsNullOrEmpty(MemberSearchCriteriaPayloadRequestBody.AccountNumber))
                    {
                        authData.accounts.Add(MemberSearchCriteriaPayloadRequestBody.AccountNumber);
                    }

                    if (!string.IsNullOrEmpty(MemberSearchCriteriaPayloadRequestBody.SubAccountNumber))
                    {
                        authData.subAccounts.Add(MemberSearchCriteriaPayloadRequestBody.SubAccountNumber);
                    }
                    break;
                case "GETMEMBERINFORMATION":
                    authData.onlyClientFilter = false;
                    authData = SetObjectValue<MemberInformationPayload>(requestPayload, authData);
                    break;
                case "GETIDCARDIMAGE":
                    authData.onlyClientFilter = false;
                    authData = SetObjectValue<IDCardImagePayload>(requestPayload, authData);
                    break;
                case "GETACCUMULATEDBALANCES":
                    authData.onlyClientFilter = false;
                    authData = SetObjectValue<AccumulatedBalancesPayload>(requestPayload, authData);
                    break;
                case "GETSUMMARYOFBENEFITSANDCOVERAGE":
                    authData.onlyClientFilter = false;
                    authData = SetObjectValue<SummaryOfBenefitsAndCoveragePayload>(requestPayload, authData);
                    break;
                case "GETBENEFITSBOOKLET":
                    authData.onlyClientFilter = false;
                    authData = SetObjectValue<BenefitsBookletPayload>(requestPayload, authData);
                    break;
                case "GETBENEFITSATAGLANCE":
                    authData.onlyClientFilter = false;
                    authData = SetObjectValue<BenefitsAtAGlancePayload>(requestPayload, authData);
                    break;
                case "GETMEMBERBENEFITSDETAILS":
                    authData.onlyClientFilter = false;
                    authData = SetObjectValue<MemberBenefitsDetailsPayload>(requestPayload, authData);
                    break;

                case "GETPRIMARYCAREDETAIL":
                    authData.onlyClientFilter = false;
                    authData = SetObjectValue<PrimaryCareDetailsPayload>(requestPayload, authData);
                    break;
                case "PRINTIDCARD":
                    authData.onlyClientFilter = false;
                    authData = SetObjectValue<PrintIdCardPayload>(requestPayload, authData);
                    break;
                case "GETCOORDINATIONOFBENEFITS":
                    authData.onlyClientFilter = false;
                    authData = SetObjectValue<CobPayload>(requestPayload, authData);
                    break;
                case "CHANGEPCPDETAIL":
                    authData.onlyClientFilter = false;
                    authData = SetObjectValue<ChangePCPPayload>(requestPayload, authData);
                    break;
                case "GETPROVIDERDETAILBYID":
                    authData.onlyClientFilter = false;
                    authData = SetObjectValue<EsCrossWalkRequest>(requestPayload, authData);
                    break;
                default:
                    break;
            }
            return (authData);
        }

        private static AuthPayloadDataDto SetObjectValue<T>(string requestPayload, AuthPayloadDataDto authData)
        {
            dynamic requestBody = JsonConvert.DeserializeObject<T>(requestPayload);
            authData.clientId = requestBody.clientId;
            authData.accounts.Add(requestBody.accountId);
            authData.subAccounts.Add(requestBody.subAccountId);
            return (authData);
        }

        public bool IsValidJson(string jsonString)
        {
            bool isValid;
            try
            {
                JsonConvert.DeserializeObject(jsonString);
                isValid = true;
            }
            catch (Exception)
            {
                isValid = false;
            }
            return isValid;
        }
    }
}
