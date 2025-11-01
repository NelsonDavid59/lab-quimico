namespace AgroLaboratorio.Utils
{
    public class LovRequest
    {
        public int Page { get; set; }
        public int PageSz { get; set; }
        public string? SearchField { get; set; }
        public string? SearchValue { get; set; }
        public string RscName { get; set; }
        public string? SortColumn { get; set; }
        public string SortDir { get; set; } = "ASC";

        public bool IsSearchable()
        {
            return !String.IsNullOrEmpty(SearchValue)
                && !String.IsNullOrEmpty(SearchField);
        }
    }
}
