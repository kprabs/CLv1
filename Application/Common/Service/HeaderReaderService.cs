using CoreLib.Application.Common.Constants;
using CoreLib.Application.Common.Interfaces;
using CoreLib.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Globalization;

namespace CoreLib.Application.Common.Service
{
    internal static partial class HeaderReaderServiceLogMessages
    {
        [LoggerMessage(Level = LogLevel.Information, EventId = 1, Message = "BrandOrgID = {strBrandName}, UserRole = {strUserRole}, BrandOrgName = {strBrand_org_name}, keyValuePairs = {keyValuePairs}")]
        internal static partial void ReceivedHeaders(this ILogger logger, StringValues strBrandName, StringValues strUserRole, StringValues strBrand_org_name, Dictionary<string, string> keyValuePairs);
    }

    public class HeaderReaderService : IHeaderReaderService
    {
        private bool isLocallyDeployed = false;
        private readonly string ORGANIZATION_HEADER;
        private string TEST_ORGANIZATION_HEADER;
        private readonly string USER_ROLE_HEADER;
        private readonly string TEST_USER_ROLE_HEADER;
        private readonly string USER_NAME_HEADER;
        private readonly string TEST_USER_NAME;
        private readonly string USER_FIRST_NAME;
        private readonly string TEST_USER_FIRSTNAME;
        private readonly string USER_LAST_NAME;
        private readonly string TEST_USER_LASTNAME;
        private readonly string sso_token;
        private readonly string Test_sso_token;
        private readonly string logintimestamp;
        private readonly string LAST_LOGIN_TIMESTAMP;
        private readonly string MVP_USERNAME;
        private readonly string TEST_MVP_USERNAME;
        private readonly string MVP_ROLE;
        private readonly string TEST_MVP_ROLE;
        private readonly string MVP_LOGINID;
        private readonly string TEST_MVP_LOGINID;
        private readonly string BROKER_CLIENT_HEADER;
        private readonly string TEST_BROKER_CLIENT_HEADER;
        private readonly string BROKER_CLIENT_MEMBERAVIALABILITY_HEADER;
        private readonly bool TEST_BROKER_CLIENT_MEMBERAVIALABILITY_HEADER = false;
        private readonly string AUTH_SECRET;
        private readonly string ibcgroup;
        private readonly string TEST_ibcgroup;
        private readonly string hmkfederatedas;
        private readonly string TEST_hmkfederatedas;
        private readonly string userEmail;
        private readonly string TEST_USER_EMAIL;
        private readonly string is_IndRptAvailable;
        private readonly string Brand_org_name;
        private readonly string TEST_Brand_org_name;
        private readonly string BROKER_ROLE;
        private readonly string BROKER_ORG_IBX;
        private readonly string BROKER_ORG_AH;
        private readonly string ORG_IBX;
        private readonly string ORG_AH;
        private readonly bool TEST_IsIndRptAvailable = false;
        private readonly bool TEST_IsSalesStlsAvailable = false;
        private readonly string is_SalesStlsAvailable;
        private readonly Dictionary<string, string> keyValuePairs;
        private readonly ILogger<HeaderReaderService> _logger;

