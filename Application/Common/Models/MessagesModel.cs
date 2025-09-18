using CoreLib.Entities.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Application.Common.Models.MessageModel;
//put result models here from message service result methods

#region result (lookup) models
public class BaseLookupResult
{
    //we can use this for: recipient, status, nessage types
    public int Id { get; set; }

    public string Name { get; set; }
}

public class MessageVariableResult : BaseLookupResult
{
    public string Value { get; set; }
}
#endregion

#region database resource models
public class MessageItem
{
    public long Id { get; set; }

    public string Subject { get; set; }
    public string Body { get; set; }

    public int TypeId { get; set; }
    public string Type { get; set; }

    public int StatusId { get; set; }
    public string Status {  get; set; }

    public DateTime PublishedDate { get; set; }
    public DateTime PublishedDateUTC { get; set; }
    public string PublishedUser { get; set; }

    public DateTime ReleaseDate { get; set; }
    public DateTime ReleaseDateUTC { get; set; }

    public DateTime ExpirationDate { get; set; }
    public DateTime ExpirationDateUTC { get; set; }
}

public class MessageRecipient
{
    public MessageRecipient(int id, string value)
    { 
        Id = id;
        Value = value;
    }

    public int Id { get; set; }
    public string Value { get; set; }
}
#endregion

#region result models
public class MessageViewResult
{
    public MessageItem Item { get; set; }
    public List<MessageRecipient> Recipients { get; set; } = new List<MessageRecipient>();
}

public class MessagesResult
{
    public List<MessageViewResult> Messages { get; set; } = new List<MessageViewResult>();
    public long Count { get; set; }
}
#endregion

#region add/update models

// Used for (new and update) Message Creation
public class MessageItemRequest
{
    public string BrandName { get; set; }
    public string CurrentUserId { get; set; }

    public string Subject { get; set; }
    public string Body { get; set; }

    public int TypeId { get; set; }
    public int StatusId { get; set; }
    public long AudienceId { get; set; }

    //stored as UTC date/time
    public DateTime? ExpirationDate { get; set; }

    public List<MessageRecipient> Recipients { get; set; } = new List<MessageRecipient>();
}

#endregion
