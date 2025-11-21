using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using FxInvestmentApi.Data;

namespace FxInvestmentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DbTestController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DbTestController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("raw-connection")]
        public async Task<IActionResult> TestRawConnection()
        {
            var connectionString = "Server=db19057.databaseasp.net;Database=db19057;User Id=db19057;Password=Bt9-8Q_bz6F@;Encrypt=False;MultipleActiveResultSets=True;";

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                // Test basic query
                using var command = new SqlCommand("SELECT @@VERSION", connection);
                var version = await command.ExecuteScalarAsync();

                // Test if our tables exist
                using var tablesCommand = new SqlCommand(@"
                    SELECT TABLE_NAME 
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_TYPE = 'BASE TABLE'", connection);

                using var reader = await tablesCommand.ExecuteReaderAsync();
                var tables = new List<string>();
                while (await reader.ReadAsync())
                {
                    tables.Add(reader.GetString(0));
                }

                return Ok(new
                {
                    success = true,
                    message = "Raw SQL Connection Successful!",
                    version = version.ToString(),
                    tables = tables,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message,
                    connectionString = connectionString.Replace("Password=Bt9-8Q_bz6F@", "Password=***"), // Hide password
                    timestamp = DateTime.UtcNow
                });
            }
        }

        // GET: api/dbtest/entity-framework
        [HttpGet("entity-framework")]
        public async Task<IActionResult> TestEntityFramework()
        {
            try
            {
                // Test Accounts
                var accountsCount = await _context.Accounts.CountAsync();
                var sampleAccounts = await _context.Accounts.Take(5).ToListAsync();

                // Test Performance
                var performanceCount = await _context.Performance.CountAsync();
                var samplePerformance = await _context.Performance.Take(5).ToListAsync();

                // Test Transactions
                var transactionsCount = await _context.Transactions.CountAsync();
                var sampleTransactions = await _context.Transactions.Take(5).ToListAsync();

                return Ok(new
                {
                    success = true,
                    message = "Entity Framework Connection Successful!",
                    counts = new
                    {
                        accounts = accountsCount,
                        performance = performanceCount,
                        transactions = transactionsCount
                    },
                    sampleData = new
                    {
                        accounts = sampleAccounts,
                        performance = samplePerformance,
                        transactions = sampleTransactions
                    },
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message,
                    innerException = ex.InnerException?.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        // GET: api/dbtest/health
        [HttpGet("health")]
        public async Task<IActionResult> HealthCheck()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();
                var databaseName = _context.Database.GetDbConnection().Database;
                var dataSource = _context.Database.GetDbConnection().DataSource;

                return Ok(new
                {
                    status = "Healthy",
                    database = databaseName,
                    server = dataSource,
                    canConnect = canConnect,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "Unhealthy",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }
    }
}