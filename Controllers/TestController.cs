using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FxInvestmentApi.Data;

namespace FxInvestmentApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TestController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("connection")]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();
                return Ok(new
                {
                    success = true,
                    message = "Entity Framework connection successful",
                    connected = canConnect,
                    databaseType = "SQL Server",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Entity Framework connection failed",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        [HttpGet("tables")]
        public async Task<IActionResult> CheckTables()
        {
            try
            {
                // Use SQL Server specific query
                var tables = await _context.Database.SqlQueryRaw<string>(
                    "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'"
                ).ToListAsync();

                return Ok(new
                {
                    success = true,
                    databaseType = "SQL Server",
                    tables = tables,
                    tableCount = tables.Count,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }
        [HttpGet("simple-test")]
        public async Task<IActionResult> SimpleTest()
        {
            try
            {
                // FIXED: Use proper SQL Server syntax
                var result = await _context.Database.SqlQueryRaw<int>(
                    "SELECT 1 as TestValue"
                ).FirstOrDefaultAsync();

                return Ok(new
                {
                    success = true,
                    message = "Simple query executed successfully",
                    testResult = result,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }
    }
}