using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FxInvestmentApi.Data;
using FxInvestmentApi.Models;

namespace FxInvestmentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PerformanceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PerformanceController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/performance
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Performance>>> GetPerformance()
        {
            return await _context.Performance.ToListAsync();
        }

        // GET: api/performance/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Performance>> GetPerformance(int id)
        {
            var performance = await _context.Performance.FindAsync(id);

            if (performance == null)
            {
                return NotFound();
            }

            return performance;
        }

        // GET: api/performance/by-fxid/FX123
        [HttpGet("by-fxid/{fxid}")]
        public async Task<ActionResult<IEnumerable<Performance>>> GetPerformanceByFxId(string fxid)
        {
            var performances = await _context.Performance
                .Where(p => p.FxId == fxid)
                .OrderByDescending(p => p.DateTime)
                .ToListAsync();

            return performances;
        }

        // GET: api/performance/week/42/year/2024
        [HttpGet("week/{week}/year/{year}")]
        public async Task<ActionResult<IEnumerable<Performance>>> GetPerformanceByWeek(int week, int year)
        {
            var performances = await _context.Performance
                .Where(p => p.Week == week && p.Year == year)
                .OrderBy(p => p.FxId)
                .ToListAsync();

            return performances;
        }

        // GET: api/performance/summary
        [HttpGet("summary")]
        public async Task<ActionResult<object>> GetPerformanceSummary()
        {
            var summary = await _context.Performance
                .GroupBy(p => p.FxId)
                .Select(g => new
                {
                    FxId = g.Key,
                    TotalResults = g.Sum(p => p.Results),
                    AverageResults = g.Average(p => p.Results),
                    RecordCount = g.Count(),
                    LastUpdated = g.Max(p => p.DateTime)
                })
                .ToListAsync();

            return Ok(summary);
        }

        // POST: api/performance
        [HttpPost]
        public async Task<ActionResult<Performance>> PostPerformance(Performance performance)
        {
            _context.Performance.Add(performance);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPerformance), new { id = performance.Id }, performance);
        }

        // PUT: api/performance/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPerformance(int id, Performance performance)
        {
            if (id != performance.Id)
            {
                return BadRequest();
            }

            _context.Entry(performance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PerformanceExists(id))
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

        // PATCH: api/performance/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchPerformance(int id, [FromBody] Dictionary<string, object> updates)
        {
            var performance = await _context.Performance.FindAsync(id);
            if (performance == null)
            {
                return NotFound();
            }

            foreach (var update in updates)
            {
                switch (update.Key.ToLower())
                {
                    case "results":
                        performance.Results = Convert.ToDecimal(update.Value);
                        break;
                    case "comments":
                        performance.Comments = update.Value?.ToString();
                        break;
                    case "filepath":
                        performance.FilePath = update.Value?.ToString();
                        break;
                    case "totaltrades":
                        performance.TotalTrades = Convert.ToInt32(update.Value);
                        break;
                    case "totalprofit":
                        performance.TotalProfit = Convert.ToDecimal(update.Value);
                        break;
                    case "maxwin":
                        performance.MaxWin = Convert.ToDecimal(update.Value);
                        break;
                    case "minwin":
                        performance.MinWin = Convert.ToDecimal(update.Value);
                        break;
                    case "accounttype":
                        performance.AccountType = update.Value?.ToString();
                        break;
                    case "week":
                        performance.Week = Convert.ToInt32(update.Value);
                        break;
                    case "month":
                        performance.Month = Convert.ToInt32(update.Value);
                        break;
                    case "year":
                        performance.Year = Convert.ToInt32(update.Value);
                        break;
                    case "datetime":
                        performance.DateTime = Convert.ToDateTime(update.Value);
                        break;
                }
            }

            performance.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PerformanceExists(id))
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

        // DELETE: api/performance/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerformance(int id)
        {
            var performance = await _context.Performance.FindAsync(id);
            if (performance == null)
            {
                return NotFound();
            }

            _context.Performance.Remove(performance);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PerformanceExists(int id)
        {
            return _context.Performance.Any(e => e.Id == id);
        }
    }
}