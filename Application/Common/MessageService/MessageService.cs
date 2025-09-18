using AutoMapper;
using CoreLib.Application.Common.Enums;
using CoreLib.Application.Common.Interfaces;
using CoreLib.Application.Common.Models;
using CoreLib.Application.Common.Models.MessageModel;
using CoreLib.Application.Common.Utility;
using CoreLib.Entities.Messages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using System.Text;

namespace CoreLib.Application.Common.MessageService;

public class MessageService : IMessageService
{
    private readonly ISqlRepository _sqlRepository;
    private readonly IGroupPortalRepository _groupPortalRepository;
    private readonly ILogger _logger;

    public MessageService(ISqlRepository sqlRepository, IGroupPortalRepository groupPortalRepository, ILogger logger)
    {
        _sqlRepository = sqlRepository;
        _groupPortalRepository = groupPortalRepository;
        _logger = logger;
    }

    #region validator methods
    /// <summary>
    /// For use with add/update methods
    /// </summary>
    /// <param name="item">new/update message item</param>
    /// <returns>true/false</returns>
    public bool MessageRequestValidator(MessageItemRequest item)
    {
        string byApp = "[MessageRequestValidator]";
        int fieldCount = 0; int expected = 8;
        List<string> missing = [];

        if (!string.IsNullOrWhiteSpace(item?.BrandName)) { ++fieldCount; } else { missing.Add("BrandName"); }
        if (!string.IsNullOrWhiteSpace(item?.CurrentUserId)) { ++fieldCount; } else { missing.Add("CurrentUserId"); }

        if (string.IsNullOrWhiteSpace(item?.Subject)) { ++fieldCount; } else { missing.Add("Subject"); }

        //add fluent validation for script tags
        if (!string.IsNullOrWhiteSpace(item?.Body)) { ++fieldCount; } else { missing.Add("Body"); }

        if (item?.AudienceId > 0) { ++fieldCount; } else { missing.Add("AudienceId"); }
        if (item?.TypeId > 0) { ++fieldCount; } else { missing.Add("TypeId"); }
        if (item?.StatusId > 0) { ++fieldCount; } else { missing.Add("StatusId"); }

        if (item?.ExpirationDate != null) { ++fieldCount; } else { missing.Add("ExpirationDate"); }

        if (fieldCount != expected)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{byApp}: called, Message request had: {fieldCount} of the required: {expected} values; ");
            sb.Append($"required fields missing: {missing.ToDelimiter<string>(",")}");

            _logger.LogWarning(sb.ToString());
        }

