using CoreLib.Application.Common.Constants;
using CoreLib.Application.Common.Interfaces;
using CoreLib.Application.Common.Models;
using CoreLib.Application.Common.Utility;

namespace CoreLib.Application.Common.FeatureConfigurationService
{
    public class FeatureConfigurationService : IFeatureConfigurationService
    {
        private readonly ISqlRepository _sqlRepository;

        public FeatureConfigurationService(ISqlRepository sqlRepository)
        { 
            _sqlRepository = sqlRepository;
        }

        #region query methods
        public async Task<List<CoreLib.Entities.Feature>> GetFeatureConfiguration(string name)
        {
            return await Task.FromResult(_sqlRepository.GetFeature(name));
        }
        #endregion

        #region evaluation methods

        #region eval check methods
        /// <summary>
        /// Returns Brand items and counts for any matches via header variables
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Found: results (.Values), # of items (.Counts), found match (.Value), # of matches (.Count)</returns>
        private FeatureEvalResponse<CoreLib.Application.Common.Models.Brand> BrandCheck(FeatureEvalRequest request)
        {
            //extract objects
            List<CoreLib.Application.Common.Models.Brand> results = request?.Feature?.Configuration?.Brand.ToList();

            //check for (single) direct match; null if none; using EqualsIgnoreCase which checks values, are not: null, empty of whitespace
            CoreLib.Application.Common.Models.Brand? item = results?.SingleOrDefault(x => x.BrandName.EqualsIgnoreCase(request.Request?.BrandName));
            //count how many direct matches; null if none
            int? count = results?.Count(x => x.BrandName.EqualsIgnoreCase(request.Request?.BrandName));

            FeatureEvalResponse<CoreLib.Application.Common.Models.Brand> result = new()
            {
                Value = item, Count = count,
                Values = results, Counts = results.Count()
            };

            return result;
        }

        /// <summary>
        /// Returns Client items and counts for any matches via header variables
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Found: results (.Values), # of items (.Counts), found match (.Value), # of matches (.Count)</returns>
        private FeatureEvalResponse<FeatureClient> ClientCheck(FeatureEvalRequest request, Models.Brand parent)
        {
            //extract objects
            List<CoreLib.Application.Common.Models.FeatureClient> results = parent?.Client.ToList();

            //check for (single) direct match; null if none; using EqualsIgnoreCase which checks values, are not: null, empty of whitespace
            CoreLib.Application.Common.Models.FeatureClient? item = results?.SingleOrDefault(x => x.ClientId.EqualsIgnoreCase(request?.Request?.ClientId));
            //count how many direct matches; null if none
            int? count = results?.Count(x => x.ClientId.EqualsIgnoreCase(request?.Request?.ClientId));

            FeatureEvalResponse<CoreLib.Application.Common.Models.FeatureClient> result = new()
            {
                Value = item, Count = count,
                Values = results, Counts = results.Count()
            };

            return result;
        }

        /// <summary>
        /// Returns Platform items and counts for any matches via header variables
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Found: results (.Values), # of items (.Counts), found match (.Value), # of matches (.Count)</returns>
        private FeatureEvalResponse<PlatformModel> PlatformCheck(FeatureEvalRequest request, Models.FeatureClient parent)
        {
            //extract objects
            List<PlatformModel> results = parent?.Platform.ToList();

            //check for (single) direct match; null if none; using EqualsIgnoreCase which checks values, are not: null, empty of whitespace
            PlatformModel? item = results?.SingleOrDefault(x => x.PlatformIndicator.EqualsIgnoreCase(request?.Request?.PlatformIndicator));
            //count how many direct matches; null if none
            int? count = results?.Count(x => x.PlatformIndicator.EqualsIgnoreCase(request?.Request?.PlatformIndicator));

            FeatureEvalResponse<PlatformModel> result = new()
            {
                Value = item, Count = count,
                Values = results, Counts = results.Count()
            };

            return result;
        }

