namespace FxInvestmentApi.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string AccountId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "DEPOSIT" or "WITHDRAWAL"
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation property
        public virtual Account? Account { get; set; }
    }
}