        return fieldCount == expected;
    }

    public bool MessageSearchValidator(MessageHeadersRequest item)
    {
        string byApp = "[MessageSearchValidator]";
        int fieldCount = 0; int expected = 5;
        List<string> missing = [];

        if (!string.IsNullOrWhiteSpace(item?.BrandName)) { ++fieldCount; } else { missing.Add("BrandName"); }
        if (!string.IsNullOrWhiteSpace(item?.ClientId)) { ++fieldCount; } else { missing.Add("ClientId"); }
        if (!string.IsNullOrWhiteSpace(item?.PlatformIndicator)) { ++fieldCount; } else { missing.Add("PlatformIndicator"); }
        if (!string.IsNullOrWhiteSpace(item?.UserRole)) { ++fieldCount; } else { missing.Add("UserRole"); }
        if (item?.UserIds?.Count > 0) { ++fieldCount; } else { missing.Add("UserIds"); }

        if (fieldCount != expected)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{byApp}: called, Message request had: {fieldCount} of the required: {expected} values; ");
            sb.Append($"required fields missing: {missing.ToDelimiter<string>(",")}");

            _logger.LogWarning(sb.ToString());
        }

        return fieldCount == expected;
    }

    #endregion

    #region query (view) methods
    #region query methods
    public async Task<MessagesResult> GetAdminMessages(MessageSearchCriteria criteria)
    {
        MessagesResult results = new() { Messages = new List<MessageViewResult>(), Count = 0 };
        string query = _sqlRepository.GetAdminMessageQuery(criteria);
        List<CoreLib.Entities.Messages.MessageView> data = _sqlRepository.GetMessages(query);

        if (data?.Count > 0)
        {
            MessageViewResult messages = new MessageViewResult();

            foreach (var m in data)
            {
                MessageItem item = new MessageItem()
                {
                    Id = m.MessageId,
                    Subject = m.Subject,
                    Body = m.Body,

                    StatusId = m.StatusId,
                    Status = m.Status,
                    TypeId = m.TypeId,
                    Type = m.Type,

                    ReleaseDate = m.ReleaseDate,
                    ExpirationDate = m.ExpirationDate,
                    PublishedDate = m.PublishedDate,
                    PublishedUser = m.PublishedUser,
                };

                string membersQuery = _sqlRepository.GetMessageGroupMembersQuery(item.Id);
                List<MessageGroupMembersView> members = _sqlRepository.GetMessageMembers(membersQuery);
                List<MessageRecipient> recipients = [];

                foreach (var member in members)
                {
                    recipients.Add(new MessageRecipient(member.AudienceId, member.Value));
                }

                MessageViewResult result = new MessageViewResult()
                {
                    Item = item,
                    Recipients = recipients
                };

                results.Messages.Add(result);
            }

            results.Count = results.Messages?.Count ?? 0;
        }

        return await Task.FromResult(results);
    }

    public async Task<MessagesResult> GetUserMessages(MessageSearchCriteria criteria)
    {
        MessagesResult results = new() { Messages = new List<MessageViewResult>(), Count = 0 };

        MessageHeadersRequest headers = criteria?.Headers;
        bool headerCheck = MessageSearchValidator(headers);

        //in order to filter for only target recipient
        //might need to handle this at a view-sql level vs. programatticaly; as we will be getting all messages to then filter, vs. filtering via SQL.
        if (headerCheck)
        {
            string query = _sqlRepository.GetUserMessageQuery(criteria?.Id);
            List<CoreLib.Entities.Messages.MessageView> data = _sqlRepository.GetMessages(query);
            List<MessageViewResult> filter = [];

            if (data?.Count > 0)
            {
                MessageViewResult messages = new MessageViewResult();

                foreach (var m in data)
                {
                    MessageItem item = new MessageItem()
                    {
                        Id = m.MessageId,
                        Subject = m.Subject,
                        Body = m.Body,

                        StatusId = m.StatusId,
                        Status = m.Status,
                        TypeId = m.TypeId,
                        Type = m.Type,

                        ReleaseDate = m.ReleaseDate,
                        ExpirationDate = m.ExpirationDate,
                        PublishedDate = m.PublishedDate,
                        PublishedUser = m.PublishedUser,
                    };

                    string membersQuery = _sqlRepository.GetMessageGroupMembersQuery(item.Id);
                    List<MessageGroupMembersView> members = _sqlRepository.GetMessageMembers(membersQuery);
                    List<MessageRecipient> recipients = [];

                    foreach (var member in members)
                    {
                        recipients.Add(new MessageRecipient(member.AudienceId, member.Value));
                    }

                    MessageViewResult result = new MessageViewResult()
                    {
                        Item = item,
                        Recipients = recipients
                    };

                    if (result.Recipients?.Count > 0)
                    {
                        int filterCount = 0;

                        bool isGlobal = false;
                        if (result.Recipients.Where(x => x.Id > MessageEnum.AudienceTypes.Global).ToList()?.Count == 1) { isGlobal = true; }
                        string? brand = result?.Recipients?.Where(x => x.Id == MessageEnum.AudienceTypes.Brand).SingleOrDefault()?.Value;
 
                        List<string>? clients = result?.Recipients?.Where(x => x.Id == MessageEnum.AudienceTypes.Client)
                            .Select(x => x.Value).ToList();

                        List<string>? platforms = result?.Recipients?.Where(x => x.Id == MessageEnum.AudienceTypes.Platform)
                            .Select(x => x.Value).ToList();

                        List<string>? roles = result?.Recipients?.Where(x => x.Id == MessageEnum.AudienceTypes.UserType)
                            .Select(x => x.Value).ToList();

                        List<string>? users = result?.Recipients?.Where(x => x.Id == MessageEnum.AudienceTypes.User)
                            .Select(x => x.Value).ToList();

                        if (isGlobal) { ++filterCount; }
                        if (!string.IsNullOrWhiteSpace(brand) && brand.EqualsIgnoreCase(headers.BrandName)) { ++filterCount; }
                        if (clients?.Count > 0 && clients.Contains(headers.ClientId, StringComparer.OrdinalIgnoreCase)) { ++filterCount; }
                        if (platforms?.Count > 0 && platforms.Contains(headers.PlatformIndicator, StringComparer.OrdinalIgnoreCase)) { ++filterCount; }
                        if (roles?.Count > 0 && roles.Contains(headers.UserRole, StringComparer.OrdinalIgnoreCase)) { ++filterCount; }
                        if (users?.Count > 0 && headers.UserIds?.Count == 1 && users.Contains(headers.UserIds.Single(), StringComparer.OrdinalIgnoreCase)) { ++filterCount; }

                        if (filterCount > 0)
                        {
                            results.Messages.Add(result);
                        }
                    }
                }

                results.Count = results.Messages?.Count ?? 0;
            }
        }

        return await Task.FromResult(results);
    }

    private async Task<List<MessageGroupMembersView>> GetMessageMembers(List<long> messageIds)
    {
        string query = _sqlRepository.GetMessageGroupMembersQuery(messageIds);
        return await Task.FromResult(_sqlRepository.GetMessageMembers(query));
    }
    #endregion

    #endregion

    #region query (lookup) methods
    public async Task<List<MessageAudienceType>> GetMessageAudienceTypes()
    {
        return await Task.FromResult(_sqlRepository.GetMessageAudienceTypes());
    }

    public async Task<List<MessageStatus>> GetMessageStatuses()
    {
        return await Task.FromResult(_sqlRepository.GetMessageStatuses());
    }

    public async Task<List<MessageType>> GetMessageTypes()
    {
        return await Task.FromResult(_sqlRepository.GetMessageTypes());
    }
    #endregion

    #region add/update methods

    //create message
    #region Add Message
    public async Task<MessageItem?> AddMessage(MessageItemRequest item) 
    {
        MessageItem? result = null;
        string byApp = "[AddMessage]";

        if (MessageRequestValidator(item))
        {
            if (item?.Recipients == null)
            {
                if (item?.BrandName == "*")
                {
                    item.Recipients.Add(new MessageRecipient(MessageEnum.AudienceTypes.Global, item?.BrandName));
                }
                else
                {
                    item.Recipients.Add(new MessageRecipient(MessageEnum.AudienceTypes.Brand, item?.BrandName));
                }
            }
            _logger.LogInformation($"{byApp} started: {JsonConvert.SerializeObject(item)}");

            try
            {
                _logger.LogInformation($"{byApp} mapping models to entities, with record creation");
                DateTime seed = DateTime.Now;
                string userId = item.CurrentUserId;

                CoreLib.Entities.Messages.Message message = new()
                {
                    MsgSubjectValue = item.Subject,
                    MsgBody = item.Body,
                    MsgStatusId = item.StatusId,
                    MsgTypeId = item.TypeId,
                    CreateDate = seed,
                    CreateUserNKey = userId,
                    LastUpdateDate = seed,
                    LastUpdateUserNKey = userId
                };
                //create message, get Id
                _logger.LogInformation($"{byApp} Message model: {JsonConvert.SerializeObject(message)}");
                _logger.LogInformation($"{byApp}: calling CreateMessage on repository.");
                await _groupPortalRepository.CreateMessage(message);
                _logger.LogInformation($"{byApp}: CreateMessage completed.");

                CoreLib.Entities.Messages.MessageGroup group = new()
                { 
                    MsgGroupNKey = Guid.NewGuid().ToString()
                };
                //create group get Id
                _logger.LogInformation($"{byApp} MessageGroup model: {JsonConvert.SerializeObject(group)}");
                _logger.LogInformation($"{byApp}: calling CreateMessageGroup on repository.");
                //await _groupPortalRepository.CreateMessageGroup(group);
                _logger.LogInformation($"{byApp}: CreateMessageGroup completed.");

                //create delivery, using msg-id, group-id
                CoreLib.Entities.Messages.MessageDelivery delivery = new()
                {
                    MsgGroupId = group.MsgGroupId,
                    ReleaseDate = DateTime.UtcNow,
                    ExpirationDate = (DateTime)item.ExpirationDate,
                    CreateDate = seed,
                    CreateUserNKey = userId,
                };
                //create delivery
                _logger.LogInformation($"{byApp} MessageDelivery model: {JsonConvert.SerializeObject(delivery)}");
                _logger.LogInformation($"{byApp}: calling CreateMessageDelivery on repository.");
                //await _groupPortalRepository.CreateMessageDelivery(delivery);
                _logger.LogInformation($"{byApp}: CreateMessageDelivery completed.");

                //create group members, using msg-id, group-id
                List<MessageGroupMember> members = [];
                foreach (MessageRecipient recipient in item.Recipients)
                {
                    CoreLib.Entities.Messages.MessageGroupMember member = new()
                    {
                        MsgGroupId = group.MsgGroupId,
                        MsgGroupMemberValue = recipient.Value,
                        MsgAudienceTypeId = recipient.Id,
                        CreateDate = seed,
                        CreateUserNKey = userId,
                    };

                    members.Add(member);
                }
                //create group members
                _logger.LogInformation($"{byApp} MessageGroupMember models: {JsonConvert.SerializeObject(members)}");
                _logger.LogInformation($"{byApp}: calling CreateMessageGroupMembers on repository.");
                //await _groupPortalRepository.CreateMessageGroupMembers(members);
                _logger.LogInformation($"{byApp}: CreateMessageGroupMembers completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred in {byApp}.");
                throw;
            }
            finally
            {
                _logger.LogInformation($"{byApp} completed.");
            }
        }

        return await Task.FromResult(result);
    }
    #endregion
    //update message (if Active, then status only); aka: archive

    //NOTES: after message creation, you need the group, group-members, delivery table rows to reference the new MsgID
    //once the message has been created, create in the following sequence:
    //#1 create new MsgGroup record, we'll use the new MsgGroup.MsgGroupID for reference; generate a GUID string value for the GroupNKey
    //#2 create new MsgGroupMember record with, the new: MsgGroup.MsgGroupID
    //  for each (of the new Msg.MsgID's) recipient: Value and MsgAudienceType.MsgAudienceTypeID

    //#3 create new MsgDelivery record with the new: Msg.MsgID, MsgGroup.MsgGroupID
    //  make sure the Msg.MsgID Msg.PublishedDate is the value for MsgDelivery.ReleaseDate
    //  we want this for now, as we don't have a date/time picker for Draft message;
    //      so as a default we want the MsgDelivery.ReleaseDate to match the Msg.PublishedDate

    //create message group
    //create message group member(s); per the new MsgID

    //delete message target (hard deleten for draft only)
    //  the idea is to not manage the MsgGroupMember rows per the MsgGroupID
    //  instead, delete all the records for the MsgGroupID, and replace with new records of the changes
    //  no reason to do merges and track, just wipe clean, and replace
    //  this should only be allowed while the MsgID as a MsgStatusID of 1 (draft)

    //create message delivery
    //update message delivery; only allow update for ReleaseDate and ExperationDate 
    #endregion
    #region Read method


    public async Task<MessageItem> UpdateMessage(MessageItemRequest item)
    {
        var m  = new  MessageItem();
        
        return await Task.FromResult(m);

    }

    public async Task<bool> DeleteMessage(MessageItemRequest item)
    {
        var m = true;
        return await Task.FromResult(m); ;
    }
    #endregion

}
