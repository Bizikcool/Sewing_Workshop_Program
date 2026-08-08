using Microsoft.AspNetCore.Mvc;
using SewingAPI.Domain.Enums;
using SewingAPI.Domain.Interfaces.Services;
using SewingAPI.Service.DTOs.Contragent;
using SewingAPI.Service.DTOs.Shared;
using SewingAPI.Service.Mapping;

namespace SewingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContragentsController : ControllerBase
    {
        private readonly IContragentService _contragents;

        public ContragentsController(IContragentService contragents)
        {
            _contragents = contragents;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<ContragentResponseDto>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] ContragentType? type = null)
        {
            var normalizedPage = NormalizePage(page);
            var normalizedPageSize = NormalizePageSize(pageSize);
            var (items, total) = await _contragents.GetPagedAsync(normalizedPage, normalizedPageSize, search, type);

            return Ok(new PagedResult<ContragentResponseDto>
            {
                Items = items.Select(ContragentMapper.ToResponse).ToList(),
                Total = total,
                Page = normalizedPage,
                PageSize = normalizedPageSize
            });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ContragentResponseDto>> GetById(int id)
        {
            var contragent = await _contragents.GetByIdAsync(id);
            return Ok(ContragentMapper.ToResponse(contragent));
        }

        [HttpPost]
        public async Task<ActionResult<ContragentResponseDto>> Create(ContragentCreateDto dto)
        {
            var contragent = await _contragents.CreateAsync(ContragentMapper.ToEntity(dto));
            var response = ContragentMapper.ToResponse(contragent);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ContragentResponseDto>> Update(int id, ContragentUpdateDto dto)
        {
            var contragent = await _contragents.UpdateAsync(id, ContragentMapper.ToEntity(dto));
            return Ok(ContragentMapper.ToResponse(contragent));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _contragents.DeleteAsync(id);
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
