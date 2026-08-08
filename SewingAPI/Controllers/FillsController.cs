using Microsoft.AspNetCore.Mvc;
using SewingAPI.Domain.Interfaces.Services;
using SewingAPI.Service.DTOs.Fill;
using SewingAPI.Service.DTOs.Shared;
using SewingAPI.Service.Mapping;

namespace SewingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FillsController : ControllerBase
    {
        private readonly IFillService _fills;

        public FillsController(IFillService fills)
        {
            _fills = fills;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<FillResponseDto>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? type = null,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null)
        {
            var normalizedPage = NormalizePage(page);
            var normalizedPageSize = NormalizePageSize(pageSize);
            var (items, total) = await _fills.GetPagedAsync(normalizedPage, normalizedPageSize, type, dateFrom, dateTo);

            return Ok(new PagedResult<FillResponseDto>
            {
                Items = items.Select(FillMapper.ToResponse).ToList(),
                Total = total,
                Page = normalizedPage,
                PageSize = normalizedPageSize
            });
        }

        [HttpPost]
        public async Task<ActionResult<FillResponseDto>> Create(FillCreateDto dto)
        {
            var fill = await _fills.CreateAsync(dto.Type, dto.Description, dto.Related);
            return Ok(FillMapper.ToResponse(fill));
        }

        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetStats()
        {
            var stats = await _fills.GetStatsAsync();
            return Ok(new
            {
                stats.TotalCount,
                stats.TodayCount
            });
        }

        private static int NormalizePage(int page) => page < 1 ? 1 : page;

        private static int NormalizePageSize(int pageSize) => pageSize switch
        {
            < 1 => 10,
            > 100 => 100,
            _ => pageSize
        };
    }
}
