namespace CoreLib.Application.Common.Models
{
    public class UserDetailsDTO
    {
        public UserDetailsDTO()
        {
            UserSystemAccesses = [];
            UserNotes = [];
        }
        public string Country { get; set; }
        public string PostalAddress { get; set; }
        public string TelephoneNumber { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public bool Status { get; set; }
        public DateTime? LastLogInDateTime { get; set; }
        public string AgencyID { get; set; }

        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public IList<UserSystemAccessDTO> UserSystemAccesses { get; set; }
        public IList<UserNotesDTO> UserNotes { get; set; }
        public int? ClientCount { get; set; }
    }
}
