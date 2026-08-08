using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SewingAPI.Repo.Data;

namespace SewingAPI.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public HealthController(AppDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [HttpGet("database")]
        public async Task<IActionResult> CheckDatabase()
        {
            try
            {
                var canConnect = await _dbContext.Database.CanConnectAsync();

                return Ok(new
                {
                    canConnect,
                    provider = _dbContext.Database.ProviderName,
                    database = _dbContext.Database.GetDbConnection().Database,
                    dataSource = _dbContext.Database.GetDbConnection().DataSource,
                    connectionStringConfigured = !string.IsNullOrWhiteSpace(_configuration.GetConnectionString("DefaultConnection")),
                    message = canConnect
                        ? "PostgreSQL connection is OK."
                        : "PostgreSQL server is reachable, but database connection failed."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    canConnect = false,
                    provider = _dbContext.Database.ProviderName,
                    database = _dbContext.Database.GetDbConnection().Database,
                    dataSource = _dbContext.Database.GetDbConnection().DataSource,
                    connectionStringConfigured = !string.IsNullOrWhiteSpace(_configuration.GetConnectionString("DefaultConnection")),
                    message = ex.Message
                });
            }
        }
    }
}
