//using CoreLib.Application.Common.Constants;
//using CoreLib.Application.Common.Interfaces;
//using CoreLib.Application.Common.Models;
//using CoreLib.Application.Common.SqlEntities;
//using CoreLib.Application.Common.Utility;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using Newtonsoft.Json;
//using System.Text;
//#pragma warning disable S2259

//namespace CoreLib.API.Attributes
//{
//    static partial class CreateUserQueryLogMessages
//    {
//        [LoggerMessage(Level = LogLevel.Information, EventId = 1, EventName = "HeadersAndDuration", Message = "User headers: {userHeadersDTO}  (+{elapsedMs})")]
//        internal static partial void HeadersAndDuration(this ILogger logger, double elapsedMs, UserHeadersDTO userHeadersDTO);
//        [LoggerMessage(Level = LogLevel.Information, EventId = 2, EventName = "DatabaseMethodlElapsed", Message = "Database method: {method} elapsed {elapsedMs}")]
//        internal static partial void DatabaseMethodlElapsed(this ILogger logger, string method, double elapsedMs);
//    }

//    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
//    public class CanUserAccessActionAttribute(string accessTypes) : Attribute, IAuthorizationFilter
//    {
//        public string AccessTypes { get; set; } = accessTypes;

//        public void OnAuthorization(AuthorizationFilterContext context)
//        {
//            if (context != null)
//            {
//                DateTime dt = DateTime.UtcNow;

//                BaseMessage baseMessage = new();

//                var headerReaderService = context.HttpContext.RequestServices.GetService(typeof(IHeaderReaderService)) as IHeaderReaderService;
//                var configuration = context.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
//                var payloadService = context.HttpContext.RequestServices.GetService(typeof(IAuthPayloadService)) as IAuthPayloadService;
//                var sqlRepository = context.HttpContext.RequestServices.GetService(typeof(ISqlRepository)) as ISqlRepository;
//                var logger = context.HttpContext.RequestServices.GetService(typeof(ILogger<CanUserAccessActionAttribute>)) as ILogger;

//                var request = context.HttpContext.Request;
//                if (!request.Body.CanSeek)
//                {
//                    // We only do this if the stream isn't *already* seekable,
//                    // as EnableBuffering will create a new stream instance
//                    // each time it's called
//                    request.EnableBuffering();
//                }

//                request.Body.Position = 0;
//                var reader = new StreamReader(request.Body, Encoding.UTF8);
//                var rawBody = reader.ReadToEndAsync().ConfigureAwait(false);
//                request.Body.Position = 0;
//                logger.LogInformation($"Request Raw Body ::  {rawBody.GetAwaiter().GetResult()}");
//                logger.LogInformation($"Headers Received :: {JsonConvert.SerializeObject(context.HttpContext.Request.Headers)}");
//                var headerObj = headerReaderService.GetUserNameFromHeaders(context.HttpContext.Request.Headers);
//                logger.LogInformation($"Header Object :: {JsonConvert.SerializeObject(headerObj)}");
//                if (headerObj == null)
//                {
//                    baseMessage.returnCode = StatusCodes.Status401Unauthorized.ToString();
//                    baseMessage.returnCodeDescription = "User unauthorized for action - Improper Headers";
//                    context.Result = new UnauthorizedObjectResult(ApiResponseWrapper.ResponseWrapper<String>(String.Empty, baseMessage));
//                    return;
//                }

//                var userHeader = (UserHeadersDTO)headerObj.responseObj;
//                logger.LogInformation($"User Header Object :: {JsonConvert.SerializeObject(userHeader)}");
//                logger?.HeadersAndDuration((DateTime.UtcNow - dt).TotalMilliseconds, userHeader);
//                dt = DateTime.UtcNow;

//                if (string.IsNullOrEmpty(userHeader.UserName))
//                {
//                    baseMessage.returnCode = StatusCodes.Status401Unauthorized.ToString();
//                    baseMessage.returnCodeDescription = "User unauthorized for action - No Username";
//                    context.Result = new UnauthorizedObjectResult(ApiResponseWrapper.ResponseWrapper<String>(String.Empty, baseMessage));
//                    return;
//                }

//                try
//                {
//                    //CHECK CLIENT, ACCOUNT, SUBACCOUNT HERE
//                    var authClientData = payloadService.ReadPayloadByService(request.Path.Value, rawBody.GetAwaiter().GetResult());
//                    logger.LogInformation($"Member Transaction request :: {0}", JsonConvert.SerializeObject(authClientData));

