using System.Globalization;

namespace CoreLib.Application.Common.ServicePayloads
{
    public class MemberInformationPayload
    {
        private string? _clientId;
        private string? _memberId;
        private string? _firstName;
        private string? _dateOfBirth;
        private string? _accountId;
        private string? _subAccountId;

        public string? subAccountId
        {
            get => string.IsNullOrEmpty(_subAccountId) ? string.Empty : Convert.ToString(_subAccountId, CultureInfo.InvariantCulture).Trim();
            set => _subAccountId = value;
        }

        public string? accountId
        {
            get => string.IsNullOrEmpty(_accountId) ? string.Empty : Convert.ToString(_accountId, CultureInfo.InvariantCulture).Trim();
            set => _accountId = value;
        }

        public string? clientId
        {
            get => string.IsNullOrEmpty(_clientId) ? string.Empty : Convert.ToString(_clientId, CultureInfo.InvariantCulture).Trim();
            set => _clientId = value;
        }
        public string? memberId
        {
            get => string.IsNullOrEmpty(_memberId) ? string.Empty : Convert.ToString(_memberId, CultureInfo.InvariantCulture).Trim();
            set => _memberId = value;
        }
        public string? dateOfBirth
        {
            get => string.IsNullOrEmpty(_dateOfBirth) ? string.Empty : Convert.ToString(_dateOfBirth, CultureInfo.InvariantCulture).Trim();
            set => _dateOfBirth = value;
        }
        public string? firstName
        {
            get => string.IsNullOrEmpty(_firstName) ? String.Empty : Convert.ToString(_firstName, CultureInfo.InvariantCulture).Trim();
            set => _firstName = value;
        }
        public string? brand { get; set; }
        public string? sourceSystem { get; set; }
    }
}
