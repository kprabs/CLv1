using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Application.Common.DBQueries.GroupPortal;

public class GetMessageQueries
{
    //primary/base (view) for messages
    public const string MessageQuery = @"
        SELECT m.MsgId as MessageId
            , m.MsgSubjectValue as Subject
            , m.MsgBody as Body
            , mt.MsgTypeId as TypeId
            , mt.Name as [Type]
            , ms.MsgStatusId as StatusId
            , ms.Name as Status
            , m.CreateDate
            , DATEADD(hh, DATEDIFF(hh, GETDATE(), GETUTCDATE()), m.CreateDate) as CreateDateUTC 
            , m.CreateUserNKey as CreateUser
            , m.LastUpdateDate
            , DATEADD(hh, DATEDIFF(hh, GETDATE(), GETUTCDATE()), m.LastUpdateDate) as LastUpdateDateUTC 
            , m.LastUpdateUserNKey as LastUpdateUser
            , m.PublishedDate
            , DATEADD(hh, DATEDIFF(hh, GETDATE(), GETUTCDATE()), m.PublishedDate) as PublishedDateUTC 
            , m.PublishedUserNKey as PublishedUser
            , md.MsgGroupId as GroupId
            , md.ReleaseDate
            , DATEADD(hh, DATEDIFF(hh, GETDATE(), GETUTCDATE()), md.ReleaseDate) as ReleaseDateUTC 
            , md.ExpirationDate
            , DATEADD(hh, DATEDIFF(hh, GETDATE(), GETUTCDATE()), md.ExpirationDate) as ExpirationDateUTC 
        FROM main.Msg m
            INNER JOIN code.MsgType mt on m.MsgTypeId = mt.MsgTypeId
            INNER JOIN code.MsgStatus ms on m.MsgStatusID = ms.MsgStatusId
            INNER JOIN main.MsgDelivery md on m.MsgId = md.MsgId";

    //primary/base (view) for message group/delivery (values)
    public const string MessageDeliveryQuery = @"
        SELECT md.MsgDeliveryId as DeliveryId
            , md.MsgId as MessageId
            , md.ReleaseDate
            , DATEADD(hh, DATEDIFF(hh, GETDATE(), GETUTCDATE()), md.ReleaseDate) as ReleaseDateUTC 
            , md.ExpirationDate
            , DATEADD(hh, DATEDIFF(hh, GETDATE(), GETUTCDATE()), md.ExpirationDate) as ExpirationDateUTC 
            , mg.MsgGroupId as GroupId
            , mg.MsgGroupNKey as GroupKey
        FROM main.MsgDelivery md
            INNER JOIN main.Msg m on m.MsgId = md.MsgId
            INNER JOIN main.MsgGroup mg on md.MsgGroupId = mg.MsgGroupId";

    //direct query forEach member group value(s); will filter by MsgGroupId
    public const string MessageGroupMemberQuery = @"
        SELECT mgm.MsgGroupMemberId as MemberId
            , md.MsgId as MessageId
            , mgm.MsgGroupId as GroupId
            , mgm.MsgGroupMemberValue as [Value]
            , ma.MsgAudienceTypeId as AudienceId
            , ma.Name as Audience
            , mg.MsgGroupNKey as GroupKey
        FROM main.MsgGroupMember mgm
            INNER JOIN main.MsgDelivery md on mgm.MsgGroupId = md.MsgGroupId
            INNER JOIN code.MsgAudienceType ma on mgm.MsgAudienceTypeId = ma.MsgAudienceTypeId 
            INNER JOIN main.MsgGroup mg on mgm.MsgGroupId = mg.MsgGroupId";

    public const string MessageAudienceTypeQuery = @"
        SELECT ma.MsgAudienceTypeID
              , ma.[Name]
        FROM code.MsgAudienceType ma
        WHERE ma.ActiveFlag = 1
        ORDER BY ma.MsgAudienceTypeID";

    public const string MessageStatusQuery = @"
        SELECT ms.MsgStatusID
            , ms.[Name]
        FROM code.MsgStatus ms
        WHERE ms.ActiveFlag = 1
        ORDER BY ms.MsgStatusID";

    public const string MessageTypeQuery = @"
        SELECT mt.MsgTypeID
            , mt.[Name]
        FROM code.MsgType mt
        WHERE mt.ActiveFlag = 1
        ORDER BY mt.MsgTypeID";
}

