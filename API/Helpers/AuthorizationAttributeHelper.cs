using CoreLib.Application.Common.Constants;
using CoreLib.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace CoreLib.API.Helpers
{
    public static class AuthorizationAttributeHelper
    {        
        public static UserDefaultDTO SetDefaultParams(HttpContext httpContext, IConfiguration configuration)
        {
            UserDefaultDTO obj = new()
            {
                ApplicationCode = configuration.GetValue<string>(AppSettingsConstants.ConfigKeyAuthAppCode),
                AuthorizationMode = configuration.GetValue<string>(AppSettingsConstants.ConfigKeyAuthMode)
            };
            if (configuration.GetValue<bool>(AppSettingsConstants.ConfigKeyIsLocallyDeployed))
            {
                obj.EmailHeader = configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTestUserEmail);
                obj.GivenName = configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTestUserName);
                obj.FirstNameHeader = configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTestFirstName);
                obj.LastNameHeader = configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTestLastName);
                obj.SessionProcessingUserName = configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTestUserName);
            }
            else
            {
                string assignedGroup = GetGroupForHeader(
                        httpContext.Request.Headers.Keys.Any(x => x == HeaderConstant.USER_ROLE_HEADER) ? httpContext.Request.Headers[HeaderConstant.USER_ROLE_HEADER].ToString() : string.Empty,
                        httpContext.Request.Headers.Keys.Any(x => x == HeaderConstant.ORGANIZATION_HEADER) ? httpContext.Request.Headers[HeaderConstant.ORGANIZATION_HEADER].ToString() : string.Empty);
                obj.EmailHeader = httpContext.Request.Headers.Keys.Any(x => x == HeaderConstant.USER_EMAIL) ? httpContext.Request.Headers[HeaderConstant.USER_EMAIL].ToString() : string.Empty;

                obj.GivenName = httpContext.Request.Headers.Keys.Any(x => x == HeaderConstant.USER_NAME_HEADER) ? httpContext.Request.Headers[HeaderConstant.USER_NAME_HEADER].ToString() : string.Empty;

                obj.FirstNameHeader = httpContext.Request.Headers.Keys.Any(x => x == HeaderConstant.USER_FIRST_NAME) ? httpContext.Request.Headers[HeaderConstant.USER_FIRST_NAME].ToString() : string.Empty;
                obj.LastNameHeader = httpContext.Request.Headers.Keys.Any(x => x == HeaderConstant.USER_LAST_NAME) ? httpContext.Request.Headers[HeaderConstant.USER_LAST_NAME].ToString() : string.Empty;
                obj.SessionProcessingUserName = httpContext.Request.Headers.Keys.Any(x => x == HeaderConstant.USER_NAME_HEADER) ? httpContext.Request.Headers[HeaderConstant.USER_NAME_HEADER].ToString() : string.Empty;
                obj.GroupSetHeader = assignedGroup.Split(",").First(x => x.StartsWith(ApplicationConstant.ORGANIZATION_CODE, StringComparison.InvariantCultureIgnoreCase));
                obj.GroupsHeader = assignedGroup;
            }

            obj.LoginSystemCode = configuration.GetValue<string>(AppSettingsConstants.ConfigKeyAuthLoginSystemCode);
            obj.RefreshMillis = ApplicationConstant.AUTH_REFRESH_MILLIS;
            obj.SessionProcessingUserSystem = ApplicationConstant.AUTH_SESSION_PROCESSING_USER_NAME;

            return obj;
        }

        public static UserDefaultDTO SetDefaultParams(IConfiguration configuration, UserDefaultDTO userDetails)
        {
            userDetails.ApplicationCode = configuration.GetValue<string>(AppSettingsConstants.ConfigKeyAuthAppCode);
            userDetails.AuthorizationMode = configuration.GetValue<string>(AppSettingsConstants.ConfigKeyAuthMode);
            userDetails.LoginSystemCode = configuration.GetValue<string>(AppSettingsConstants.ConfigKeyAuthLoginSystemCode);
            userDetails.RefreshMillis = ApplicationConstant.AUTH_REFRESH_MILLIS;
            userDetails.SessionProcessingUserSystem = ApplicationConstant.AUTH_SESSION_PROCESSING_USER_NAME;
            return userDetails;
        }

        public static UserDefaultDTO SetDefaultParams(HttpContext context, IConfiguration configuration, string newRole)
        {
            UserDefaultDTO obj = new()
            {
                ApplicationCode = configuration.GetValue<string>(AppSettingsConstants.ConfigKeyAuthAppCode),
                AuthorizationMode = configuration.GetValue<string>(AppSettingsConstants.ConfigKeyAuthMode)
            };
            if (configuration.GetValue<bool>(AppSettingsConstants.ConfigKeyIsLocallyDeployed))
            {
                obj.EmailHeader = configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTestUserEmail);
                if (newRole == ApplicationConstant.FULLY_QUALIFIED_ADMIN_ROLE_IBX
                                || newRole == ApplicationConstant.FULLY_QUALIFIED_ADMIN_ROLE_AH
                                || newRole == ApplicationConstant.ADMIN_ROLE)
                {
                    obj.GivenName = ApplicationConstant.DUMMY_ADMIN;
                }
                else if (ApplicationConstant.FULLY_QUALIFIED_BROKER_ROLE_IBX.Split('|').Contains(newRole)
                            || ApplicationConstant.FULLY_QUALIFIED_BROKER_ROLE_AH.Split('|').Contains(newRole)
                            || ApplicationConstant.BROKER_ROLE.Split(',').Contains(newRole))
                {
                    obj.GivenName = ApplicationConstant.DUMMY_BROKER;
                }
                else if (ApplicationConstant.FULLY_QUALIFIED_STOPLOSS_ROLE_IBX.Split('|').Contains(newRole)
                            || ApplicationConstant.FULLY_QUALIFIED_STOPLOSS_ROLE_AH.Split('|').Contains(newRole)
                            || ApplicationConstant.STOPLOSS_ROLE.Split(',').Contains(newRole))
                {
                    obj.GivenName = ApplicationConstant.DUMMY_STOP_LOSS;
                }
                else
                {
                    obj.GivenName = configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTestUserName);
                }

                obj.FirstNameHeader = configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTestFirstName);
                obj.LastNameHeader = configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTestLastName);
                obj.SessionProcessingUserName = configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTestUserName);
                obj.GroupsHeader = newRole;
                obj.GroupSetHeader = newRole.Split(",").First(x => x.StartsWith(ApplicationConstant.ORGANIZATION_CODE, StringComparison.InvariantCultureIgnoreCase));
                obj.AuthorizationMode = ApplicationConstant.AUTH_LOGIN_SYSTEM_CODE_FORGEROCK;
                /**FOR MVP USER**/
                obj.MVPUserName = configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTestMvpUserName);
                obj.MVPLoginId = configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTestMvpLoginId);
                obj.MVPRole = configuration.GetSection(AppSettingsConstants.ConfigSectionLocal).GetValue<string>(AppSettingsConstants.ConfigKeyTestMvpRole);
             }
            else
            {
                string assignedGroup = GetGroupForHeader(
                      context.Request.Headers.Keys.Any(x => x == HeaderConstant.USER_ROLE_HEADER) ?
                          context.Request.Headers[HeaderConstant.USER_ROLE_HEADER].ToString() : string.Empty,
                      context.Request.Headers.Keys.Any(x => x == HeaderConstant.ORGANIZATION_HEADER) ?
                          context.Request.Headers[HeaderConstant.ORGANIZATION_HEADER].ToString() : string.Empty);
                obj.EmailHeader = context.Request.Headers.Keys.Any(static x => x == HeaderConstant.USER_EMAIL) ? 
                                    context.Request.Headers[HeaderConstant.USER_EMAIL].ToString() : string.Empty;

                if (newRole == ApplicationConstant.FULLY_QUALIFIED_ADMIN_ROLE_IBX
                                || newRole == ApplicationConstant.FULLY_QUALIFIED_ADMIN_ROLE_AH
                                || newRole == ApplicationConstant.ADMIN_ROLE)
                {
                    obj.GivenName = ApplicationConstant.DUMMY_ADMIN;
                }
                else if (ApplicationConstant.FULLY_QUALIFIED_BROKER_ROLE_IBX.Split('|').Contains(newRole)
                            || ApplicationConstant.FULLY_QUALIFIED_BROKER_ROLE_AH.Split('|').Contains(newRole)
                            || ApplicationConstant.BROKER_ROLE.Split(',').Contains(newRole))
                {
                    obj.GivenName = ApplicationConstant.DUMMY_BROKER;
                }
                else if (ApplicationConstant.FULLY_QUALIFIED_STOPLOSS_ROLE_IBX.Split('|').Contains(newRole)
                            || ApplicationConstant.FULLY_QUALIFIED_STOPLOSS_ROLE_AH.Split('|').Contains(newRole)
                            || ApplicationConstant.STOPLOSS_ROLE.Split(',').Contains(newRole))
                {
                    obj.GivenName = ApplicationConstant.DUMMY_STOP_LOSS;
                }
                else if (context.Request.Headers.Keys.Any(x => x == HeaderConstant.USER_NAME_HEADER))
                {
                    obj.GivenName = context.Request.Headers[HeaderConstant.USER_NAME_HEADER].ToString();
                }
                else
                {
                    obj.GivenName = string.Empty;
                }

                obj.FirstNameHeader = context.Request.Headers.Keys.Any(x => x == HeaderConstant.USER_FIRST_NAME) ? context.Request.Headers[HeaderConstant.USER_FIRST_NAME].ToString() : string.Empty;
                obj.LastNameHeader = context.Request.Headers.Keys.Any(x => x == HeaderConstant.USER_LAST_NAME) ? context.Request.Headers[HeaderConstant.USER_LAST_NAME].ToString() : string.Empty;
                obj.SessionProcessingUserName = context.Request.Headers.Keys.Any(x => x == HeaderConstant.USER_NAME_HEADER) ? context.Request.Headers[HeaderConstant.USER_NAME_HEADER].ToString() : string.Empty;

                obj.GroupsHeader = newRole;
                obj.GroupSetHeader = newRole.Split(",").First(x => x.StartsWith(ApplicationConstant.ORGANIZATION_CODE, StringComparison.InvariantCultureIgnoreCase));
                obj.AuthorizationMode = ApplicationConstant.AUTH_LOGIN_SYSTEM_CODE_FORGEROCK;
                /**FOR MVP USER**/
                obj.MVPUserName = context.Request.Headers.Keys.Any(x => x == HeaderConstant.MVP_USERNAME) ? context.Request.Headers[HeaderConstant.MVP_USERNAME].ToString() : string.Empty;
                obj.MVPLoginId = context.Request.Headers.Keys.Any(x => x == HeaderConstant.MVP_LOGINID) ? context.Request.Headers[HeaderConstant.MVP_LOGINID].ToString() : string.Empty;
                obj.MVPRole = context.Request.Headers.Keys.Any(x => x == HeaderConstant.MVP_ROLE) ? context.Request.Headers[HeaderConstant.MVP_ROLE].ToString() : string.Empty;
            }

            obj.LoginSystemCode = configuration.GetValue<string>(AppSettingsConstants.ConfigKeyAuthLoginSystemCode);
            obj.RefreshMillis = ApplicationConstant.AUTH_REFRESH_MILLIS;
            obj.SessionProcessingUserSystem = ApplicationConstant.AUTH_SESSION_PROCESSING_USER_NAME;

            return obj;
        }

        public static string GetGroupForHeader(string newRole, string orgName)
        {
            return ApplicationConstant.ORGANIZATION_CODE.Trim() + orgName.Trim() + ApplicationConstant.ROLE_CODE.Trim() + newRole.Trim();
        }
    }
}
