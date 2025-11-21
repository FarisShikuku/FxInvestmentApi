using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FxInvestmentApi.Data;
using FxInvestmentApi.Models;

namespace FxInvestmentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TransactionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            return await _context.Transactions
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        // GET: api/transactions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null)
            {
                return NotFound();
            }

            return transaction;
        }

        // GET: api/transactions/by-account/AC1
        [HttpGet("by-account/{accountId}")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactionsByAccount(string accountId)
        {
            var transactions = await _context.Transactions
                .Where(t => t.AccountId == accountId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            return transactions;
        }

        // GET: api/transactions/by-type/DEPOSIT
        [HttpGet("by-type/{type}")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactionsByType(string type)
        {
            var transactions = await _context.Transactions
                .Where(t => t.Type.ToUpper() == type.ToUpper())
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            return transactions;
        }

        // GET: api/transactions/date-range
        [HttpGet("date-range")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactionsByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var transactions = await _context.Transactions
                .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            return transactions;
        }

        // GET: api/transactions/summary/AC1
        [HttpGet("summary/{accountId}")]
        public async Task<ActionResult<object>> GetTransactionSummary(string accountId)
        {
            var deposits = await _context.Transactions
                .Where(t => t.AccountId == accountId && t.Type == "DEPOSIT")
                .SumAsync(t => t.Amount);

            var withdrawals = await _context.Transactions
                .Where(t => t.AccountId == accountId && t.Type == "WITHDRAWAL")
                .SumAsync(t => t.Amount);

            return Ok(new
            {
                AccountId = accountId,
                TotalDeposits = deposits,
                TotalWithdrawals = withdrawals,
                NetAmount = deposits - withdrawals
            });
        }

        // POST: api/transactions
        [HttpPost]
        public async Task<ActionResult<Transaction>> PostTransaction(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            // Update account balance
            await UpdateAccountBalance(transaction.AccountId, transaction.Type, transaction.Amount);

            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
        }

        // PUT: api/transactions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransaction(int id, Transaction transaction)
        {
            if (id != transaction.Id)
            {
                return BadRequest();
            }

            // Get the original transaction to calculate balance changes
            var originalTransaction = await _context.Transactions.AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (originalTransaction == null)
            {
                return NotFound();
            }

            _context.Entry(transaction).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                // Recalculate account balance since transaction changed
                await RecalculateAccountBalance(transaction.AccountId);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // PATCH: api/transactions/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchTransaction(int id, [FromBody] Dictionary<string, object> updates)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            var originalAmount = transaction.Amount;
            var originalType = transaction.Type;
            var originalAccountId = transaction.AccountId;

            foreach (var update in updates)
            {
                switch (update.Key.ToLower())
                {
                    case "amount":
                        transaction.Amount = Convert.ToDecimal(update.Value);
                        break;
                    case "type":
                        transaction.Type = update.Value?.ToString()?.ToUpper();
                        break;
                    case "description":
                        transaction.Description = update.Value?.ToString();
                        break;
                    case "transactiondate":
                        transaction.TransactionDate = Convert.ToDateTime(update.Value);
                        break;
                    case "accountid":
                        transaction.AccountId = update.Value?.ToString();
                        break;
                }
            }

            try
            {
                await _context.SaveChangesAsync();

                // Recalculate balances for both original and new account if account changed
                if (updates.ContainsKey("accountid") && originalAccountId != transaction.AccountId)
                {
                    await RecalculateAccountBalance(originalAccountId);
                    await RecalculateAccountBalance(transaction.AccountId);
                }
                else if (updates.ContainsKey("amount") || updates.ContainsKey("type"))
                {
                    // If amount or type changed, recalculate balance
                    await RecalculateAccountBalance(transaction.AccountId);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/transactions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            var accountId = transaction.AccountId;

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            // Recalculate account balance after deletion
            await RecalculateAccountBalance(accountId);

            return NoContent();
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.Id == id);
        }

        private async Task UpdateAccountBalance(string accountId, string type, decimal amount)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId);
            if (account != null)
            {
                if (type == "DEPOSIT")
                {
                    account.CurrentBalance += amount;
                }
                else if (type == "WITHDRAWAL")
                {
                    account.CurrentBalance -= amount;
                }

                account.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        private async Task RecalculateAccountBalance(string accountId)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId);
            if (account != null)
            {
                var deposits = await _context.Transactions
                    .Where(t => t.AccountId == accountId && t.Type == "DEPOSIT")
                    .SumAsync(t => (decimal?)t.Amount) ?? 0;

                var withdrawals = await _context.Transactions
                    .Where(t => t.AccountId == accountId && t.Type == "WITHDRAWAL")
                    .SumAsync(t => (decimal?)t.Amount) ?? 0;

                account.CurrentBalance = account.InitialDeposit + deposits - withdrawals;
                account.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }
        }
    }
}