//                    AccessTypes = !string.IsNullOrEmpty(configuration.GetSection("PermissionCodes").GetValue<string>(AccessTypes))
//                                           ? configuration.GetSection("PermissionCodes").GetValue<string>(AccessTypes) ?? String.Empty
//                                           : AccessTypes;

//                    //CHECK BROKER HERE
//                    bool isBroker = ApplicationConstant.BROKER_ROLE.Split(',').Contains(userHeader.UserRole);
//                    List<ClassifiedInstancesEntity> objClassifiedInstances = new();
//                    UserAccessInfoDTO userAccess = new(); //call service here

//                    if (isBroker)
//                    {
//                        if ((bool)!userHeader.IsMbrAvailable)
//                        {
//                            baseMessage.returnCode = StatusCodes.Status401Unauthorized.ToString();
//                            baseMessage.returnCodeDescription = "User unauthorized for action -- Broker";
//                            context.Result = new UnauthorizedObjectResult(ApiResponseWrapper.ResponseWrapper<String>(String.Empty, baseMessage));
//                            return;
//                        }

//                        dt = DateTime.UtcNow;

//                        objClassifiedInstances = sqlRepository.GetClassifiedInstancesByClientId(userHeader.BrokerClientId);

//                        logger?.DatabaseMethodlElapsed("GetClassifiedInstancesByClientId", (DateTime.UtcNow - dt).TotalMilliseconds);
//                        dt = DateTime.UtcNow;

//                        /** RESTRUCTURE DATA TO USERACCESSINFODTO **/
//                        var brokerData = UserAccessStructForBroker(objClassifiedInstances);

//                        userAccess.Features.Add(brokerData);
//                    }
//                    else
//                    {

//                        dt = DateTime.UtcNow;

//                        var featureData = sqlRepository.GetUserInfoOnPermissionCodeToList(Convert.ToString(ApplicationConstant.SYSTEM_ID), userHeader.UserName, AccessTypes);

//                        logger.LogInformation($"Feature Data :: {0}", JsonConvert.SerializeObject(featureData));
//                        logger?.DatabaseMethodlElapsed("GetUserInfoOnPermissionCodeToList", (DateTime.UtcNow - dt).TotalMilliseconds);
//                        dt = DateTime.UtcNow;

//                        /** RESTRUCTURE DATA TO USERACCESSINFODTO **/
//                        var structData = UserAccessStruct(featureData);

//                        userAccess.Features = structData.ToList();
//                    }
//                    logger.LogInformation($"userAccess.Features values :: {0}", JsonConvert.SerializeObject(userAccess));

//                    if (!userAccess.Features.Any(x => x.SubFeatures.Any(y => y.PermissionCode == AccessTypes && y.HasAccess == true)))
//                    {
//                        baseMessage.returnCode = StatusCodes.Status401Unauthorized.ToString();
//                        baseMessage.returnCodeDescription = "User unauthorized for action - no feature has permission";
//                        context.Result = new UnauthorizedObjectResult(ApiResponseWrapper.ResponseWrapper<String>(String.Empty, baseMessage));
//                        return;
//                    }
//                    if (!userAccess.Features.Any(x => x.SubFeatures.Any(y => y.PermissionCode == AccessTypes && y.HasAccess == true && y.Clients.Any(z => Convert.ToInt32(z.ClientId) == Convert.ToInt32(authClientData.clientId)))))
//                    {
//                        baseMessage.returnCode = StatusCodes.Status401Unauthorized.ToString();
//                        baseMessage.returnCodeDescription = "User unauthorized for ClientId";
//                        context.Result = new UnauthorizedObjectResult(ApiResponseWrapper.ResponseWrapper<String>(String.Empty, baseMessage));
//                        return;
//                    }

//                    if (!authClientData.onlyClientFilter)
//                    {
//                        if (!userAccess.Features.Any(x => x.SubFeatures.Any(y => y.PermissionCode == AccessTypes && y.HasAccess == true && y.Clients.Any(z => z.ClientId == authClientData.clientId && (!(z.Accounts.Count == 0) && z.Accounts.Any(xx => authClientData.accounts.Contains(xx.AccountId)))))))
//                        {
//                            baseMessage.returnCode = StatusCodes.Status401Unauthorized.ToString();
//                            baseMessage.returnCodeDescription = "User unauthorized for AccountId";
//                            context.Result = new UnauthorizedObjectResult(ApiResponseWrapper.ResponseWrapper<String>(String.Empty, baseMessage));
//                            return;
//                        }
//                    }

