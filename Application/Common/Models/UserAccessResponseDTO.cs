using System.Runtime.Serialization;

namespace CoreLib.Application.Common.Models
{
    public class UserAccessResponseDTO
    {
        public UserAccessResponseDTO()
        {
            userAuthorizationAccessDetails = [];
            headers = [];
        }
        public Dictionary<string, string>? headers { get; set; }
        public Dictionary<string, string>? userInfo { get; set; }
        public IList<UserAuthorizationAccessDetail2DTO> userAuthorizationAccessDetails { get; set; }
        public UserAccessInfoDTO UserAccessInfo { get; set; }
        public UserDetailsInfoDTO UserDetails { get; set; }
        public string logtime { get; set; }
        public MigrationStatusRepesonseDto? clientMigrationStatus { get; set; }
    }    

    public class UserAuthorizationAccessDetail2DTO
    {
        private string? AccessTypeCodeField;

        private int AccessTypeIdField;

        private string? ClassifiedAreaCodeField;

        private string? ClassifiedAreaSegmentNKeyField;

        private string? ClassifiedAreaSegmentNameField;

        private string? ClassifiedSegmentCodeField;

        private int ClassifiedSegmentInstanceIdField;

        private int? ParentClassifiedAreaSegmentIdField;

        private bool SelectedForUserField;

        [DataMember]
        public string? AccessTypeCode
        {
            get
            {
                return AccessTypeCodeField;
            }
            set
            {
                AccessTypeCodeField = value;
            }
        }

        [DataMember]
        public int AccessTypeId
        {
            get
            {
                return AccessTypeIdField;
            }
            set
            {
                AccessTypeIdField = value;
            }
        }

        [DataMember]
        public string? ClassifiedAreaCode
        {
            get
            {
                return ClassifiedAreaCodeField;
            }
            set
            {
                ClassifiedAreaCodeField = value;
            }
        }

        [DataMember]
        public string? ClassifiedAreaSegmentNKey
        {
            get
            {
                return ClassifiedAreaSegmentNKeyField;
            }
            set
            {
                ClassifiedAreaSegmentNKeyField = value;
            }
        }

        [DataMember]
        public string? ClassifiedAreaSegmentName
        {
            get
            {
                return ClassifiedAreaSegmentNameField;
            }
            set
            {
                ClassifiedAreaSegmentNameField = value;
            }
        }

        [DataMember]
        public string? ClassifiedSegmentCode
        {
            get
            {
                return ClassifiedSegmentCodeField;
            }
            set
            {
                ClassifiedSegmentCodeField = value;
            }
        }

        [DataMember]
        public int ClassifiedSegmentInstanceId
        {
            get
            {
                return ClassifiedSegmentInstanceIdField;
            }
            set
            {
                ClassifiedSegmentInstanceIdField = value;
            }
        }

        [DataMember]
        public int? ParentClassifiedAreaSegmentId
        {
            get
            {
                return ParentClassifiedAreaSegmentIdField;
            }
            set
            {
                ParentClassifiedAreaSegmentIdField = value;
            }
        }

        [DataMember]
        public bool SelectedForUser
        {
            get
            {
                return SelectedForUserField;
            }
            set
            {
                SelectedForUserField = value;
            }
        }
    }
}

