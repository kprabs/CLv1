namespace CoreLib.Application.Common.Models
{
    public class UserSearchCriteriaDTO
    {
        public UserSearchCriteriaDTO()
        {
            criteria = [];
        }
        public IList<SearchCriteria> criteria { get; set; }
    }
    public class SearchCriteria
    {
        public string SearchField { get; set; }
        public string SearchValue { get; set; }
    }
}
