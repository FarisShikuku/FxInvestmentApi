using Microsoft.AspNetCore.Mvc;

namespace FxInvestmentApi.Controllers
{
    [ApiController]
    [Route("/")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                message = "FxInvestment API is running!",
                version = "1.0",
                timestamp = DateTime.UtcNow,
                endpoints = new
                {
                    accounts = "/api/accounts",
                    performance = "/api/performance",
                    transactions = "/api/transactions",
                    swagger = "/swagger"
                }
            });
        }
    }
}