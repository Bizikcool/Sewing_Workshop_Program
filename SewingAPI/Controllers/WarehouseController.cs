using Microsoft.AspNetCore.Mvc;
using SewingAPI.Domain.Enums;
using SewingAPI.Domain.Interfaces.Services;
using SewingAPI.Service.DTOs.Shared;
using SewingAPI.Service.DTOs.Warehouse;
using SewingAPI.Service.Mapping;

namespace SewingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _warehouse;

        public WarehouseController(IWarehouseService warehouse)
        {
            _warehouse = warehouse;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<WarehouseResponseDto>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] ContragentType? contragentType = null,
            [FromQuery] MaterialType? materialType = null)
        {
            var normalizedPage = NormalizePage(page);
            var normalizedPageSize = NormalizePageSize(pageSize);
            var (items, total) = await _warehouse.GetPagedAsync(normalizedPage, normalizedPageSize, search, contragentType, materialType);

            return Ok(new PagedResult<WarehouseResponseDto>
            {
                Items = items.Select(WarehouseMapper.ToResponse).ToList(),
                Total = total,
                Page = normalizedPage,
                PageSize = normalizedPageSize
            });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<WarehouseResponseDto>> GetById(int id)
        {
            var item = await _warehouse.GetByIdAsync(id);
            return Ok(WarehouseMapper.ToResponse(item));
        }

        [HttpPut("{id:int}/quantity")]
        public async Task<ActionResult<WarehouseResponseDto>> UpdateQuantity(int id, WarehouseUpdateQtyDto dto)
        {
            var item = await _warehouse.UpdateQuantityAsync(id, dto.Qty);
            return Ok(WarehouseMapper.ToResponse(item));
        }

        [HttpPost("{id:int}/deduct")]
        public async Task<ActionResult<WarehouseResponseDto>> DeductQuantity(int id, WarehouseUpdateQtyDto dto)
        {
            var item = await _warehouse.DeductQuantityAsync(id, dto.Qty);
            return Ok(WarehouseMapper.ToResponse(item));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _warehouse.DeleteAsync(id);
            return NoContent();
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
