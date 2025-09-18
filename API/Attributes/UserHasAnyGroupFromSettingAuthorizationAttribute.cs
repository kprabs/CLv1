//using AuthServiceNewRef;
//using CoreLib.API.Helpers;
//using CoreLib.Application.Common;
//using CoreLib.Application.Common.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Microsoft.Extensions.Configuration;

//namespace CoreLib.API.Attributes
//{

//    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
//    public class UserHasAnyGroupFromSettingAuthorizationAttribute : Attribute, IAuthorizationFilter
//    {
//        public string _settingName { get; set; }
//        public UserHasAnyGroupFromSettingAuthorizationAttribute(string settingName)
//        {
//            _settingName = settingName;

//        }
//        public void OnAuthorization(AuthorizationFilterContext context)
//        {
//            if (context != null)
//            {
//                var authorizationService = context.HttpContext.RequestServices.GetService(typeof(IAuthorizationNugetServices)) as IAuthorizationNugetServices;
//                var configuration = context.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
//                var cacheHelper = context.HttpContext.RequestServices.GetService(typeof(ICacheHelper)) as ICacheHelper;
//                if (configuration != null && configuration.GetValue<bool>("IS_AUTHORIZATION_ENABLED"))
//                {
//                    UserDefaultDTO userDefaultDTO = AuthorizationAttributeHelper.SetDefaultParams(context.HttpContext, configuration);
//                    _settingName = string.IsNullOrEmpty(configuration.GetValue<string>(_settingName))
//                                       ? configuration.GetValue<string>(_settingName) ?? String.Empty
//                                       : _settingName;
//                    object cachedObject;
//                    if (!cacheHelper.TryGetCachedObject(userDefaultDTO.GivenName ?? String.Empty, out cachedObject))
//                    {
//                        var userDetails = authorizationService.ValidateAuthorization(userDefaultDTO);
//                        cachedObject = cacheHelper.SetCachedObject(userDefaultDTO.GivenName ?? String.Empty, userDetails);
//                    }
//                    UserAuthorizationAccessDTO authorizationObject = (UserAuthorizationAccessDTO)cachedObject;
//                    if (!authorizationService.HasAnyGroupFromSetting(_settingName))
//                    {
//                        context.Result = new ForbidResult();
//                    }
//                }
//            }
//        }
//    }
//}
