using CoreLib.Application.Common.Interfaces;
using CoreLib.Application.Common.Models.MessageModel;
using CoreLib.Constants;
using CoreLib.Entities;
using CoreLib.Entities.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Globalization;
namespace CoreLib.Infrastructure.Persistence
{
    public class GroupPortalRepository(IRepository<GroupPortalDbContext> groupportalrepository, ILogger<GroupPortalRepository> logger) : IGroupPortalRepository
    {
        public async Task<IEnumerable<SystemUserLoginLog>> GetUserLoginLogAsync(string username)
        {
            var result = await groupportalrepository.GetAllByWhere<SystemUserLoginLog>(x => x.SystemUserName == username)
                .ContinueWith(x => x.Result.OrderByDescending<SystemUserLoginLog, DateTime>(y => y.LastUpdateDate).Distinct().ToList());
            return result;
        }

        public async Task<bool> SetUserLoginLogAsync(SystemUserLoginLog userNotes)
        {
            try
            {
                await groupportalrepository.AddAsync(userNotes);
                await groupportalrepository.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<DocumentInstance> GetTermsAndConditionsDetail()
        {
            var currentDate = DateTime.Now.Date;
            var documentType = await groupportalrepository.FirstOrDefaultAsync<DocumentType>(x => x.Code == "GP_TCS");
            return await groupportalrepository.FirstOrDefaultAsync<DocumentInstance>(x => x.ActiveFlag == true
                                                                                && x.DocumentTypeId == documentType.DocumentTypeId
                                                                                && currentDate >= x.DocumentEffDate
                                                                                && currentDate < x.DocumentTermDate);
        }

        public async Task<bool> DeactiveUserTermsAndContions(string userName)
        {
            var documentInstance = await GetTermsAndConditionsDetail();
            if (documentInstance == null)
            {
                return true; //no active terms and conditions
            }
            //Check user acknowledgement exists for user
            var data = await groupportalrepository.GetAllByWhere<UserDocumentAction>(x => x.ExternalSystemUserNkey == userName
                                                        && x.DocumentInstanceId == documentInstance.DocumentInstanceId)
                                                            .ContinueWith(x => x.Result.OrderByDescending(x => x.DecisionDate));
            if (data == null || data.ToList().Count == 0)
            {
                return true;
            }

            var list = data.ToList();
            bool isAcknowledgementAccepted = Convert.ToBoolean(list[0].AcceptanceFlag, CultureInfo.InvariantCulture);

            if (isAcknowledgementAccepted)
            {
                UserDocumentAction record = new()
                {
                    ExternalSystemUserNkey = userName,
                    DocumentInstanceId = documentInstance.DocumentInstanceId,
                    AcceptanceFlag = false,
                    DecisionDate = DateTime.Now
                };
                await groupportalrepository.AddAsync(record);
                await groupportalrepository.SaveChangesAsync();
                return true;
            }
            return true;
        }

        public async Task<string> GetUserPermissionClientNKeyName(string nKey)
        {
            var permissionClient = await groupportalrepository.FirstOrDefaultAsync<EmployerPortalUserPermission>(x => x.ClientIdNKey == nKey);
            return permissionClient?.ClientNKey;
        }

        public async Task SetAutoProvisionLog(AutoProvisionLog autoProvisionLog)
        {
            try
            {
                groupportalrepository.Add(autoProvisionLog);
                groupportalrepository.SaveChanges();
            }
            catch (Exception ex)
            {
                autoProvisionLog.RemarkDetail = ex.StackTrace;
                groupportalrepository.Add(autoProvisionLog);
                groupportalrepository.SaveChanges();
            }
        }

        public async void UpdateAutoProvisionLog(string RequestID, string statusCode, string remarks)
        {
            groupportalrepository.UpdateOnCondition<AutoProvisionLog>(x => x.RequestUserIdNKey == RequestID, e =>
             {
                 e.LastUpdateDate = DateTime.Now;
                 e.RemarkDetail = string.IsNullOrEmpty(remarks) ? e.RemarkDetail : remarks;
                 e.AutoProvisionStatusCode = statusCode;
             });
        }

        public AutoProvisionLog? GetAutoProvisionLog(string RequestID)
        {
            return groupportalrepository.FindWhere<AutoProvisionLog>(x => x.RequestUserIdNKey == RequestID).Result.FirstOrDefault();
        }

        public async Task SetAPIAudit(APIAudit setAPIAudit)
        {
            logger.LogInformation("SetAPIAudit started.");
            if (setAPIAudit == null)
            {
                logger.LogWarning("setAPIAudit parameter is null.");
                throw new ArgumentNullException(nameof(setAPIAudit));
            }

            logger.LogInformation($"SetAPIAudit started. {JsonConvert.SerializeObject(setAPIAudit)}");

            try
            {

                logger.LogInformation("Adding APIAudit entity to repository.");
                await groupportalrepository.AddAsync<APIAudit>(setAPIAudit);
                logger.LogInformation("Saving changes to repository.");
                await groupportalrepository.SaveChangesAsync();
                logger.LogInformation("SetAPIAudit completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception occurred in SetAPIAudit.");
                throw;
            }
        }

        public async Task<APIAuditPCP?> GetAPIAuditPCP(string memberIdNumber)
        {
            APIAuditPCP? objGetPcp = null;
            if (string.IsNullOrWhiteSpace(memberIdNumber) || memberIdNumber.Length != CoreLibConstant.MEMBERIDLENGTH)
            {
                throw new InvalidDataException("Bad MemberID for PCP related changes");
            }

            var memberKey = Convert.ToInt64(memberIdNumber, CultureInfo.InvariantCulture);
            var apiAuditPcPs = await
                groupportalrepository.FindWhere<APIAuditPCP>(x => x.MemberKey == memberKey && !x.IsProcessedFlag);
            if (!apiAuditPcPs.Any())
            {
                return objGetPcp;
            }

            objGetPcp = apiAuditPcPs
                .OrderByDescending(d => d.CreateDate)
                .FirstOrDefault();

            return objGetPcp;
        }

        public async Task AddAPIAuditPCP(APIAuditPCP setAPIAuditPCP)
        {
            try
            {
                await groupportalrepository.AddAsync(setAPIAuditPCP);
                await groupportalrepository.SaveChangesAsync();
                logger.LogInformation($"APIAuditPCP added successfully for MemberKey: {setAPIAuditPCP.MemberKey} at {DateAndTime.Now}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error adding APIAuditPCP for MemberKey: {setAPIAuditPCP.MemberKey} at {DateAndTime.Now}");
            }
        }

        public int UpdateAPIAuditPCP(APIAuditPCP setAPIAuditPCP)
        {
            int iRowsUpdated = 0;
            try
            {
                var entity = groupportalrepository.FindWhere<APIAuditPCP>(x => x.MemberKey == setAPIAuditPCP.MemberKey && !x.IsProcessedFlag).Result.FirstOrDefault();
                if (entity != null)
                {
                    entity.ModifiedDate = DateTime.Now;
                    entity.IsProcessedFlag = true;
                    groupportalrepository.Update(entity);
                    iRowsUpdated = groupportalrepository.SaveChanges();
                    logger.LogInformation($"UpdateAPIAuditPCP updated successfully for MemberKey:" + $" {setAPIAuditPCP.MemberKey} at {DateAndTime.Now}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in UpdateAPIAuditPCP while updating for MemberKey: {setAPIAuditPCP.MemberKey} at {DateAndTime.Now}");
            }
            return iRowsUpdated;
        }

        #region Message Add methods    
        public Task<int> CreateMessage(Message message)
        {
            throw new NotImplementedException();
        }

        public Task<int> CreateMessageGroup(MessageGroup group)
        {
            throw new NotImplementedException();
        }

        public Task<int> CreateMessageDelivery(MessageDelivery delivery)
        {
            throw new NotImplementedException();
        }

        public Task<int> CreateMessageGroupMembers(List<MessageGroupMember> members)
        {
            throw new NotImplementedException();
        }
        #endregion
        public Task<Message> UpdateMessage(Message message)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteMessage(Message message)
        {
            throw new NotImplementedException();
        }

       
    }
}
