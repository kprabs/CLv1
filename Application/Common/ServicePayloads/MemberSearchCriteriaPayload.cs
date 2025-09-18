using System.Globalization;

namespace CoreLib.Application.Common.ServicePayloads
{
    public class MemberSearchCriteriaPayload
    {
        private string? _firstName;
        private string? _lastName;
        private string? _ssn;
        private string? _memberId;
        private string? _dateOfBirth;
        private string? _accountNumber;
        private string? _subAccountNumber;
        private string? _alternateID;

        public string? LastName { get => string.IsNullOrEmpty(_lastName) ? String.Empty : Convert.ToString(_lastName, CultureInfo.InvariantCulture).Trim(); set => _lastName = value; }
        public string? FirstName { get => string.IsNullOrEmpty(_firstName) ? String.Empty : Convert.ToString(_firstName, CultureInfo.InvariantCulture).Trim(); set => _firstName = value; }
        public string? Ssn { get => string.IsNullOrEmpty(_ssn) ? String.Empty : Convert.ToString(_ssn, CultureInfo.InvariantCulture).Trim(); set => _ssn = value; }
        public string? Id { get => string.IsNullOrEmpty(_memberId) ? String.Empty : Convert.ToString(_memberId, CultureInfo.InvariantCulture).Trim(); set => _memberId = value; }
        public string? DateOfBirth { get => string.IsNullOrEmpty(_dateOfBirth) ? String.Empty : Convert.ToString(_dateOfBirth, CultureInfo.InvariantCulture).Trim(); set => _dateOfBirth = value; }
        public string? AccountNumber { get => string.IsNullOrEmpty(_accountNumber) ? String.Empty : Convert.ToString(_accountNumber, CultureInfo.InvariantCulture).Trim(); set => _accountNumber = value; }
        public string? SubAccountNumber { get => string.IsNullOrEmpty(_subAccountNumber) ? String.Empty : Convert.ToString(_subAccountNumber, CultureInfo.InvariantCulture).Trim(); set => _subAccountNumber = value; }
        public string? ClientId { get; set; }
        public List<string>? Status { get; set; }
        public string? AlternateID { get => string.IsNullOrEmpty(_alternateID) ? String.Empty : Convert.ToString(_alternateID, CultureInfo.InvariantCulture).Trim(); set => _alternateID = value; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }
        public string? OrderBy { get; set; }
        public string? Sorting { get; set; }
    }
}
