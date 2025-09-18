using CoreLib.Application.Common.Constants;
using CoreLib.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace CoreLib.Application.Common.Utility
{
    public class HeaderHepler(IConfiguration configuration)
    {
        public DebugResponseDTO GetOrgHeader(IHeaderDictionary Headers)
        {
            DebugResponseDTO obj = new()
            {
                IsError = false
            };
            try
            {
                if (!configuration.GetValue<bool>(AppSettingsConstants.ConfigKeyIsLocallyDeployed))
                {
                    obj.responseObj = Headers.Keys.Any(key => key == HeaderConstant.ORGANIZATION_HEADER)
                       ? Headers[HeaderConstant.ORGANIZATION_HEADER].ToString()
                       : string.Empty;
                }
                else
                {
                    obj.responseObj = Headers.Keys.Any(key => key == HeaderConstant.ORGANIZATION_HEADER)
                      ? Headers[HeaderConstant.ORGANIZATION_HEADER].ToString()
                      : configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTestOrganizationHeader);
                }
            }
            catch (Exception ex)
            {
                obj.IsError = true;
                obj.ErrorMessage = ex.ToString();
            }
            return obj;
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
                if (!configuration.GetValue<bool>(AppSettingsConstants.ConfigKeyIsLocallyDeployed))
                {
                    headers.BrandOrgID = Headers.Keys.Any(key => key == HeaderConstant.ORGANIZATION_HEADER)
                       ? Headers[HeaderConstant.ORGANIZATION_HEADER].ToString()
                       : string.Empty;
                    headers.BrandName = Headers.Keys.Any(key => key == HeaderConstant.ORGANIZATION_HEADER)
                       ? Headers[HeaderConstant.ORGANIZATION_HEADER].ToString().GetOrganizationNameByOrgGuid()
                        : string.Empty;
                    headers.UserRole = Headers.Keys.Any(key => key == HeaderConstant.USER_ROLE_HEADER)
                        ? Headers[HeaderConstant.USER_ROLE_HEADER].ToString().Replace("\"", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                                                                             .Replace("\\", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                        : string.Empty;
                    headers.UserName = Headers.Keys.Any(key => key == HeaderConstant.USER_NAME_HEADER)
                        ? Headers[HeaderConstant.USER_NAME_HEADER].ToString()
                        : string.Empty;
                    headers.Cookies = Headers.Keys.FirstOrDefault(x => x.Equals("COOKIE", StringComparison.InvariantCultureIgnoreCase)) ?? String.Empty;
                    headers.MVPUserName = Headers.Keys.Any(key => key == HeaderConstant.MVP_USERNAME)
                        ? Headers[HeaderConstant.MVP_USERNAME].ToString()
                        : string.Empty;
                    headers.MVPRole = Headers.Keys.Any(key => key == HeaderConstant.MVP_ROLE)
                        ? Headers[HeaderConstant.MVP_ROLE].ToString()
                        : string.Empty;
                    headers.MVPLoginId = Headers.Keys.Any(key => key == HeaderConstant.MVP_LOGINID)
                        ? Headers[HeaderConstant.MVP_LOGINID].ToString()
                        : string.Empty;
                }
                else
                {

                    headers.BrandOrgID = Headers.Keys.Any(key => key == HeaderConstant.ORGANIZATION_HEADER)
                      ? Headers[HeaderConstant.ORGANIZATION_HEADER].ToString()
                      : configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTestOrganizationHeader);
                    headers.BrandName = Headers.Keys.Any(key => key == HeaderConstant.ORGANIZATION_HEADER)
                       ? Headers[HeaderConstant.ORGANIZATION_HEADER].ToString().GetOrganizationNameByOrgGuid()
                       : configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTestOrganizationHeader).GetOrganizationNameByOrgGuid();
                    headers.UserRole = Headers.Keys.Any(key => key == HeaderConstant.USER_ROLE_HEADER)
                       ? Headers[HeaderConstant.USER_ROLE_HEADER].ToString().Replace("\"", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                                                                            .Replace("\\", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                       : configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTestUserRole).Replace("\"", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                                                                                                    .Replace("\\", string.Empty, StringComparison.InvariantCultureIgnoreCase);
                    headers.UserName = Headers.Keys.Any(key => key == HeaderConstant.USER_NAME_HEADER)
                       ? Headers[HeaderConstant.USER_NAME_HEADER].ToString()
                       : configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTestUserName);
                    headers.Cookies = Headers.Keys.FirstOrDefault(x => x.Equals("COOKIE", StringComparison.InvariantCultureIgnoreCase)) ?? String.Empty;
                    headers.MVPUserName = Headers.Keys.Any(key => key == HeaderConstant.MVP_USERNAME)
                       ? Headers[HeaderConstant.MVP_USERNAME].ToString()
                       : configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTestMvpUserName);
                    headers.MVPRole = Headers.Keys.Any(key => key == HeaderConstant.MVP_ROLE)
                       ? Headers[HeaderConstant.MVP_ROLE].ToString()
                       : configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTestMvpRole);
                    headers.MVPLoginId = Headers.Keys.Any(key => key == HeaderConstant.MVP_LOGINID)
                       ? Headers[HeaderConstant.MVP_LOGINID].ToString()
                       : configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTestMvpLoginId);
                }
                obj.responseObj = headers;
            }
            catch (Exception ex)
            {
                obj.IsError = true;
                obj.ErrorMessage = ex.ToString();
            }
            return obj;
        }
    }
}
