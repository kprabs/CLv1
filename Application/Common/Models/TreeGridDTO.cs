namespace CoreLib.Application.Common.Models
{
    public class TreeGridDTO<T>
    {
        public TreeGridDTO()
        {
            LevelHeaders = [];
            ValueHeaders = [];
            Rows = [];
        }
        public IList<TreeGridHeaderDTO> LevelHeaders { get; set; }
        public IList<TreeGridHeaderDTO> ValueHeaders { get; set; }
        public IList<TreeGridRowDTO<T>> Rows { get; set; }
    }
}
