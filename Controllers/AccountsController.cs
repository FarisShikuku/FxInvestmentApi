using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FxInvestmentApi.Data;
using FxInvestmentApi.Models;

namespace FxInvestmentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AccountsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/accounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            return await _context.Accounts.ToListAsync();
        }

        // GET: api/accounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccount(int id)
        {
            var account = await _context.Accounts.FindAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            return account;
        }

        // GET: api/accounts/by-accountid/AC1
        [HttpGet("by-accountid/{accountId}")]
        public async Task<ActionResult<Account>> GetAccountByAccountId(string accountId)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId);

            if (account == null)
            {
                return NotFound();
            }

            return account;
        }

        // GET: api/accounts/active
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Account>>> GetActiveAccounts()
        {
            return await _context.Accounts
                .Where(a => a.IsActive)
                .OrderBy(a => a.AccountName)
                .ToListAsync();
        }

        // GET: api/accounts/summary
        [HttpGet("summary")]
        public async Task<ActionResult<object>> GetAccountsSummary()
        {
            var summary = await _context.Accounts
                .GroupBy(a => a.Currency)
                .Select(g => new
                {
                    Currency = g.Key,
                    TotalAccounts = g.Count(),
                    TotalBalance = g.Sum(a => a.CurrentBalance),
                    ActiveAccounts = g.Count(a => a.IsActive)
                })
                .ToListAsync();

            var totalSummary = new
            {
                TotalAccounts = await _context.Accounts.CountAsync(),
                TotalBalance = await _context.Accounts.SumAsync(a => a.CurrentBalance),
                ActiveAccounts = await _context.Accounts.CountAsync(a => a.IsActive),
                ByCurrency = summary
            };

            return Ok(totalSummary);
        }

        // POST: api/accounts
        [HttpPost]
        public async Task<ActionResult<Account>> PostAccount(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAccount), new { id = account.Id }, account);
        }

        // PUT: api/accounts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccount(int id, Account account)
        {
            if (id != account.Id)
            {
                return BadRequest();
            }

            _context.Entry(account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
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

        // PATCH: api/accounts/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchAccount(int id, [FromBody] Dictionary<string, object> updates)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            foreach (var update in updates)
            {
                switch (update.Key.ToLower())
                {
                    case "accountname":
                        account.AccountName = update.Value?.ToString();
                        break;
                    case "currentbalance":
                        account.CurrentBalance = Convert.ToDecimal(update.Value);
                        break;
                    case "currency":
                        account.Currency = update.Value?.ToString();
                        break;
                    case "description":
                        account.Description = update.Value?.ToString();
                        break;
                    case "isactive":
                        account.IsActive = Convert.ToBoolean(update.Value);
                        break;
                }
            }

            account.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
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

        // DELETE: api/accounts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }
    }
}