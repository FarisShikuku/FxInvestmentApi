namespace FxInvestmentApi.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string AccountId { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public decimal InitialDeposit { get; set; }
        public decimal CurrentBalance { get; set; }
        public string Currency { get; set; } = "USD";
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation property
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}