using CoreLib.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Entities.Messages;

#region Base models
public class BaseAuditFields
{
    [Required] //stored as (and store as) UTC date/time
    public DateTime CreateDate { get; set; }
    [Required]
    public string CreateUserNKey { get; set; }
}

public class BaseAuditUpdateFields : BaseAuditFields
{
    //stored as (and store as) UTC date/time
    public DateTime? LastUpdateDate { get; set; }
    public string? LastUpdateUserNKey { get; set; }
}

public class BaseAuditUpdateRequiredFields : BaseAuditFields
{
    [Required] //stored as (and store as) UTC date/time
    public DateTime LastUpdateDate { get; set; }
    [Required]
    public string LastUpdateUserNKey { get; set; }
}

//we can use this for: audience, status, nessage types
public class BaseLookupFields
{
    public string Name { get; set; }
 
    //public bool ActiveFlag { get; set; }
    ////stored as (and store as) UTC date/time
    //public DateTime CreateDate { get; set; }
}
#endregion

#region Main/Core (table) Models
[Table("Msg", Schema = "Main")]
public class Message : BaseAuditUpdateRequiredFields
{
    [Key]
    public long MsgId { get; set; }

    [Required]
    public string MsgSubjectValue { get; set; }
    [Required]
    public string MsgBody { get; set; }

    [Required]
    public long MsgTypeId { get; set; }
    [Required]
    public int MsgStatusId { get; set; }

    //stored as (and store as) UTC date/time
    public DateTime? PublishedDate { get; set; }
    public string? PublishedUserNKey { get; set; }
}

[Table("MsgTemplate", Schema = "Main")]
public class MessageTemplate : BaseAuditUpdateFields
{
    [Key]
    public int MsgTemplateId { get; set; }

    [Required]
    public string MsgTemplateName { get; set; }

    [Required]
    public string MsgTemplateSubjectValue { get; set; }
    [Required]
    public string MsgTemplateBody { get; set; }

    [Required]
    public int MsgTypeId { get; set; }
    [Required]
    public int MsgStatusId { get; set; }

    [Required]
    public bool ActiveFlag { get; set; }
}

#region Message Grouping models
[Table("MsgGroup", Schema = "Main")]
public class MessageGroup : BaseAuditUpdateFields
{
    [Key]
    public int MsgGroupId { get; set; }
    [Required]
    public string MsgGroupNKey { get; set; }
    [Required]
    public string? MsgGroupName { get; set; }
}

[Table("MsgGroupMember", Schema = "Main")]
public class MessageGroupMember : BaseAuditUpdateFields
{
    [Key]
    public long MsgGroupMemberId { get; set; }
    [Required]
    public int MsgGroupId { get; set; }

    [Required]
    public string MsgGroupMemberValue { get; set; }

    [Required]
    public int MsgAudienceTypeId { get; set; }
}

[Table("MsgDelivery", Schema = "Main")]
public class MessageDelivery : BaseAuditUpdateFields
{
    public long MsgDeliveryId { get; set; }
    [Required]
    public long MsgGroupId { get; set; }

    [Required] //stored as (and store as) UTC date/time
    public DateTime ReleaseDate { get; set; }
    [Required] //stored as (and store as) UTC date/time
    public DateTime ExpirationDate { get; set; }
}
#endregion

#endregion

#region Lookup/Reference (table) Models
//[Table("MsgType", Schema = "Code")]
public class MessageType : BaseLookupFields
{
    public int MsgTypeId { get; set; }
}

//[Table("MsgAudienceType", Schema = "Code")]
public class MessageAudienceType : BaseLookupFields
{
    public int MsgAudienceTypeId { get; set; }
}

//[Table("MsgStatus", Schema = "Code")]
public class MessageStatus : BaseLookupFields
{
    public int MsgStatusId { get; set; }
}

[Table("MsgVariable", Schema = "Main")]
public class MessageVariable : BaseAuditUpdateFields
{
    [Key]
    public int MsgVaribleId { get; set; }

    [Required]
    public string Name { get; set; }
    [Required]
    public string Variable { get; set; }

    [Required]
    public bool ActiveFlag { get; set; }
}

#endregion

#region criteria models
public class MessageHeadersRequest
{
    public string? BrandName { get; set; }
    public string? ClientId { get; set; }
    public string? PlatformIndicator { get; set; }
    public string? UserRole { get; set; }
    public List<string> UserIds { get; set; } = new List<string>();
}

public class MessageSearchCriteria
{
    public MessageHeadersRequest? Headers { get; set; }

    #region for: MessageView (sql)
    public long? Id { get; set; }

    //MsgSubjectValue; Subject; wildcard LIKE
    public string? Subject { get; set; }