//                    if (!authClientData.onlyClientFilter)
//                    {
//                        if (!userAccess.Features.Any(x => x.SubFeatures.Any(y => y.PermissionCode == AccessTypes && y.HasAccess == true && y.Clients.Any(z => z.ClientId == authClientData.clientId && (z.Accounts.Count == 0 || z.Accounts.Any(xx => authClientData.accounts.Contains(xx.AccountId) && (!(xx.SubAccounts.Count == 0) && xx.SubAccounts.Any(yy => authClientData.subAccounts.Contains(yy.SubAccountId)))))))))
//                        {
//                            baseMessage.returnCode = StatusCodes.Status401Unauthorized.ToString();
//                            baseMessage.returnCodeDescription = "User unauthorized for SubAccountId";
//                            context.Result = new UnauthorizedObjectResult(ApiResponseWrapper.ResponseWrapper<String>(String.Empty, baseMessage));
//                            return;
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {
//                    baseMessage.returnCode = StatusCodes.Status401Unauthorized.ToString();
//                    baseMessage.returnCodeDescription = $"User unauthorized for action Exception {ex.StackTrace}";
//                    context.Result = new UnauthorizedObjectResult(ApiResponseWrapper.ResponseWrapper<String>(String.Empty, baseMessage));
//                    return;
//                }
//            }
//        }

//        private FeatureAccessInfoDTO UserAccessStructForBroker(List<ClassifiedInstancesEntity> objClassifiedInstances)
//        {
//            var structDataForBroker = from client in objClassifiedInstances
//                                      group client by client.ClientId into clientGrp
//                                      select new ClientAccessInfoDTO
//                                      {
//                                          ClientId = clientGrp.Key,
//                                          Accounts = (from account in clientGrp.AsEnumerable()
//                                                      where Convert.ToInt32(account.AccountId) != 0
//                                                      group account by account.AccountId into accountGrp
//                                                      select new AccountAccessInfoDTO
//                                                      {
//                                                          AccountId = accountGrp.Key,
//                                                          SubAccounts = (from subaccount in
//                                                          accountGrp.AsEnumerable()
//                                                                         where
//                                                                         Convert.ToInt32(subaccount.SubAccountId) != 0
//                                                                         select new SubAccountAccessInfoDTO
//                                                                         {
//                                                                             SubAccountId = subaccount.SubAccountId
//                                                                         }).ToList(),
//                                                      }).ToList()
//                                      };

//            //FORM FEATURE OBJECT HERE
//            FeatureAccessInfoDTO objFeature = new()
//            {
//                FeatureId = 0,
//                FeatureName = "Member Eligibility",
//                SubFeatures = [ new () {
//                                    SubFeatureId = 0,
//                                    SubFeatureName = "All Accounts",
//                                    PermissionCode = "MBR_VIEW",
//                                    HasAccess=true,
//                                    Clients=structDataForBroker.ToList()
//                                } ]
//            };

//            return objFeature;
//        }

//        private List<FeatureAccessInfoDTO> UserAccessStruct(List<UserInfoOnPermissionCode> featureData)
//        {
//            var structData = from client in featureData
//                             group client by client.ClientId into clientGrp
//                             select new FeatureAccessInfoDTO
//                             {
//                                 FeatureId = Convert.ToInt32(clientGrp.First().FeatureId),
//                                 FeatureName = clientGrp.First().FeatureName,
//                                 SubFeatures = [ new() {
//                                                SubFeatureId = Convert.ToInt32(clientGrp.First().SubFeatureId),
//                                                SubFeatureName = clientGrp.First().SubFeatureName,
//                                                PermissionCode = clientGrp.First().PermissionCode,
//                                                HasAccess=Convert.ToBoolean(clientGrp.First().HasAccess),
//                                                Clients = [
//                                                    new ClientAccessInfoDTO {
//                                                        ClientId = clientGrp.Key,
//                                                        Accounts = (from account in clientGrp.AsEnumerable()
//                                                                    where Convert.ToInt32(account.AccountId)!=0
//                                                                    group account by account.AccountId into                                              accountGrp
//                                                                    select new AccountAccessInfoDTO
//                                                                    {
//                                                                        AccountId = accountGrp.Key,
//                                                                        SubAccounts = (from subaccount in
//                                                                        accountGrp.AsEnumerable()
//                                                                        where
//                                                                        Convert.ToInt32                                                                    (subaccount.SubAccountId) != 0
//                                                                        select new SubAccountAccessInfoDTO
//                                                                        {
//                                                                            SubAccountId = subaccount.SubAccountId
//                                                                        }).ToList(),
//                                                                    }).ToList()
//                                                    }
//                                                ]
//                                            }
//                                        ]
//                             };

//            return structData.ToList();
//        }
//    }
//}