        public HeaderReaderService(IConfiguration configuration, ILogger<HeaderReaderService> logger)
        {
            isLocallyDeployed = configuration.GetValue<bool>(AppSettingsConstants.ConfigKeyIsLocallyDeployed);
            ORGANIZATION_HEADER = HeaderConstant.ORGANIZATION_HEADER;
            TEST_ORGANIZATION_HEADER = configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTestOrganizationHeader);
            USER_ROLE_HEADER = HeaderConstant.USER_ROLE_HEADER;
            TEST_USER_ROLE_HEADER = configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTestUserRole);
            USER_NAME_HEADER = HeaderConstant.USER_NAME_HEADER;
            TEST_USER_NAME = configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTestUserName);
            USER_FIRST_NAME = HeaderConstant.USER_FIRST_NAME;
            TEST_USER_FIRSTNAME = configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTestFirstName);
            USER_LAST_NAME = HeaderConstant.USER_LAST_NAME;
            TEST_USER_LASTNAME = configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTestLastName);
            sso_token = HeaderConstant.sso_token;
            Test_sso_token = configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTestSsoToken);
            logintimestamp = HeaderConstant.login_timestamp;
            LAST_LOGIN_TIMESTAMP = HeaderConstant.LAST_LOGIN_TIMESTAMP;
            MVP_USERNAME = HeaderConstant.MVP_USERNAME;
            TEST_MVP_USERNAME = configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTestMvpUserName);
            MVP_ROLE = HeaderConstant.MVP_ROLE;
            TEST_MVP_ROLE = configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTestMvpRole);
            MVP_LOGINID = HeaderConstant.MVP_LOGINID;
            TEST_MVP_LOGINID = configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTestMvpLoginId);
            BROKER_CLIENT_HEADER = HeaderConstant.BROKER_CLIENT_HEADER;
            TEST_BROKER_CLIENT_HEADER = configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTestBrokerClientHeader);
            BROKER_CLIENT_MEMBERAVIALABILITY_HEADER = HeaderConstant.BROKER_CLIENT_MEMBERAVIALABILITY_HEADER;
            TEST_BROKER_CLIENT_MEMBERAVIALABILITY_HEADER = configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<bool>(AppSettingsConstants.ConfigKeyTestBrokerClientMemeberAvailabilityHeader);
            AUTH_SECRET = HeaderConstant.AUTH_SECRET;
            ibcgroup = HeaderConstant.ibcgroup;
            TEST_ibcgroup = configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTest_ibcgroup);
            hmkfederatedas = HeaderConstant.hmkfederatedas;
            TEST_hmkfederatedas = configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTest_hmkfederatedas);
            userEmail = HeaderConstant.USER_EMAIL;
            TEST_USER_EMAIL = configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTestUserEmail);
            is_IndRptAvailable = HeaderConstant.is_IndRptAvailable;
            Brand_org_name = HeaderConstant.Brand_org_name;
            TEST_Brand_org_name = configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTestBrand_org_name);
            BROKER_ROLE = ApplicationConstant.BROKER_ROLE;
            BROKER_ORG_IBX = HeaderConstant.BROKER_ORG_IBX;
            BROKER_ORG_AH = HeaderConstant.BROKER_ORG_AH;
            ORG_IBX = configuration.GetValue<string>(AppSettingsConstants.ConfigKeyOrgIbx);
            ORG_AH = configuration.GetValue<string>(AppSettingsConstants.ConfigKeyOrgAh);
            TEST_IsIndRptAvailable = configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<bool>(AppSettingsConstants.ConfigKeyTestIsIndRptAvailable);
            TEST_IsSalesStlsAvailable = configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<bool>(AppSettingsConstants.ConfigKeyTestIsSalesStlsAvailable);
            is_SalesStlsAvailable = HeaderConstant.is_SalesStlsAvailable;
            keyValuePairs = configuration.GetSection(AppSettingsConstants.ConfigKeyOrgGuid).GetChildren().ToDictionary(x => x.Key, x => x.Value);
            _logger = logger;
        }

        public DebugResponseDTO GetUserHeaders(IHeaderDictionary Headers)
        {
            DebugResponseDTO obj = new()
            {
                IsError = false
            };
            UserHeadersDTO headers = new();
            try
            {
                Headers.TryGetValue(HeaderConstant.isLocalConfig, out StringValues isLocalConfigValue);
                Headers.TryGetValue(HeaderConstant.isLocalBrand, out StringValues brandValue);
                if (!string.IsNullOrEmpty(isLocalConfigValue))
                {
                    isLocallyDeployed = true;
                }
                if (!string.IsNullOrEmpty(brandValue) && isLocallyDeployed)
                {
                    switch (brandValue)
                    {
                        case "ibx":
                            TEST_ORGANIZATION_HEADER = ORG_IBX;
                            break;
                        case "ah":
                            TEST_ORGANIZATION_HEADER = ORG_AH;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception)
            {

            }

            try
            {
                StringValues strBrandOrgID = string.Empty;
                StringValues strBrandName = string.Empty;
                StringValues strUserRole = string.Empty;
                StringValues strUserName = string.Empty;
                StringValues strFirstName = string.Empty;
                StringValues strLastName = string.Empty;
                StringValues strSSOToken = string.Empty;
                StringValues strLoginTimeStamp = string.Empty;
                StringValues strMVPUserName = string.Empty;
                StringValues strMVPRole = string.Empty;
                StringValues strMVPLoginId = string.Empty;
                StringValues strBrokerClientId = string.Empty;
                StringValues strIsMbrAvailable = string.Empty;
                StringValues strAuthSecret = string.Empty;
                StringValues stribcgroup = string.Empty;
                StringValues strhmkfederatedas = string.Empty;
                StringValues strUserEmail = string.Empty;
                StringValues strIsIndRptAvailable = string.Empty;
                StringValues strBrand_org_name = string.Empty;
                StringValues strIsSalesStlsAvailable = string.Empty;

                if (isLocallyDeployed)
                {
                    strBrandOrgID = TEST_ORGANIZATION_HEADER;
                    strBrandName = TEST_ORGANIZATION_HEADER;
                    strUserRole = TEST_USER_ROLE_HEADER;
                    strUserName = TEST_USER_NAME;
                    strFirstName = TEST_USER_FIRSTNAME;
                    strLastName = TEST_USER_LASTNAME;
                    strSSOToken = Test_sso_token;
                    strLoginTimeStamp = LAST_LOGIN_TIMESTAMP;
                    strMVPUserName = TEST_MVP_USERNAME;
                    strMVPRole = TEST_MVP_ROLE;
                    strMVPLoginId = TEST_MVP_LOGINID;
                    strBrokerClientId = TEST_BROKER_CLIENT_HEADER;
                    strIsMbrAvailable = Convert.ToString(TEST_BROKER_CLIENT_MEMBERAVIALABILITY_HEADER, CultureInfo.InvariantCulture);
                    stribcgroup = TEST_ibcgroup;
                    strhmkfederatedas = TEST_hmkfederatedas;
                    strUserEmail = TEST_USER_EMAIL;
                    strIsIndRptAvailable = Convert.ToString(TEST_IsIndRptAvailable, CultureInfo.InvariantCulture);
                    strIsSalesStlsAvailable = Convert.ToString(TEST_IsSalesStlsAvailable, CultureInfo.InvariantCulture);

                    strBrand_org_name = TEST_Brand_org_name;

                    if (BROKER_ROLE.Split(",").Contains(GetApplicationBasedRole(Convert.ToString(strUserRole, CultureInfo.InvariantCulture))))
                    {
                        if (strBrand_org_name == BROKER_ORG_IBX)
                        {
                            strBrandOrgID = ORG_IBX;
                        }
                        else if (strBrand_org_name == BROKER_ORG_AH)
                        {
                            strBrandOrgID = ORG_AH;
                        }
                        else
                        {
                            //SC: nothing to handle
                        }
                        strBrandName = strBrandOrgID;
                    }
                }
                else
                {
                    Headers.TryGetValue(USER_ROLE_HEADER, out strUserRole);
                    Headers.TryGetValue(USER_NAME_HEADER, out strUserName);
                    Headers.TryGetValue(USER_FIRST_NAME, out strFirstName);
                    Headers.TryGetValue(USER_LAST_NAME, out strLastName);
                    Headers.TryGetValue(sso_token, out strSSOToken);
                    Headers.TryGetValue(logintimestamp, out strLoginTimeStamp);
                    Headers.TryGetValue(MVP_USERNAME, out strMVPUserName);
                    Headers.TryGetValue(MVP_ROLE, out strMVPRole);
                    Headers.TryGetValue(MVP_LOGINID, out strMVPLoginId);
                    Headers.TryGetValue(BROKER_CLIENT_HEADER, out strBrokerClientId);
                    Headers.TryGetValue(BROKER_CLIENT_MEMBERAVIALABILITY_HEADER, out strIsMbrAvailable);
                    Headers.TryGetValue(AUTH_SECRET, out strAuthSecret);
                    Headers.TryGetValue(ibcgroup, out stribcgroup);
                    Headers.TryGetValue(hmkfederatedas, out strhmkfederatedas);
                    Headers.TryGetValue(userEmail, out strUserEmail);
                    Headers.TryGetValue(Brand_org_name, out strBrand_org_name);
                    Headers.TryGetValue(is_IndRptAvailable, out strIsIndRptAvailable);
                    Headers.TryGetValue(is_SalesStlsAvailable, out strIsSalesStlsAvailable);

                    if (BROKER_ROLE.Split(",").Contains(GetApplicationBasedRole(Convert.ToString(strUserRole, CultureInfo.InvariantCulture))))
                    {
                        if (strBrand_org_name == BROKER_ORG_IBX)
                        {
                            strBrandOrgID = ORG_IBX;
                        }
                        else if (strBrand_org_name == BROKER_ORG_AH)
                        {
                            strBrandOrgID = ORG_AH;
                        }
                        else
                        {
                            //SC: nothing to handle
                        }
                        strBrandName = strBrandOrgID;
                    }
                    else
                    {
                        Headers.TryGetValue(ORGANIZATION_HEADER, out strBrandOrgID);
                        Headers.TryGetValue(ORGANIZATION_HEADER, out strBrandName);
                    }
                }
                headers.BrandOrgID = strBrandOrgID;
                _logger.ReceivedHeaders(strBrandName, strUserRole, strBrand_org_name, keyValuePairs);
                headers.BrandName = keyValuePairs[Convert.ToString(strBrandName, CultureInfo.InvariantCulture)];
                headers.UserRole = GetApplicationBasedRole(Convert.ToString(strUserRole, CultureInfo.InvariantCulture));
                headers.AllUserRole = strUserRole;
                headers.UserName = strUserName;
                headers.FirtName = strFirstName;
                headers.LastName = strLastName;
                headers.SSOToken = strSSOToken;
                headers.Cookies = Headers.Keys.FirstOrDefault(x => x.Equals("COOKIE", StringComparison.InvariantCultureIgnoreCase)) ?? String.Empty;
                headers.LoginTimeStamp = strLoginTimeStamp;
                headers.MVPUserName = strMVPUserName;
                headers.MVPRole = strMVPRole;
                headers.MVPLoginId = strMVPLoginId;
                headers.BrokerClientId = strBrokerClientId;
                headers.IsMbrAvailable = Convert.ToBoolean(strIsMbrAvailable, CultureInfo.InvariantCulture);
                headers.AuthSecret = strAuthSecret;
                headers.ibcgroup = stribcgroup;
                headers.hmkfederatedas = strhmkfederatedas;
                headers.UserEmail = strUserEmail;
                headers.Brandorgname = strBrand_org_name;
                headers.IsIndRptAvailable = Convert.ToBoolean(strIsIndRptAvailable, CultureInfo.InvariantCulture);
                headers.IsStlsRptAvailable = Convert.ToBoolean(strIsSalesStlsAvailable, CultureInfo.InvariantCulture);

                obj.responseObj = headers;
            }
            catch (Exception ex)
            {
                obj.IsError = true;
                obj.ErrorMessage = Convert.ToString(ex, CultureInfo.InvariantCulture);
            }
            return obj;
        }

        private static string GetApplicationBasedRole(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                roleName = roleName.Replace("\"", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                                    .Replace("\\", string.Empty, StringComparison.InvariantCultureIgnoreCase);
                List<string> systemRoles = ApplicationConstant.SYSTEM_SUPPORTED_ROLES.Split(",").ToList();
                foreach (string role in roleName.Split(",").ToList())
                {
                    if (systemRoles.Contains(role.Trim()))
                    {
                        return role.Trim();
                    }
                }
            }
            return string.Empty;
        }

        //public DebugResponseDTO GetOrgHeader(IHeaderDictionary Headers)
        //{
        //    DebugResponseDTO obj = new DebugResponseDTO();
        //    obj.IsError = false;
        //    try
        //    {
        //        if (!_configuration.GetValue<bool>("isLocallyDeployed"))
        //        {
        //            obj.responseObj = Headers.Keys.Any(key => key == _configuration.GetValue<string>("ORGANIZATION_HEADER"))
        //               ? Headers[_configuration.GetValue<string>("ORGANIZATION_HEADER")].ToString()
        //               : string.Empty;
        //        }
        //        else
        //        {
        //            obj.responseObj = Headers.Keys.Any(key => key == _configuration.GetValue<string>("ORGANIZATION_HEADER"))
        //              ? Headers[_configuration.GetValue<string>("ORGANIZATION_HEADER")].ToString()
        //              : _configuration.GetValue<string>("TEST_ORGANIZATION_HEADER");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        obj.IsError = true;
        //        obj.ErrorMessage = ex.ToString();
        //    }
        //    return obj;
        //}

        public DebugResponseDTO GetUserNameFromHeaders(IHeaderDictionary Headers)
        {
            DebugResponseDTO obj = new()
            {
                IsError = false
            };
            UserHeadersDTO headers = new();
            try
            {
                StringValues strUSER_ROLE_HEADER = string.Empty;
                StringValues strUSER_NAME_HEADER = string.Empty;
                StringValues strBROKER_CLIENT_HEADER = string.Empty;
                StringValues strBROKER_CLIENT_MEMBERAVIALABILITY_HEADER = "false";

                if (isLocallyDeployed)
                {
                    strUSER_ROLE_HEADER = TEST_USER_ROLE_HEADER;
                    strUSER_NAME_HEADER = TEST_USER_NAME;
                    strBROKER_CLIENT_HEADER = TEST_BROKER_CLIENT_HEADER;
                    strBROKER_CLIENT_MEMBERAVIALABILITY_HEADER = Convert.ToString(TEST_BROKER_CLIENT_MEMBERAVIALABILITY_HEADER, CultureInfo.InvariantCulture);
                }
                else
                {
                    Headers.TryGetValue(USER_ROLE_HEADER, out strUSER_ROLE_HEADER);
                    Headers.TryGetValue(USER_NAME_HEADER, out strUSER_NAME_HEADER);
                    Headers.TryGetValue(BROKER_CLIENT_HEADER, out strBROKER_CLIENT_HEADER);
                    Headers.TryGetValue(BROKER_CLIENT_MEMBERAVIALABILITY_HEADER, out strBROKER_CLIENT_MEMBERAVIALABILITY_HEADER);
                }

                headers.UserRole = GetApplicationBasedRole(Convert.ToString(strUSER_ROLE_HEADER, CultureInfo.InvariantCulture));
                headers.UserName = strUSER_NAME_HEADER;
                headers.BrokerClientId = strBROKER_CLIENT_HEADER;
                headers.IsMbrAvailable = Convert.ToBoolean(strBROKER_CLIENT_MEMBERAVIALABILITY_HEADER, CultureInfo.InvariantCulture);
                obj.responseObj = headers;
            }
            catch (Exception ex)
            {
                obj.IsError = true;
                obj.ErrorMessage = Convert.ToString(ex, CultureInfo.InvariantCulture);
            }
            return obj;
        }

        public UserAccessInfoDTO CreateDummyAccessInfo(string userName)
        {
            string userAccessData = string.Empty;
            if (userName.Equals("pen-david-1", StringComparison.InvariantCultureIgnoreCase) ||
                userName.Equals("pen-garrett-1", StringComparison.InvariantCultureIgnoreCase) ||
                userName.Equals("test-pan-1", StringComparison.InvariantCultureIgnoreCase))
            {
                userAccessData = @"{""allClients"": false,""currentVersion"": ""1.0"",""features"": [{""featureId"": 15,""featureName"": ""Member Information"",""subFeatures"": [{""subFeatureId"": 80,""subFeatureName"": ""Access"",""permissionCode"": ""MBR_VIEW"",""hasAccess"": true,""version"": ""1.0"",""isPHI"": false,""clients"": [{""clientId"": ""999563"",""accounts"": [{""accountId"":""3011024"",""subAccounts"":[{""subAccountId"":""3156278""}]}],}]}]}],""selectedClientIds"": [{""clientId"": ""999563"",""clientName"": ""IBC (PA) Consumer ACA""}],""userName"": ""pen-david-1""}";
            }

            UserAccessInfoDTO userAccess = JsonConvert.DeserializeObject<UserAccessInfoDTO>(userAccessData);

            return userAccess;
        }

        public bool IsBroker(UserHeadersDTO userHeaders)
        {
            return userHeaders.UserRole.Contains(UserRoleConstant.BrokerConsultant, StringComparison.InvariantCultureIgnoreCase)
                 || userHeaders.UserRole.Contains(UserRoleConstant.BrokerConsultant1, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
