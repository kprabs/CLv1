using CoreLib.Application.Common.Constants;
using CoreLib.Application.Common.DBQueries;
using CoreLib.Application.Common.DBQueries.GroupPortal;
using CoreLib.Application.Common.Enums;
using CoreLib.Application.Common.Interfaces;
using CoreLib.Application.Common.Models;
using CoreLib.Application.Common.SqlEntities;
using CoreLib.Application.Common.Utility;
using CoreLib.Entities;
using CoreLib.Entities.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Text;

namespace CoreLib.Infrastructure.Persistence
{
    public partial class SqlRepository(ILogger<SqlRepository> logger) : ISqlRepository
    {
        public DataTable GetAllFeatures(string systemId)
        {
            Dictionary<string, string> objParams = new()
            {
                { "@SystemId", systemId }
            };

            var dataResult = SqlWrapper.ExecuteDataTable(SqlQueries.AuthGetAllFeaturesQuery, objParams, logger);

            return dataResult;
        }

        public List<FeatureEntity> GetAllFeaturesToList(string systemId)
        {
            Dictionary<string, string> objParams = new()
            {
                { "@SystemId", systemId }
            };

            var dataResult = SqlWrapper.ExecuteDataTable(SqlQueries.AuthGetAllFeaturesQuery, objParams, logger);

            var dataToList = DBUtilities.ConvertDataTable<FeatureEntity>(dataResult);
            return dataToList;
        }

        public DataTable GetAssignableClientsHeirarchyForUser(string systemId, string userId)
        {
            Dictionary<string, string> objParams = new()
            {
                { "@SystemId", systemId },
                { "@UserId", userId }
            };

            using (BlockStopwatch blockStopWatch = new(nameof(SqlQueries.AuthGetAllAssignableClientsQuery), logger))
            {
                var dataResult = SqlWrapper.ExecuteDataTable(SqlQueries.AuthGetAllAssignableClientsQuery, objParams, logger);

                return dataResult;
            }
        }

        public List<Tenant> GetAssignableClientsHeirarchyForUserToList(string systemId, string userId)
        {
            Dictionary<string, string> objParams = new()
            {
                { "@SystemId", systemId },
                { "@UserId", userId }
            };

            using (var blockStopWatch = new BlockStopwatch(nameof(SqlQueries.AuthGetAllAssignableClientsQuery), logger))
            {
                var dataResult = SqlWrapper.ExecuteDataTable(SqlQueries.AuthGetAllAssignableClientsQuery, objParams, logger);

                var dataToList = DBUtilities.ConvertJsonDataTable<Tenant>(dataResult);
                return dataToList;
            }
        }

        public DataTable GetUserInformationOfUser(string systemId, string userName)
        {
            Dictionary<string, string> objParams = new()
            {
                { "@SystemId", systemId },
                { "@UserName", userName }
            };

            var dataResult = SqlWrapper.ExecuteDataTable(SqlQueries.UserInfoQuery, objParams, logger);

            return dataResult;
        }

        public DataTable GetUserInformationOfUser(string userName)
        {
            Dictionary<string, string> objParams = new()
            {
                { "@UserName", userName }
            };

            var dataResult = SqlWrapper.ExecuteDataTable(SqlQueries.NewUserInfoQuery, objParams, logger);

            return dataResult;
        }

        public List<UserInfoEntity> GetUserInformationOfUserToList(string systemId, string userId)
        {
            Dictionary<string, string> objParams = new()
            {
                { "@SystemId", systemId },
                { "@UserId", userId }
            };
            var dataResult = SqlWrapper.ExecuteDataTable(SqlQueries.UserInfoQuery, objParams, logger);

            var dataToList = DBUtilities.ConvertDataTable<UserInfoEntity>(dataResult);
            return dataToList;
        }

        public List<UserInfoEntity> GetUserInformationOfUserToList(string userId)
        {
            Dictionary<string, string> objParams = new()
            {
                { "@UserId", userId }
            };
            var dataResult = SqlWrapper.ExecuteDataTable(SqlQueries.NewUserInfoQuery, objParams, logger);

            var dataToList = DBUtilities.ConvertDataTable<UserInfoEntity>(dataResult);
            return dataToList;
        }

