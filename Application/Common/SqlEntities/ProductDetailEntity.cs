
namespace CoreLib.Application.Common.SqlEntities
{
    public class ProductDetailEntity
    {
        public string? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? EffectiveDate { get; set; }
        public string? TerminationDate { get; set; }
        public string? LineOfBusiness { get; set; }
        public string? EligibleEmployeeCount { get; set; }
        public string? RenewalMonth { get; set; }
        public string? CoverageCategory { get; set; }
    }
}
