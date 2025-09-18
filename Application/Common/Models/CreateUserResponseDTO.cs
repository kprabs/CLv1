namespace CoreLib.Application.Common.Models
{
    public class CreateUserResponseDTO
    {
        public CreateUserResponseDTO()
        {
            memberOfOrgIDs = [];
            frIndexedMultivalued3 = [];
            consentedMappings = [];
            frIndexedMultivalued4 = [];
            frIndexedMultivalued5 = [];
            frIndexedMultivalued1 = [];
            frIndexedMultivalued2 = [];
            effectiveAssignments = [];
            frUnindexedMultivalued1 = [];
            frUnindexedMultivalued3 = [];
            frUnindexedMultivalued2 = [];
            aliasList = [];
            frUnindexedMultivalued5 = [];
            frUnindexedMultivalued4 = [];
            kbaInfo = [];
            effectiveRoles = [];
            messages = [];
        }
        public string statusCode { get; set; }
        public IList<Message> messages { get; set; }
        public string _id { get; set; }
        public string _rev { get; set; }
        public string country { get; set; }
        public string frUnindexedString1 { get; set; }
        public string mail { get; set; }
        public IList<string> memberOfOrgIDs { get; set; }
        public string frIndexedDate5 { get; set; }
        public string frUnindexedString2 { get; set; }
        public string assignedDashboard { get; set; }
        public string frIndexedDate4 { get; set; }
        public string frUnindexedString3 { get; set; }
        public string frIndexedDate3 { get; set; }
        public string frUnindexedString4 { get; set; }
        public string postalCode { get; set; }
        public string frUnindexedString5 { get; set; }
        public string profileImage { get; set; }
        public string frIndexedString5 { get; set; }
        public string frIndexedString4 { get; set; }
        public string frIndexedString3 { get; set; }
        public string frIndexedString2 { get; set; }
        public string frIndexedString1 { get; set; }
        public IList<EmptyClass> frIndexedMultivalued3 { get; set; }
        public string frUnindexedInteger5 { get; set; }
        public IList<EmptyClass> consentedMappings { get; set; }
        public IList<EmptyClass> frIndexedMultivalued4 { get; set; }
        public string frUnindexedInteger4 { get; set; }
        public IList<EmptyClass> frIndexedMultivalued5 { get; set; }
        public string frUnindexedInteger3 { get; set; }
        public string frUnindexedInteger2 { get; set; }
        public IList<EffectiveGroups> effectiveGroups { get; set; }
        public IList<EmptyClass> frIndexedMultivalued1 { get; set; }
        public IList<EmptyClass> frIndexedMultivalued2 { get; set; }
        public string frUnindexedInteger1 { get; set; }
        public string givenName { get; set; }
        public string stateProvince { get; set; }
        public string postalAddress { get; set; }
        public string telephoneNumber { get; set; }
        public string city { get; set; }
        public string displayName { get; set; }
        public IList<EmptyClass> effectiveAssignments { get; set; }
        public string description { get; set; }
        public string accountStatus { get; set; }
        public string frUnindexedDate3 { get; set; }
        public IList<EmptyClass> frUnindexedMultivalued1 { get; set; }
        public IList<EmptyClass> frUnindexedMultivalued3 { get; set; }
        public string frUnindexedDate2 { get; set; }
        public string frUnindexedDate5 { get; set; }
        public IList<EmptyClass> frUnindexedMultÿivalued3 { get; set; }
        public string frUnindexedDate4 { get; set; }
        public IList<EmptyClass> frUnindexedMultivalued2 { get; set; }
        public IList<EmptyClass> aliasList { get; set; }
        public IList<EmptyClass> frUnindexedMultivalued5 { get; set; }
        public IList<EmptyClass> frUnindexedMultivalued4 { get; set; }
        public IList<EmptyClass> kbaInfo { get; set; }
        public string frIndexedInteger4 { get; set; }
        public string frIndexedInteger3 { get; set; }
        public string frIndexedInteger2 { get; set; }
        public string frIndexedInteger1 { get; set; }
        public string sn { get; set; }
        public string frUnindexedDate1 { get; set; }
        public string frIndexedInteger5 { get; set; }
        public string preferences { get; set; }
        public string userName { get; set; }
        public string frIndexedDate2 { get; set; }
        public string frIndexedDate1 { get; set; }
        public IList<EmptyClass> effectiveRoles { get; set; }
    }

    public class Message
    {
        public string messgage { get; set; }
    }

    public class EmptyClass
    {
    }

    public class EffectiveGroups
    {
        public string _refResourceCollection { get; set; }
        public string _refResourceId { get; set; }
        public string _ref { get; set; }
    }
}