        public DataTable GetUserInfoOnPermissionCode(string systemId, string userId, string permissionCode)
        {
            Dictionary<string, string> objParams = new()
            {
                { "@SystemId", systemId },
                { "@UserId", userId },
                { "@PermissionCode", permissionCode }
            };
            var dataResult = SqlWrapper.ExecuteDataTable(SqlQueries.UserInfoOnPermissionCodeQuery, objParams, logger);

            return dataResult;
        }

        public List<UserInfoOnPermissionCode> GetUserInfoOnPermissionCodeToList(string systemId, string userName, string permissionCode)
        {
            Dictionary<string, string> objParams = new()
            {
                { "@SystemId", systemId },
                { "@UserName", userName },
                { "@PermissionCode", permissionCode }
            };
            var dataResult = SqlWrapper.ExecuteDataTable(SqlQueries.UserInfoOnPermissionCodeQuery, objParams, logger);

            var dataToList = DBUtilities.ConvertDataTable<UserInfoOnPermissionCode>(dataResult);
            return dataToList;
        }

        public List<UserInfoOnPermissionCode> GetUserInfoForClientToList(string systemId, string userName, string clientId)
        {
            Dictionary<string, string> objParams = new()
            {
                { "@SystemId", systemId },
                { "@UserName", userName },
                { "@ClientId", clientId }
            };
            var dataResult = SqlWrapper.ExecuteDataTable(SqlQueries.UserInfoForClientQuery, objParams, logger);

            var dataToList = DBUtilities.ConvertDataTable<UserInfoOnPermissionCode>(dataResult);
            return dataToList;
        }

        public DataTable GetUserListBySearchCriteria(string systemId, string userName, string lastName, string firstName, string email, string status)
        {
            Dictionary<string, string> objParams = [];
            var query = SqlQueries.UserListBySearchCriteriaMainQuery.Replace("@SystemId", systemId, StringComparison.InvariantCultureIgnoreCase);
            if (!string.IsNullOrEmpty(userName))
            {
                query += SqlQueries.UserListBySearchCriteriaUserNameSubQuery.Replace("@UserName", userName, StringComparison.InvariantCultureIgnoreCase);
            }

            if (!string.IsNullOrEmpty(firstName))
            {
                query += SqlQueries.UserListBySearchCriteriaFirstNameSubQuery.Replace("@FirstName", firstName, StringComparison.InvariantCultureIgnoreCase);
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                query += SqlQueries.UserListBySearchCriteriaLastNameSubQuery.Replace("@LastName", lastName, StringComparison.InvariantCultureIgnoreCase);
            }

            if (!string.IsNullOrEmpty(email))
            {
                query += SqlQueries.UserListBySearchCriteriaEmailAddressSubQuery.Replace("@Email", email, StringComparison.InvariantCultureIgnoreCase);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query += SqlQueries.UserListBySearchCriteriaActiveFlagSubQuery.Replace("@Status", status, StringComparison.InvariantCultureIgnoreCase);
            }

            var dataResult = SqlWrapper.ExecuteDataTable(query, objParams, logger);

            return dataResult;
        }

