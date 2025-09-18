using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Application.Common.Enums;

public class MessageEnum
{
    public class AudienceTypes
    {
        public const int Global = 1;
        public const int Brand = 2;
        public const int Client = 3;
        public const int Platform = 4;
        public const int UserType = 5;
        public const int User = 6;
        public const int Account = 7;
        public const int SubAccount = 8;
    }

    public class MesasgeTypes 
    {
        public const int General = 1;
        public const int Announcement = 2;
        public const int Critical = 3;
    }

    public class MesasgeStatuses
    {
        public const int Draft = 1;
        public const int Active = 2;
        public const int Archived = 3;
    }
}