    //MsgAudienceTypeId; TypeId
    public int? TypeId { get; set; }

    //MsgStatusTypeId; StatusId; for users this is always: 2 (draft)
    public int? StatusId { get; set; }

    //ReleaseDate; will only filter for IF both dates are not-null
    //public DateTime? ReleaseDateStart { get; set; }
    ////ReleaseDate
    //public DateTime? ReleaseDateEnd { get; set; }

    //ExpirationDate; will only filter for IF both dates are not-null
    public DateTime? ExpirationDateStart { get; set; }
    //ExpirationDate
    public DateTime? ExpirationDateEnd { get; set; }

    //"Sent By" date; will only filter for IF both dates are not-null
    public DateTime? PublishedDateStart { get; set; }
    public DateTime? PublishedDateEnd { get; set; }

    //"Sent By" user
    public string? PublishedUser { get; set; }
    #endregion

    #region for: MessageGroupMembersView (sql)
    //MsgAudienceType: AudienceId
    public int? AudienceId { get; set; }

    //IF MsgAudienceTypeId = 6 (user)
    //THEN MsgAudienceTypeId (AudienceId) = 6 AND MsgGroupMemberValue (Value) in Users
    public List<string> Users { get; set; }

    public string CurrentUserId { get; set; }
    #endregion
}
#endregion

#region View Models
public class MessageView
{
    public long MessageId { get; set; }

    public string Subject { get; set; }
    public string Body { get; set; }

    public int TypeId { get; set; }
    public string Type { get; set; }

    public int StatusId { get; set; }
    public string Status { get; set; }

    public long AudienceId { get; set; }
    public string GroupKey { get; set; }

    //stored as UTC date/time
    public DateTime PublishedDate { get; set; }
    public DateTime PublishedDateUTC { get; set; }
    public string PublishedUser { get; set; }

    //stored as UTC date/time
    public DateTime ReleaseDate { get; set; }
    public DateTime ReleaseDateUTC { get; set; }

    //stored as UTC date/time
    public DateTime ExpirationDate { get; set; }
    public DateTime ExpirationDateUTC { get; set; }
}

public class MessageGroupMembersView
{
    public int MemberId { get; set; }
    public long MessageId { get; set; }
    public int GroupId { get; set; }

    public string Value { get; set; }
    public int AudienceId { get; set; }
    public string Audience { get; set; }

    public string GroupKey { get; set; }
}

public class MessageDeliveryView
{
    public long DeliveryId { get; set; }
    public long MessageId { get; set; }
    public int GroupId { get; set; }

    public DateTime ReleaseDate { get; set; }
    public DateTime ReleaseDateUTC { get; set; }

    public DateTime ExpirationDate { get; set; }
    public DateTime ExpirationDateUTC { get; set; }

    public long GroupKey { get; set; }
}
#endregion

#region Service ViewModels/Models/DTOsm - API stuff
 
public sealed class CreateMessageRequest
{
    public string Subject { get; init; } = "";
    public string SentBy { get; init; } = "";
    public string RecipientsSemiColonDelimited { get; init; } = "";
    public string MessageType { get; init; } = "";
    public DateTime? Sent { get; init; }
    public DateTime? Expires { get; init; }
    public string Status { get; init; } = "";
}
public sealed class UpdateMessageRequest
{
    public string Subject { get; init; } = "";
    public string SentBy { get; init; } = "";
    public string RecipientsSemiColonDelimited { get; init; } = "";
    public string MessageType { get; init; } = "";
    public DateTime? Sent { get; init; }
    public DateTime? Expires { get; init; }
    public string Status { get; init; } = "";
}


public sealed class MessageResult<T>
{
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int Total { get; init; }
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
}
public class MessageViewModel
{
    public long MessageId { get; set; }

    public string Subject { get; set; }
    public string Body { get; set; }

    public int TypeId { get; set; }
    public string Type { get; set; }

    public int StatusId { get; set; }
    public string Status { get; set; }

    public DateTime CreateDate { get; set; }
    public DateTime CreateDateUTC { get; set; }

    public string CreateUser { get; set; }

    //stored as UTC date/time
    public DateTime LastUpdateDate { get; set; }
    public DateTime LastUpdateDateUTC { get; set; }

    public string LastUpdateUser { get; set; }

    public DateTime PublishedDate { get; set; }
    public DateTime PublishedDateUTC { get; set; }

    public string PublishedUser { get; set; }

    public int GroupId { get; set; }


    //stored as UTC date/time
    public DateTime ReleaseDate { get; set; }
    public DateTime ReleaseDateUTC { get; set; }

    //stored as UTC date/time
    public DateTime ExpirationDate { get; set; }
    public DateTime ExpirationDateUTC { get; set; }
}

#endregion