        public List<UserListBySearchCriteriaEntity> GetUserListBySearchCriteriaToList(string systemId, string? userName, string? lastName, string? firstName, string? email, string? status)
        {
            Dictionary<string, string> objParams = [];
            string query = SqlQueries.UserListBySearchCriteriaMainQuery.Replace("@SystemId", systemId, StringComparison.InvariantCultureIgnoreCase);

            query = !string.IsNullOrEmpty(userName) ?
                   query.Replace("@UserNameCondition", SqlQueries.UserListBySearchCriteriaUserNameSubQuery.Replace("@UserName", userName, StringComparison.InvariantCultureIgnoreCase), StringComparison.InvariantCultureIgnoreCase)
                   : query.Replace("@UserNameCondition", string.Empty, StringComparison.InvariantCultureIgnoreCase);
            query = !string.IsNullOrEmpty(firstName) ?
                   query.Replace("@FirstNameCondition", SqlQueries.UserListBySearchCriteriaFirstNameSubQuery.Replace("@FirstName", firstName, StringComparison.InvariantCultureIgnoreCase), StringComparison.InvariantCultureIgnoreCase)
                   : query.Replace("@FirstNameCondition", string.Empty, StringComparison.InvariantCultureIgnoreCase);
            query = !string.IsNullOrEmpty(lastName) ?
                   query.Replace("@LastNameCondition", SqlQueries.UserListBySearchCriteriaLastNameSubQuery.Replace("@LastName", lastName, StringComparison.InvariantCultureIgnoreCase), StringComparison.InvariantCultureIgnoreCase)
                   : query.Replace("@LastNameCondition", string.Empty, StringComparison.InvariantCultureIgnoreCase);
            query = !string.IsNullOrEmpty(email) ?
                   query.Replace("@EmailCondition", SqlQueries.UserListBySearchCriteriaEmailAddressSubQuery.Replace("@Email", email, StringComparison.InvariantCultureIgnoreCase), StringComparison.InvariantCultureIgnoreCase)
                   : query.Replace("@EmailCondition", string.Empty, StringComparison.InvariantCultureIgnoreCase);
            query = !string.IsNullOrEmpty(status) ?
                   query.Replace("@ActiveFlagCondition", SqlQueries.UserListBySearchCriteriaActiveFlagSubQuery.Replace("@Status", status, StringComparison.InvariantCultureIgnoreCase), StringComparison.InvariantCultureIgnoreCase)
                   : query.Replace("@ActiveFlagCondition", string.Empty, StringComparison.InvariantCultureIgnoreCase);
            //query = userName == ApplicationConstant.DUMMY_ADMIN || userName == ApplicationConstant.DUMMY_BROKER || userName == ApplicationConstant.DUMMY_STOP_LOSS?
            //    query.Replace("@CLIENTUSERCONDITION", string.Empty, StringComparison.InvariantCultureIgnoreCase)
            //    : query.Replace("@CLIENTUSERCONDITION", SqlQueries.ClientUserCondition, StringComparison.InvariantCultureIgnoreCase);

            DataTable dataResult = SqlWrapper.ExecuteDataTable(query, objParams, logger);

            var dataToList = DBUtilities.ConvertDataTable<UserListBySearchCriteriaEntity>(dataResult);
            return dataToList;
        }

        public DataTable GetFeatureAssignedInstancesForUser(string systemId, string userId)
        {
            Dictionary<string, string> objParams = new()
            {
                { "@SystemId", systemId },
                { "@UserId", userId }
            };

            var dataResult = SqlWrapper.ExecuteDataTable(SqlQueries.FeatureAssignedInstancesQuery, objParams, logger);

            return dataResult;
        }

        public List<FeatureAssingedInstanceEntity> GetFeatureAssignedInstancesForUserToList(string systemId, string userId)
        {
            Dictionary<string, string> objParams = new()
            {
                { "@SystemId", systemId },
                { "@UserId", userId }
            };

            var dataResult = SqlWrapper.ExecuteDataTable(SqlQueries.FeatureAssignedInstancesQuery, objParams, logger);

            var dataToList = DBUtilities.ConvertDataTable<FeatureAssingedInstanceEntity>(dataResult);
            return dataToList;
        }