        /// <summary>
        /// Returns UserRole items and counts for any matches via header variables
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Found: results (.Values), # of items (.Counts), found match (.Value), # of matches (.Count)</returns>
        private FeatureEvalResponse<UserRole> UserRoleCheck (FeatureEvalRequest request, Models.PlatformModel parent)
        {
            //extract objects
            List<UserRole> results = parent?.UserRole.ToList();

            //check for (single) direct match; null if none; using EqualsIgnoreCase which checks values, are not: null, empty of whitespace
            UserRole? item = results?.SingleOrDefault(x => x.Role.EqualsIgnoreCase(request?.Request?.UserRole));
            //count how many direct matches; null if none.Request?
            int? count = results?.Count(x => x.Role.EqualsIgnoreCase(request?.Request?.UserRole));

            FeatureEvalResponse<UserRole> result = new()
            {
                Value = item, Count = count,
                Values = results, Counts = results.Count()
            };

            return result;
        }

        /// <summary>
        /// Returns User items and counts for any matches via header variables
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Found: results (.Values), # of items (.Counts), found match (.Value), # of matches (.Count)</returns>
        private FeatureEvalResponse<Users> UserIdsCheck(FeatureEvalRequest request, Models.UserRole parent)
        {
            List<Users> results = parent.Role.EqualsIgnoreCase(request.Request?.UserRole) ? parent.Users.ToList() : new List<Users>();

            List<string> items = new List<string>();
            if (results?.Count > 0)
            {
                foreach (Users u in results)
                {
                    List<string>? ids = u.UserId;
                    if (ids?.Count > 0)
                    {
                        foreach (string id in ids)
                        {
                            if (request.Request.UserIds.Contains(id))
                            {
                                items.Add(id);
                            }
                        }
                    }
                }
            }

            //check for (single) direct match; null if none
            Users item = new() { UserId = items };            
            //count how many direct matches; null if none
            int? count = items.Count;

            FeatureEvalResponse<Users> result = new()
            {
                Value = item, Count = count,
                Values = results, Counts = results.Count()
            };

            return result;
        }
        #endregion

        #region deeper check methods
        /// <summary>
        /// Determines matches from specified inputs
        /// </summary>
        /// <param name="request">Feature Evaluation Request</param>
        /// <param name="eval">Brand object to review</param>
        /// <returns>returns a log of reason checks with a total # ot matches from eval object and sub/child hierachy</returns>
        private FeatureEvalResultCheck DeepCheckForBrand(FeatureEvalRequest request, FeatureEvalResponse<Models.Brand> eval, int logCount = 0)
        {
            int result = -1; int wildcards = 0; int finalCount = 0;
            string prepend = "DC> for Brand:";
            string logStep = $"BrandCheck: {request.Feature.FeatureName}";
            List<ReasonLogItem> log = [];

            if (eval.Counts > 0)
            {
                result = 0;
                log.Add(new ReasonLogItem(++logCount, logStep, $"{prepend} found {eval.Counts} Brand(s)"));
                bool foundMatch = false; string foundValue = string.Empty;

                //iterate and focus on each item
                foreach (Models.Brand item in eval.Values)
                {
                    ++logCount;
                    //if the match is found, stop processing
                    if (foundMatch == false) 
                    {
                        log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} EVAL: `{item.BrandName}`"));

                        List<FeatureClient> children = item.Client?.ToList();
                        log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} [`{item.BrandName}`] found {children.Count} Client(s)"));

                        bool deepCheck = false;

                        if (eval.Values.Count == 1)
                        {
                            if (string.IsNullOrWhiteSpace(item.BrandName))
                            {
                                //means we have null match; enable for all
                                ++result; foundMatch = true; foundValue = "*"; ++wildcards;
                                log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} .BrandName wildcard match: `{item.BrandName}`"));
                            }
                        }
                        if (eval.Count == 1 && eval.Value != null)
                        {
                            //means we have a (single) match; no dups
                            foundMatch = request.Request.BrandName.EqualsIgnoreCase(item.BrandName);
                            if (foundMatch)
                            {
                                ++result;
                                log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} .BrandName direct match: `{item.BrandName}`"));
                            }
                            foundValue = item.BrandName;
                            log.Add(new ReasonLogItem(++logCount, logStep, $"{prepend} .Brand found: `{foundValue}`"));
                        }

