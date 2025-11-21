using Microsoft.EntityFrameworkCore;
using FxInvestmentApi.Models;

namespace FxInvestmentApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Performance> Performance { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Explicit table mappings for SQL Server
            modelBuilder.Entity<Account>().ToTable("accounts");
            modelBuilder.Entity<Performance>().ToTable("performance");
            modelBuilder.Entity<Transaction>().ToTable("transactions");

            // Configure Account entity
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.HasIndex(a => a.AccountId).IsUnique();

                // Explicit column mappings for SQL Server
                entity.Property(a => a.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(a => a.AccountId)
                    .HasColumnName("account_id")
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(a => a.AccountName)
                    .HasColumnName("account_name")
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(a => a.InitialDeposit)
                    .HasColumnName("initial_deposit")
                    .HasColumnType("decimal(15,2)");

                entity.Property(a => a.CurrentBalance)
                    .HasColumnName("current_balance")
                    .HasColumnType("decimal(15,2)")
                    .HasDefaultValue(0.00m);

                entity.Property(a => a.Currency)
                    .HasColumnName("currency")
                    .HasMaxLength(10)
                    .HasDefaultValue("USD");

                entity.Property(a => a.Description)
                    .HasColumnName("description");

                entity.Property(a => a.CreatedDate)
                    .HasColumnName("created_date")
                    .IsRequired();

                entity.Property(a => a.IsActive)
                    .HasColumnName("is_active")
                    .HasDefaultValue(true);

                entity.Property(a => a.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(a => a.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("GETDATE()");
            });

            // Configure Performance entity
            modelBuilder.Entity<Performance>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.HasIndex(p => p.FxId);
                entity.HasIndex(p => p.Week);
                entity.HasIndex(p => p.Year);
                entity.HasIndex(p => p.DateTime);
                entity.HasIndex(p => new { p.FxId, p.Week, p.Year });

                // Explicit column mappings
                entity.Property(p => p.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(p => p.FxId)
                    .HasColumnName("fxid")
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(p => p.AccountBase)
                    .HasColumnName("account_base")
                    .HasMaxLength(10);

                entity.Property(p => p.Week)
                    .HasColumnName("week")
                    .IsRequired();

                entity.Property(p => p.Month)
                    .HasColumnName("month")
                    .IsRequired();

                entity.Property(p => p.Year)
                    .HasColumnName("year")
                    .IsRequired();

                entity.Property(p => p.Results)
                    .HasColumnName("results")
                    .HasColumnType("decimal(15,2)")
                    .IsRequired();

                entity.Property(p => p.DateTime)
                    .HasColumnName("datetime")
                    .IsRequired();

                entity.Property(p => p.Comments)
                    .HasColumnName("comments");

                entity.Property(p => p.FilePath)
                    .HasColumnName("file_path")
                    .HasMaxLength(255);

                entity.Property(p => p.TotalTrades)
                    .HasColumnName("total_trades");

                entity.Property(p => p.TotalProfit)
                    .HasColumnName("total_profit")
                    .HasColumnType("decimal(15,2)");

                entity.Property(p => p.MaxWin)
                    .HasColumnName("max_win")
                    .HasColumnType("decimal(15,2)");

                entity.Property(p => p.MinWin)
                    .HasColumnName("min_win")
                    .HasColumnType("decimal(15,2)");

                entity.Property(p => p.AccountType)
                    .HasColumnName("account_type")
                    .HasMaxLength(50);

                entity.Property(p => p.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(p => p.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("GETDATE()");
            });

            // Configure Transaction entity
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.HasIndex(t => t.AccountId);
                entity.HasIndex(t => t.TransactionDate);

                // Explicit column mappings
                entity.Property(t => t.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(t => t.AccountId)
                    .HasColumnName("account_id")
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(t => t.Type)
                    .HasColumnName("type")
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(t => t.Amount)
                    .HasColumnName("amount")
                    .HasColumnType("decimal(15,2)")
                    .IsRequired();

                entity.Property(t => t.Description)
                    .HasColumnName("description");

                entity.Property(t => t.TransactionDate)
                    .HasColumnName("transaction_date")
                    .IsRequired();

                entity.Property(t => t.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("GETDATE()");

                // Configure relationship (optional - remove if causing issues)
                entity.HasOne(t => t.Account)
                    .WithMany(a => a.Transactions)
                    .HasForeignKey(t => t.AccountId)
                    .HasPrincipalKey(a => a.AccountId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}