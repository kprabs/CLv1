namespace CoreLib.Application.Common.Models
{
    public class CreateUserRequestDTO
    {
        public string? userName { get; set; }
        public string? givenName { get; set; }
        public string? sn { get; set; }
        public string? mail { get; set; }

        public IList<Groups> groups { get; set; }
        public IList<MemberOfOrg> memberOfOrg { get; set; }
    }

    public class Groups
    {
        public string? _ref { get; set; }
    }

    public class MemberOfOrg
    {
        public string? _ref { get; set; }
    }
}
