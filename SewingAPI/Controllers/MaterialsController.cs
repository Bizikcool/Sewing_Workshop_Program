using Microsoft.AspNetCore.Mvc;
using SewingAPI.Domain.Interfaces.Services;
using SewingAPI.Service.DTOs.Material;
using SewingAPI.Service.DTOs.Shared;
using SewingAPI.Service.Mapping;

namespace SewingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaterialsController : ControllerBase
    {
        private readonly IMaterialService _materials;

        public MaterialsController(IMaterialService materials)
        {
            _materials = materials;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<MaterialResponseDto>>> GetAll(
            [FromQuery] int contragentId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var normalizedPage = NormalizePage(page);
            var normalizedPageSize = NormalizePageSize(pageSize);
            var (items, total) = await _materials.GetPagedByContragentAsync(contragentId, normalizedPage, normalizedPageSize);

            return Ok(new PagedResult<MaterialResponseDto>
            {
                Items = items.Select(MaterialMapper.ToResponse).ToList(),
                Total = total,
                Page = normalizedPage,
                PageSize = normalizedPageSize
            });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MaterialResponseDto>> GetById(int id)
        {
            var material = await _materials.GetByIdAsync(id);
            return Ok(MaterialMapper.ToResponse(material));
        }

        [HttpPost]
        public async Task<ActionResult<MaterialResponseDto>> Create(MaterialCreateDto dto)
        {
            var material = await _materials.CreateAsync(MaterialMapper.ToEntity(dto));
            var response = MaterialMapper.ToResponse(material);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<MaterialResponseDto>> Update(int id, MaterialUpdateDto dto)
        {
            var material = await _materials.UpdateAsync(id, MaterialMapper.ToEntity(dto));
            return Ok(MaterialMapper.ToResponse(material));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _materials.DeleteAsync(id);
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