        public UserConsentType? GetUserConsentType(int SystemLoginUserId, string ClassifiedSegmentInstanceId)
        {
            try
            {
                //string? classfiedSegmentIds = string.Join(",", ClassifiedSegmentInstanceId).Trim();
                string qry = SqlQueries.GetAutoProvisionConcentForUserQuery;
                qry = qry.Replace("@UserId", $" LogInSystemUserId={SystemLoginUserId} ", StringComparison.InvariantCultureIgnoreCase);
                qry = qry.Replace("@ClassifiedSegmentInstanceId", string.IsNullOrEmpty(ClassifiedSegmentInstanceId)
                                                                    ? string.Empty
                                                                    : $" AND ClassifiedSegmentInstanceId IN ({ClassifiedSegmentInstanceId})", StringComparison.InvariantCultureIgnoreCase);
                Dictionary<string, string> objParams = [];

                DataTable dataResult = SqlWrapper.ExecuteDataTable(qry, objParams, logger);
                if (dataResult.Rows.Count > 0)
                {
                    var dataToList = DBUtilities.ConvertDataTable<UserConsentType>(dataResult);
                    return dataToList.FirstOrDefault();
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<UserAccessVerficationEntity> GetUserVerificationToList(string systemCode, string userName, string userRole)
        {
            Dictionary<string, string> objParams = new()
            {
                { "@SystemCode", systemCode },
                { "@UserName", userName },
                { "@UserRole", userRole }
            };

            var dataResult = SqlWrapper.ExecuteDataTable(SqlQueries.ValidateUserAuthorizationQuery, objParams, logger);

            var dataToList = DBUtilities.ConvertDataTable<UserAccessVerficationEntity>(dataResult);
            return dataToList;
        }

        /// <summary>
        /// This method is used for New UserInfo, assignableClients, features, featureAssingedInstances
        /// </summary>
        /// <param name="systemId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserAccessInfoModel GetNewUserInfoDetails(string systemId, string userId, string brandName, IList<int>? clientIds)
        {
            UserAccessInfoModel userAccessInfoModel = new();
            Dictionary<string, string> objParams = [];
            if (clientIds != null && clientIds.Count > 0)
            {
                string cids = "(" + string.Join(",", clientIds) + ")";
                objParams.Add("@Condition", $"and (c.ClassifiedSegmentInstanceId IN {cids})");
                objParams.Add("@JoinCondition", string.Empty);
            }
            else
            {
                objParams.Add("@JoinCondition", "join main.LogInSystemUserSystemAccessibleInstance lsusai on " +
                                "c.ClassifiedSegmentInstanceId = lsusai.ClassifiedSegmentInstanceId and lsusai.LogInSystemUserId=@UserId");
                objParams.Add("@Condition", string.Empty);
            }
            objParams.Add("@SystemId", systemId);
            objParams.Add("@UserId", userId);
            objParams.Add("@BrandName", brandName);
            List<string> queries =
            [
                SqlQueries.NewUserInfoQuery,
                SqlQueries.AuthGetAllAssignableClientsQuery,
                SqlQueries.AuthGetAllFeaturesQuery,
                SqlQueries.FeatureAssignedInstancesQuery,
            ];

            using (BlockStopwatch blockStopWatch = new("GetNewUserInfoDetails", logger))
            {
                var userDataResult = SqlWrapper.ExecuteDataSet(queries, objParams, logger);
                userAccessInfoModel.userInfo = DBUtilities.ConvertDataTable<UserInfoEntity>(userDataResult.Tables[0]);
                userAccessInfoModel.tenants = DBUtilities.ConvertJsonDataTable<Tenant>(userDataResult.Tables[1]);
                userAccessInfoModel.features = DBUtilities.ConvertDataTable<FeatureEntity>(userDataResult.Tables[2]);
                userAccessInfoModel.featureAssingedInstances = DBUtilities.ConvertDataTable<FeatureAssingedInstanceEntity>(userDataResult.Tables[3]);
                return userAccessInfoModel;
            }
        }

        public UserAccessInfoModel GetUserInfoDetails(string systemId, string userId, string brandName, IList<int>? clientId, IList<string>? clientNKeys)
        {
            UserAccessInfoModel userAccessInfoModel = new();
            Dictionary<string, string> objParams = [];
            if (clientId != null && clientId.Count > 0)
            {
                string cids = "(" + string.Join(",", clientId) + ")";
                objParams.Add("@Condition", $"and (c.ClassifiedSegmentInstanceId IN {cids})");
                objParams.Add("@JoinCondition", string.Empty);
            }
            else if (clientNKeys != null && clientNKeys.Count > 0)
            {
                string cNKeys = "(" + string.Join(",", clientNKeys.Select(cnkey => $"'{cnkey}'")) + ")";
                objParams.Add("@Condition", $"and (c.ClassifiedAreaSegmentNKey IN {cNKeys})");
                objParams.Add("@JoinCondition", string.Empty);
            }
            else
            {
                objParams.Add("@JoinCondition", "join main.LogInSystemUserSystemAccessibleInstance lsusai on " +
                                "c.ClassifiedSegmentInstanceId = lsusai.ClassifiedSegmentInstanceId and lsusai.LogInSystemUserId=@UserId");
                objParams.Add("@Condition", string.Empty);
            }
            objParams.Add("@SystemId", systemId);
            objParams.Add("@UserId", userId);
            objParams.Add("@BrandName", brandName);
            List<string> queries =
            [
                SqlQueries.UserInfoQuery,
                SqlQueries.AuthGetAllAssignableClientsQuery,
                SqlQueries.AuthGetAllFeaturesQuery,
                SqlQueries.FeatureAssignedInstancesQuery,
            ];

            using (var blockStopWatch = new BlockStopwatch("GetNewUserInfoDetails", logger))
            {
                var userDataResult = SqlWrapper.ExecuteDataSet(queries, objParams, logger);
                userAccessInfoModel.userInfo = DBUtilities.ConvertDataTable<UserInfoEntity>(userDataResult.Tables[0]);
                userAccessInfoModel.tenants = DBUtilities.ConvertJsonDataTable<Tenant>(userDataResult.Tables[1]);
                userAccessInfoModel.features = DBUtilities.ConvertDataTable<FeatureEntity>(userDataResult.Tables[2]);
                userAccessInfoModel.featureAssingedInstances = DBUtilities.ConvertDataTable<FeatureAssingedInstanceEntity>(userDataResult.Tables[3]);
                return userAccessInfoModel;
            }
        }

        public UserAccessInfoModel GetClientsForBrand(string systemId, string userId, string BrandName, string cid, string clientName)
        {
            UserAccessInfoModel userAccessInfoModel = new();
            Dictionary<string, string> objParams = [];

            bool isConditionSet = false;
            string query = SqlQueries.AuthGetAllAssignableClientsQuery;
            List<string> queries = [SqlQueries.UserInfoQuery];
            if (!string.IsNullOrEmpty(cid))
            {
                objParams.Add("@Condition", $"and (c.ClassifiedAreaSegmentNKey IN ('{cid}'))");
                objParams.Add("@JoinCondition", string.Empty);
                isConditionSet = true;
            }
            objParams.Add("@SystemId", systemId);
            objParams.Add("@UserId", userId);
            objParams.Add("@BrandName", BrandName);
            queries.Add(query);
            if (isConditionSet)
            {
                using (var blockStopWatch = new BlockStopwatch("GetNewUserInfoDetails", logger))
                {
                    var userDataResult = SqlWrapper.ExecuteDataSet(queries, objParams, logger);
                    userAccessInfoModel.userInfo = DBUtilities.ConvertDataTable<UserInfoEntity>(userDataResult.Tables[0]);
                    userAccessInfoModel.tenants = DBUtilities.ConvertJsonDataTable<Tenant>(userDataResult.Tables[1]);
                    return userAccessInfoModel;
                }
            }
            else
            {
                return userAccessInfoModel;
            }
        }

        public List<ClassifiedInstancesEntity> GetClassifiedInstancesByClientId(string clientId)
        {
            Dictionary<string, string> objParams = new()
            {
                { "@ClientIdNKey", clientId }
            };
            var dataResult = SqlWrapper.ExecuteDataTable(SqlQueries.GetClassifiedInstancesQuery, objParams, logger);

            var dataToList = DBUtilities.ConvertDataTable<ClassifiedInstancesEntity>(dataResult);
            return dataToList;
        }

        public List<UsersByClientEntity> GetUsersByClientData(string CID, string brandName, string userType)
        {
            Dictionary<string, string> objParams = [];
            string query = SqlQueries.UsersByClientQuery.Replace("@CID", CID, StringComparison.InvariantCultureIgnoreCase);
            if (!string.IsNullOrEmpty(brandName))
                query = string.Concat(query, SqlQueries.UserInfoBrandNameSubQuery.Replace("@BrandName", brandName, StringComparison.InvariantCultureIgnoreCase));
            if (!string.IsNullOrEmpty(userType))
                query = string.Concat(query, SqlQueries.UserInfoUserTypeSubQuery.Replace("@userType", userType, StringComparison.InvariantCultureIgnoreCase));

            var dataResult = SqlWrapper.ExecuteDataTable(query, objParams, logger);
            var dataToList = DBUtilities.ConvertDataTable<UsersByClientEntity>(dataResult);
            return dataToList;
        }

        public List<T> GetQueryResult<T>(string query)
        {
            var dataResult = SqlWrapper.ExecuteDataTable(query, logger);
            var dataToList = DBUtilities.ConvertDataTable<T>(dataResult);
            return dataToList;
        }

        /// <summary>
        /// Returns the Configuration value fron the specified feature-name key; from the Group Portal Feature table
        /// </summary>
        /// <param name="featureName">the name of the feature name key</param>
        /// <returns></returns>
        public List<Entities.Feature> GetFeature(string featureName)
        {
            Dictionary<string, string> objParams = [];
            StringBuilder sb = new();
            sb.Append(SqlQueries.FeaturesConfigurationQuery);

            if (!string.IsNullOrWhiteSpace(featureName) && featureName != "*")
            {
                sb.Append($" WHERE f.FeatureNKey = '{featureName?.ToString()}'");
            }

            sb.Append(" ORDER BY f.FeatureNKey;");
            var query = sb.ToString();

            string connString = ConfigurationHelper.config.GetSection("ConnectionStrings:GroupPortalEntities").Value;

            var dataResult = SqlWrapper.ExecuteDataTable(connString, query, objParams, logger);
            var dataToList = DBUtilities.ConvertDataTable<Entities.Feature>(dataResult);
            return dataToList;
        }

        public List<GroupPortalCrossWalkPartialDTO> GetMasterLoginIdentifier(string username)
        {
            Dictionary<string, string> objParams = new()
            {
                { "@username", username },
                //{ "@clientNkey", clientNkey }
            };
            var dataResult = SqlWrapper.ExecuteDataTable(SqlQueries.AuthGetMasterLoginIdentifier, objParams, logger);
            if (dataResult.Rows.Count > 0)
            {
                var dataToList = DBUtilities.ConvertDataTable<GroupPortalCrossWalkPartialDTO>(dataResult);
                return dataToList;
                //return dataResult.Rows[0]["MasterLoginIdentifier"].ToString() ?? null;
            }
            return null;
        }

        public List<UserAssociateClientInforamtion> GetAllClientNKeyAssociatedToUser(string loginUserName)
        {
            Dictionary<string, string> objParams = new()
            {
                { "@LOGINUSERNAME", loginUserName }
            };
            var dataResult = SqlWrapper.ExecuteDataTable(SqlQueries.GetAllClientNKeyAssociatedToUserQuery, objParams, logger);
            var dataToList = DBUtilities.ConvertDataTable<Entities.UserAssociateClientInforamtion>(dataResult);
            return dataToList;
        }
        public List<GroupPortalCrossWalkDTO> GetUserCrossWalkInfo(int userId)
        {
            Dictionary<string, string> objParams = new()
            {
                { "@LOGINUSERID", userId.ToString() }
            };
            var dataResult = SqlWrapper.ExecuteDataTable(SqlQueries.GetUserCrossWalkInfoQuery, objParams, logger);
            var dataToList = DBUtilities.ConvertDataTable<GroupPortalCrossWalkDTO>(dataResult);
            return dataToList;
        }

        #region message queries
        /// <summary>
        /// Appends SQL clauses based on input
        /// </summary>
        /// <param name="clauses">SQL clauses</param>
        /// <returns>SQL WHERE clause(s)</returns>
        private string? ClauseBuilder(List<string> clauses)
        {
            string result = null;

            if (clauses?.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                int loopCount = 0;

                foreach (string clause in clauses)
                {
                    ++loopCount;

                    string prepend = loopCount == 1 ? "WHERE" : " AND";
                    sb.AppendLine($"{prepend} {clause}");
                }

                result = sb.ToString();
            }

            return result;
        }

        /// <summary>
        /// Returns the base get messages SQL and WHERE statements base on criteria provided
        /// </summary>
        /// <param name="criteria">search criteria</param>
        /// <returns>SQL</returns>
        private string GetBaseMessageQuery(CoreLib.Entities.Messages.MessageSearchCriteria? criteria)
        {
            List<string> clauses = [];
            StringBuilder sb = new StringBuilder();
            sb.Append(GetMessageQueries.MessageQuery);

            if (criteria?.TypeId > 0)
            {
                clauses.Add($"mt.MsgTypeId = {criteria.TypeId}");
            }

            sb.AppendLine(ClauseBuilder(clauses));

            return sb.ToString();
        }

        /// <summary>
        /// </summary>
        /// <param name="criteria">search criteria</param>
        /// <returns>SQL</returns>
        public string GetAdminMessageQuery(CoreLib.Entities.Messages.MessageSearchCriteria criteria)
        {
            List<string> clauses = [];
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(GetBaseMessageQuery(criteria));

            if (!string.IsNullOrWhiteSpace(criteria?.Subject))
            {
                clauses.Add($"m.MsgSubjectValue LIKE '%{criteria.Subject.ToQuoteString()}%'");
            }

            if (criteria?.StatusId > 0)
            {
                clauses.Add($"ms.MsgStatusId = {criteria.StatusId}");

                if (criteria.StatusId == MessageEnum.MesasgeStatuses.Draft) 
                {
                    clauses.Add($"m.LastUpdateUserNKey = '{criteria.CurrentUserId.Trim().ToQuoteString()}'");
                }

                if (criteria.StatusId > MessageEnum.MesasgeStatuses.Draft)
                {
                    if (criteria?.PublishedDateStart != null && criteria?.PublishedDateEnd != null)
                    {
                        clauses.Add($"m.PublishedDate >= '{criteria.PublishedDateStart?.ToString("yyyy/MM/dd hh:mm:00").ToQuoteString()}'");
                        clauses.Add($"m.PublishedDate <= '{criteria.PublishedDateEnd?.ToString("yyyy/MM/dd hh:mm:00").ToQuoteString()}'");

                        if (!string.IsNullOrWhiteSpace(criteria?.PublishedUser))
                        {
                            clauses.Add($"m.PublishedUserNKey = '{criteria.PublishedUser.Trim().ToQuoteString()}'");
                        }
                    }
                }
            }

            sb.AppendLine(ClauseBuilder(clauses));

            return sb.ToString();
        }

        /// <summary>
        /// Returns the User get messages SQL and WHERE statements base on criteria provided
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public string GetUserMessageQuery(long? id)
        {
            List<string> clauses = [];
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(GetBaseMessageQuery(null));

            clauses.Add($"ms.MsgStatusId = {MessageEnum.MesasgeStatuses.Active}"); //this is Active
            clauses.Add("md.ReleaseDate <= GETUTCDATE()");
            clauses.Add("md.ExpirationDate >= GETUTCDATE()");

            if (id != null && id > 0)
            {
                clauses.Add($"m.MsgId = '{id}'"); //this is for when we want to retrieve a single MsgId
            }

            sb.AppendLine(ClauseBuilder(clauses));

            //if base is changed to the consolidated view... that only add conditionally if you know... the following recipient targets
            //SQL (example): 
            //  AND (
            //  ((mgm.MsgAudienceTypeID = 1 AND mgm.MsgGroupMemberValue = '*') OR (mgm.MsgAudienceTypeID = 2 AND mgm.MsgGroupMemberValue = 'IBC'))
            //  OR (mgm.MsgAudienceTypeID = 3 AND mgm.MsgGroupMemberValue IN('6179'))
            //  OR (mgm.MsgAudienceTypeID = 4 AND mgm.MsgGroupMemberValue IN('CFI'))
            //  OR (mgm.MsgAudienceTypeID = 5 AND mgm.MsgGroupMemberValue IN('Clientuser'))
            //  OR (mgm.MsgAudienceTypeID = 6 AND mgm.MsgGroupMemberValue IN('NV_6179'))
            //)

            return sb.ToString();
        }

        /// <summary>
        /// Returns the Message Group Member SQL per a single message id
        /// </summary>
        /// <param name="messageId">single message id</param>
        /// <returns></returns>
        public string GetMessageGroupMembersQuery(long id)
        {
            List<string> clauses = [];
            StringBuilder sb = new StringBuilder();

            if (id > 0)
            {
                sb.AppendLine(GetMessageQueries.MessageGroupMemberQuery);
                clauses.Add($"md.MsgID = '{id}'");

                sb.AppendLine(ClauseBuilder(clauses));
                sb.AppendLine("ORDER BY md.MsgId, mgm.MsgGroupId, ma.MsgAudienceTypeId");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns the Message Group Member SQL per the message id(s)
        /// </summary>
        /// <param name="ids">message id(s)</param>
        /// <returns></returns>
        public string GetMessageGroupMembersQuery(List<long> ids)
        {
            List<string> clauses = [];
            StringBuilder sb = new StringBuilder();

            if (ids?.Count > 0)
            {
                sb.AppendLine(GetMessageQueries.MessageGroupMemberQuery);
                clauses.Add($"md.MsgID IN ({ids.ToInString()})");

                sb.AppendLine(ClauseBuilder(clauses));
                sb.AppendLine("ORDER BY md.MsgId, mgm.MsgGroupId, ma.MsgAudienceTypeId");
            }

            return sb.ToString();
        }
        #endregion

        #region get (query) methods
        /// <summary>
        /// Returns Message (view) items, per the provided SQL Query
        /// </summary>
        /// <param name="query">SQL Query</param>
        /// <returns>List of MessageView objects</returns>
        public List<CoreLib.Entities.Messages.MessageView> GetMessages(string query)
        {
            Dictionary<string, string> objParams = [];
            List<CoreLib.Entities.Messages.MessageView> result = [];

            if (!string.IsNullOrWhiteSpace(query))
            { 
                string connString = ConfigurationHelper.config.GetSection("ConnectionStrings:GroupPortalEntities").Value;

                var dataResult = SqlWrapper.ExecuteDataTable(connString, query, objParams, logger);
                var dataToList = DBUtilities.ConvertDataTable< CoreLib.Entities.Messages.MessageView> (dataResult);
                result = dataToList;
            }

            return result;
        }

        /// <summary>
        /// Returns Message Group Member (view) items, per the provided SQL Query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<CoreLib.Entities.Messages.MessageGroupMembersView> GetMessageMembers(string query)
        {
            Dictionary<string, string> objParams = [];
            List<CoreLib.Entities.Messages.MessageGroupMembersView> result = [];

            if (!string.IsNullOrWhiteSpace(query))
            {
                string connString = ConfigurationHelper.config.GetSection("ConnectionStrings:GroupPortalEntities").Value;

                var dataResult = SqlWrapper.ExecuteDataTable(connString, query, objParams, logger);
                var dataToList = DBUtilities.ConvertDataTable<CoreLib.Entities.Messages.MessageGroupMembersView>(dataResult);
                result = dataToList;
            }

            return result;
        }
        #endregion

        #region lookup queries)
        public List<CoreLib.Entities.Messages.MessageAudienceType> GetMessageAudienceTypes()
        {
            Dictionary<string, string> objParams = [];
            List<CoreLib.Entities.Messages.MessageAudienceType> result = [];

            string connString = ConfigurationHelper.config.GetSection("ConnectionStrings:GroupPortalEntities").Value;

            var dataResult = SqlWrapper.ExecuteDataTable(connString, GetMessageQueries.MessageAudienceTypeQuery, objParams, logger);
            var dataToList = DBUtilities.ConvertDataTable<CoreLib.Entities.Messages.MessageAudienceType>(dataResult);
            result = dataToList;

            return result;
        }

        public List<CoreLib.Entities.Messages.MessageStatus> GetMessageStatuses()
        {
            Dictionary<string, string> objParams = [];
            List<CoreLib.Entities.Messages.MessageStatus> result = [];

            string connString = ConfigurationHelper.config.GetSection("ConnectionStrings:GroupPortalEntities").Value;

            var dataResult = SqlWrapper.ExecuteDataTable(connString, GetMessageQueries.MessageStatusQuery, objParams, logger);
            var dataToList = DBUtilities.ConvertDataTable<CoreLib.Entities.Messages.MessageStatus>(dataResult);
            result = dataToList;

            return result;
        }

        public List<CoreLib.Entities.Messages.MessageType> GetMessageTypes()
        {
            Dictionary<string, string> objParams = [];
            List<CoreLib.Entities.Messages.MessageType> result = [];

            string connString = ConfigurationHelper.config.GetSection("ConnectionStrings:GroupPortalEntities").Value;

            var dataResult = SqlWrapper.ExecuteDataTable(connString, GetMessageQueries.MessageTypeQuery, objParams, logger);
            var dataToList = DBUtilities.ConvertDataTable<CoreLib.Entities.Messages.MessageType>(dataResult);
            result = dataToList;

            return result;
        }
        #endregion
        public List<MessageViewModel> GetAllMessages()
        {
            var query = GetMessageQueries.MessageQuery;
            List<MessageViewModel> result = [];

            if (!string.IsNullOrWhiteSpace(query))
            {
                string connString = ConfigurationHelper.config.GetSection("ConnectionStrings:GroupPortalEntities").Value;

                var dataResult = SqlWrapper.ExecuteDataTable(connString, query, logger);         


                var dataToList = DBUtilities.ConvertDataTable<MessageViewModel>(dataResult);
                result = dataToList;
            }

            return result;
        }
    }
}