                        deepCheck = (result > 0 && children?.Count > 0);

                        if (deepCheck)
                        {
                            log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} checking deeper for Client(s)"));

                            int count = 0; bool foundDeepMatch = false;
                            foreach (FeatureClient child in children)
                            {
                                ++logCount;

                                log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} DC> EVAL: `{child.ClientId}`"));
                                if (foundDeepMatch == false) 
                                {
                                    FeatureEvalResponse<FeatureClient> check = ClientCheck(request, item);
                                    log.Add(new ReasonLogItem(++logCount, logStep, $"-->>> {prepend} {item.BrandName}: Cient.ClientId: `{child.ClientId}`; {check.Counts} found"));

                                    FeatureEvalResultCheck deeperCheck = DeepCheckForClient(request, check, logCount);
                                    count += deeperCheck.FinalCounts;
                                    log.AddRange(deeperCheck.ReasonLog);

                                    foundDeepMatch = deeperCheck.FinalCounts > 0;
                                }

                                if (foundDeepMatch == true)
                                {
                                    break;
                                }
                            }
                            result += count;
                            finalCount = count == 0 ? 0 : result;
                            log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} deepcheck: final result: {finalCount}"));
                        }
                        else
                        {
                            finalCount = result;
                        }

                        logCount += log.Count;
                        log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} match `{foundValue}` found? `{foundMatch.ToString()}`, result: {result}, wildcards: {wildcards}, final: {finalCount}"));
                    }

                }
            }

            return new FeatureEvalResultCheck(result, log, wildcards, finalCount);
        }

        /// <summary>
        /// Determines matches from specified inputs
        /// </summary>
        /// <param name="request">Feature Evaluation Request</param>
        /// <param name="eval">Client object to review</param>
        /// <returns>returns a log of reason checks with a total # ot matches from eval object and sub/child hierachy</returns>
        private FeatureEvalResultCheck DeepCheckForClient(FeatureEvalRequest request, FeatureEvalResponse<FeatureClient> item, int logCount = 0)
        {
            int result = 0; int wildcards = 0; int finalCount = 0;
            string prepend = $"> DC> for Client:";
            string logStep = $"ClientCheck: {request.Feature.FeatureName}";
            List<ReasonLogItem> log = [];

            if (item.Counts > 0)
            {
                ++logCount;
                log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} found {item.Counts} Client(s)"));
                bool foundMatch = false; string foundValue = string.Empty;

                log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} EVAL: `{item.Value?.ClientId}`"));
                List<PlatformModel> children = item.Value?.Platform?.ToList() ?? new();
                log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} [`{item.Value?.ClientId}`] found {children.Count} Platform(s)"));

                bool deepCheck = false;
                FeatureClient parent = null;

                if (item.Values.Count == 1)
                {
                    if (string.IsNullOrWhiteSpace(item.Values[0]?.ClientId))
                    {
                        //means we have null match; enable for all
                        ++result; foundMatch = true; foundValue = "*"; ++wildcards;

                        log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} .ClientId wildcard match: `{item.Value?.ClientId}`"));
                    }

                    if (wildcards > 0)
                    {
                        parent = item.Values[0];
                        if (parent?.Platform?.Count > 0 && children.Count == 0)
                        {
                            children = parent.Platform.ToList();
                        }
                    }
                }
                if (item.Count == 1 && item?.Value != null)
                {
                    //means we have a direct match
                    foundMatch = request.Request.ClientId.EqualsIgnoreCase(item.Value?.ClientId);
                    if (foundMatch)
                    {
                        ++result;
                        parent = item.Value;
                        log.Add(new ReasonLogItem(++logCount, logStep, $"{prepend} .ClientId direct match: `{item.Value?.ClientId}`"));
                    }
                    foundValue = item.Value?.ClientId;
                    log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} .ClientId found: `{foundValue}`"));
                }

                deepCheck = (result > 0 && children.Count > 0);

                if (deepCheck)
                {
                    log.Add(new ReasonLogItem(++logCount, logStep, $"-->>> {prepend} checking deeper for Platform(s)"));

                    if (children.Count > 0)
                    {
                        int count = 0; bool foundDeepMatch = false;
                        foreach (PlatformModel child in children)
                        {
                            ++logCount;
                            log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} DC> EVAL: `{child.PlatformIndicator}`"));
                            if (foundDeepMatch == false)
                            {
                                FeatureEvalResponse<PlatformModel> check = PlatformCheck(request, parent);
                                log.Add(new ReasonLogItem(++logCount, logStep, $"-->>> {prepend} {parent.ClientId}: Platform.PlatformIndicator: `{child.PlatformIndicator}`; {check.Counts} found"));

                                FeatureEvalResultCheck deeperCheck = DeepCheckForPlatform(request, check, logCount);
                                count += deeperCheck.FinalCounts;
                                log.AddRange(deeperCheck.ReasonLog);

                                foundDeepMatch = deeperCheck.FinalCounts > 0;
                            }

                            if (foundDeepMatch == true)
                            {
                                break;
                            }
                        }
                        result += count;
                        finalCount = count == 0 ? 0 : result;
                    }
                    log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} deepcheck: final result: {finalCount}"));
                }
                else
                {
                    finalCount = result;
                }

                logCount += log.Count;
                log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} match `{foundValue}` found? `{foundMatch.ToString()}`, result: {result}, wildcards: {wildcards}, final: {finalCount}"));
            }

            return new FeatureEvalResultCheck(result, log, wildcards, finalCount);
        }

        /// <summary>
        /// Determines matches from specified inputs
        /// </summary>
        /// <param name="request">Feature Evaluation Request</param>
        /// <param name="eval">Platform object to review</param>
        /// <returns>returns a log of reason checks with a total # ot matches from eval object and sub/child hierachy</returns>
        private FeatureEvalResultCheck DeepCheckForPlatform(FeatureEvalRequest request, FeatureEvalResponse<PlatformModel> item, int logCount = 0)
        {
            int result = 0; int wildcards = 0; int finalCount = 0;
            string prepend = $">> DC> for Platform:";
            string logStep = $"PlatformCheck: {request.Feature.FeatureName}";
            List<ReasonLogItem> log = [];

            if (item.Counts > 0)
            {
                ++logCount;
                log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} found {item.Counts} Platforms(s)"));
                bool foundMatch = false; string foundValue = string.Empty;

                log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} EVAL: `{item.Value?.PlatformIndicator}`"));
                List<UserRole> children = item.Value?.UserRole?.ToList() ?? new();
                log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} [`{item.Value?.PlatformIndicator}`] found {children.Count} UserRole(s)"));

                bool deepCheck = false;
                PlatformModel parent = null;

                if (item.Values.Count == 1)
                {
                    if (string.IsNullOrWhiteSpace(item.Values[0]?.PlatformIndicator))
                    {
                        //means we have null match; enable for all
                        ++result; foundMatch = true; foundValue = "*"; ++wildcards;

                        log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} .PlatformIndicator wildcard match: `{item.Value?.PlatformIndicator}`"));
                    }

                    if (wildcards > 0)
                    {
                        parent = item.Values[0];
                        if (parent.UserRole.Count > 0 && children.Count == 0)
                        {
                            children = parent.UserRole.ToList();
                        }
                    }
                }
                if (item.Count == 1 && item?.Value != null)
                {
                    //means we have a direct match
                    foundMatch = request.Request.PlatformIndicator.EqualsIgnoreCase(item.Value?.PlatformIndicator);
                    if (foundMatch)
                    {
                        ++result;
                        parent = item.Value;
                        log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} .PlatformIndicator direct match: `{item.Value?.PlatformIndicator}`"));
                    }
                    foundValue = item.Value?.PlatformIndicator;

                    log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} .PlatformIndicator found: `{foundValue}`"));
                }

                deepCheck = (result > 0 && children.Count > 0);

                if (deepCheck)
                {
                    log.Add(new ReasonLogItem(++logCount, logStep, $"{prepend} checking deeper for UserRole(s)"));

                    if (children.Count > 0)
                    {
                        int count = 0; bool foundDeepMatch = false;
                        foreach (UserRole child in children)
                        {
                            ++logCount;
                            log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} DC> EVAL: `{child.Role}`"));
                            if (foundDeepMatch == false)
                            {
                                FeatureEvalResponse<UserRole> check = UserRoleCheck(request, parent);
                                log.Add(new ReasonLogItem(++logCount, logStep, $"-->>> {prepend} {parent.PlatformIndicator}: UserRole.Role: `{child.Role}`; {check.Counts} found"));

                                FeatureEvalResultCheck deeperCheck = DeepCheckForUserRole(request, check, logCount);
                                count += deeperCheck.FinalCounts;
                                log.AddRange(deeperCheck.ReasonLog);

                                foundDeepMatch = deeperCheck.FinalCounts > 0;
                            }

                            if (foundDeepMatch == true)
                            {
                                break;
                            }
                        }
                        result += count;
                        finalCount = count == 0 ? 0 : result;
                    }
                    log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} deepcheck: final result: {finalCount}"));
                }
                else
                {
                    finalCount = result;
                }

                logCount += log.Count;
                log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} match `{foundValue}` found? `{foundMatch.ToString()}`, result: {result}, wildcards: {wildcards}, final: {finalCount}"));
            }

            return new FeatureEvalResultCheck(result, log, wildcards, finalCount);
        }

        /// <summary>
        /// Determines matches from specified inputs
        /// </summary>
        /// <param name="request">Feature Evaluation Request</param>
        /// <param name="eval">UserRole object to review</param>
        /// <returns>returns a log of reason checks with a total # ot matches from eval object and sub/child hierachy</returns>
        private FeatureEvalResultCheck DeepCheckForUserRole(FeatureEvalRequest request, FeatureEvalResponse<UserRole> item, int logCount = 0)
        {
            int result = 0; int wildcards = 0; int finalCount = 0;
            string prepend = $">>> DC> for UserRole:";
            string logStep = $"UserRoleCheck: {request.Feature.FeatureName}";
            List<ReasonLogItem> log = [];

            if (item.Counts > 0)
            {
                ++logCount;
                log.Add(new ReasonLogItem(++logCount, logStep, $"{prepend} found {item.Counts} UserRole(s)"));
                bool foundMatch = false; string foundValue = string.Empty;

                log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} EVAL: `{item.Value?.Role}`"));
                List<Users> children = item.Value?.Users?.ToList() ?? new();
                log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} [`{item.Value?.Role}`] found {children.Count} Users(s)"));

                bool deepCheck = false;
                UserRole parent = null;

                if (item.Values?.Count == 1)
                {
                    if (string.IsNullOrWhiteSpace(item.Values[0]?.Role))
                    {
                        //means we have null match; enable for all
                        ++result; foundMatch = true; foundValue = "*"; ++wildcards;

                        log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} .Role wildcard match: `{item.Value?.Role}`"));
                    }

                    if (wildcards > 0)
                    {
                        parent = item.Values[0];
                        if (parent?.Users?.Count > 0 && children.Count == 0)
                        {
                            children = item.Values[0].Users.ToList();
                        }
                    }
                }
                if (item.Count == 1 && item?.Value != null)
                {
                    //means we have a direct match
                    foundMatch = request.Request.UserRole.EqualsIgnoreCase(item.Value?.Role);
                    if (foundMatch)
                    {
                        ++result;
                        parent = item.Value;
                        log.Add(new ReasonLogItem(++logCount, logStep, $"{prepend} .Role direct match: `{item.Value?.Role}`"));
                    }
                    foundValue = item.Value?.Role;
                    log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} .Role found: `{foundValue}`"));
                }

                deepCheck = (result > 0 && children.Count > 0);

                if (deepCheck)
                {
                    log.Add(new ReasonLogItem(++logCount, logStep, $"{prepend} checking deeper for UserRole(s)"));

                    if (children.Count > 0)
                    {
                        int count = 0; bool foundDeepMatch = false;
                        foreach (Users child in children)
                        {
                            ++logCount;
                            log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} DC> EVAL: `{child.UserId.ToDelimiter<string>(", ")}`"));
                            if (foundDeepMatch == false)
                            {
                                FeatureEvalResponse<Users> check = UserIdsCheck(request, parent);
                                log.Add(new ReasonLogItem(++logCount, logStep, $"-->>> {prepend} {parent.Role}: User.UserId: {check.Counts} found"));

                                FeatureEvalResultCheck deeperCheck = DeepCheckForUserIds(request, check, logCount);
                                count += deeperCheck.FinalCounts;
                                log.AddRange(deeperCheck.ReasonLog);

                                foundDeepMatch = deeperCheck.FinalCounts > 0;
                            }

                            if (foundDeepMatch == true)
                            {
                                break;
                            }
                        }
                        result += count;
                        finalCount = count == 0 ? 0 : result;
                    }
                    log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} deepcheck: final result: {finalCount}"));
                }
                else
                {
                    finalCount = result;
                }

                logCount += log.Count;
                log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} match `{foundValue}` found? `{foundMatch.ToString()}`, result: {result}, wildcards: {wildcards}, final: {finalCount}"));
            }
            return new FeatureEvalResultCheck(result, log, wildcards, finalCount);
        }

        /// <summary>
        /// Determines matches from specified inputs
        /// </summary>
        /// <param name="request">Feature Evaluation Request</param>
        /// <param name="eval">Users object to review</param>
        /// <returns>returns a log of reason checks with a total # ot matches from eval object and sub/child hierachy</returns>
        private FeatureEvalResultCheck DeepCheckForUserIds(FeatureEvalRequest request, FeatureEvalResponse<Users> item, int logCount = 0)
        {
            int result = 0;
            string prepend = $">>>> DC> for UserId:";
            string logStep = $"UserIdCheck: {request.Feature.FeatureName}";
            List<ReasonLogItem> log = [];

            bool foundMatch = false; string foundValue = string.Empty;

            if (item?.Value?.UserId.Count() > 0)
            {
                ++logCount;
                log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} EVAL: `{item.Value?.UserId.ToDelimiter<string>(", ")}`"));

                foreach (string user in item?.Value?.UserId)
                {
                    if (foundMatch == false) 
                    {
                        foundMatch = request.Request.UserIds.Contains(user);
                        if (foundMatch)
                        {
                            ++result;
                            foundValue = user;
                            log.Add(new ReasonLogItem(++logCount, logStep, $"{prepend} .UserId[] direct match: `{user}`"));
                        }
                        log.Add(new ReasonLogItem(++logCount, logStep, $"{prepend} .UserId[] found: `{user}`"));
                    }
                }
                log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} deepcheck: final result: {result}"));
            }

            logCount += log.Count;
            log.Add(new ReasonLogItem(++logCount, logStep, $"-->> {prepend} match `{foundValue}` found? `{foundMatch.ToString()}`, result: {result}"));

            return new FeatureEvalResultCheck(result, log, 0, result);
        }
        #endregion

        #region header checks
        /// <summary>
        /// Reads specified Header values and confirms required values exist
        /// </summary>
        /// <param name="request">feature request</param>
        /// <returns>true/false</returns>
        public bool CheckForHeaders(FeatureRequest request)
        {
            //count number of field/value checks
            int fieldCheck = 0;
            //# of expected values to proceed
            int validCheck = 6;

            //check for header/userdetail values; fc = 5
            if (!string.IsNullOrWhiteSpace(request.ClientId)) { ++fieldCheck; }
            if (!string.IsNullOrWhiteSpace(request.BrandName)) { ++fieldCheck; }
            if (!string.IsNullOrWhiteSpace(request.PlatformIndicator)) { ++fieldCheck; }
            if (!string.IsNullOrWhiteSpace(request.UserRole)) { ++fieldCheck; }
            if (request.UserIds?.Count > 0) { ++fieldCheck; }

            //check feature for name; fc = 1
            if (!string.IsNullOrWhiteSpace(request.FeatureName)) { ++fieldCheck; }

            return fieldCheck == validCheck;
        }
        #endregion

        #region result method(s)
        /// <summary>
        /// Evaluates Feature.Configuration to determine if it should be enabled based on specified Header values
        /// </summary>
        /// <param name="requests"></param>
        /// <returns>FeatureName and if Configuration should be enabled</returns>
        public async Task<List<FeatureEvalResponse>> GetFeatureConfigurationEvals(List<FeatureEvalRequest> requests)
        {
            List<FeatureEvalResponse> results = new();
            List<ReasonLogItem> log = [];
            int logCount = 0; string logStep = "EvalsCheck";
            log.Add(new ReasonLogItem(++logCount, logStep, $"INFO: {requests.Count} feature request(s)"));
            log.Add(new ReasonLogItem(++logCount, logStep, $"START: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}"));

            if (requests?.Count > 0)
            {
                log.Add(new ReasonLogItem(++logCount, logStep, $"HEADER: FeatureName: {requests[0].Request.FeatureName}"));
                log.Add(new ReasonLogItem(++logCount, logStep, $"HEADER: BrandName: {requests[0].Request.BrandName}"));
                log.Add(new ReasonLogItem(++logCount, logStep, $"HEADER: ClientId: {requests[0].Request.ClientId}"));
                log.Add(new ReasonLogItem(++logCount, logStep, $"HEADER: PlatformIndicator: {requests[0].Request.PlatformIndicator}"));
                log.Add(new ReasonLogItem(++logCount, logStep, $"HEADER: UserRole: {requests[0].Request.UserRole}"));
                log.Add(new ReasonLogItem(++logCount, logStep, $"HEADER: UserId: {requests[0].Request.UserIds.FirstOrDefault()}"));

                foreach (FeatureEvalRequest request in requests) 
                {
                    //values found for all fields; all checks should pass for processing
                    if (CheckForHeaders(request?.Request))
                    {
                        ++logCount;

                        log.Add(new ReasonLogItem(++logCount, $"{logStep}: {request.Feature.FeatureName}", $"REQUEST: FeatureName: {request.Feature.FeatureName}"));
                        if (request?.Feature?.Configuration != null) 
                        {
                            log.Add(new ReasonLogItem(++logCount, $"{logStep}: {request.Feature.FeatureName}", "Configuration:"));
                            log.Add(new ReasonLogItem(++logCount, $"{logStep}: {request.Feature.FeatureName}", request?.Feature?.Configuration?.SerializeObjectToJson(false)));
                        }
                        log.Add(new ReasonLogItem(++logCount, $"{logStep}: {request.Feature.FeatureName}", $"---\\ {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} \\---"));
                        log.Add(new ReasonLogItem(++logCount, $"{logStep}: {request.Feature.FeatureName}", $"EVAL: FeatureName: {request.Request.FeatureName}"));

                        FeatureEvalResponse response = new FeatureEvalResponse()
                        { 
                            FeatureName = request.Feature.FeatureName
                        };

                        int resultCount = -1;
                        //asumptions you can make about each check method
                        // .Values will always be new/empty by default if no matches; .Counts will always be 0 or greater
                        // .Value will be null or a single found match from Header value
                        // .Count will be null (if .Value is null), or # of match(es) from Header value
                        //      IF .Count this is greater than 1, .Value will be null; we only expect 1 instance of <key property> value

                        FeatureEvalResponse<CoreLib.Application.Common.Models.Brand> brands = BrandCheck(request);
                        if (brands.Counts > 0)
                        {
                            resultCount = 0;
                            FeatureEvalResultCheck deeperCheck = DeepCheckForBrand(request, brands, logCount);
                            resultCount += deeperCheck.FinalCounts;
                            response.ReasonLog.AddRange(deeperCheck.ReasonLog);
                            log.AddRange(deeperCheck.ReasonLog);
                        }

                        //if no brands, the result should be null, otherwise true/false by result (match) counts
                        response.Value = resultCount == -1 ? null : resultCount > 0;
                        results.Add(response);
                    }
                    logCount += (log.Count + 1);
                }
            }

            log.Add(new ReasonLogItem(++logCount, logStep, $"STOP: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}"));

            return await Task.FromResult(results);
        }
        #endregion

        #endregion
    }
}
