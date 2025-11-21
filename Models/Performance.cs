namespace FxInvestmentApi.Models
{
    public class Performance
    {
        public int Id { get; set; }
        public string FxId { get; set; } = string.Empty;
        public string? AccountBase { get; set; }
        public int Week { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Results { get; set; }
        public DateTime DateTime { get; set; }
        public string? Comments { get; set; }
        public string? FilePath { get; set; }
        public int? TotalTrades { get; set; }
        public decimal? TotalProfit { get; set; }
        public decimal? MaxWin { get; set; }
        public decimal? MinWin { get; set; }
        public string? AccountType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}