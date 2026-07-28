using CoffeeKiosk.Application.Common;
using CoffeeKiosk.Application.DTOs;
using CoffeeKiosk.Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeKiosk.API.Controllers;

    [Authorize(Roles = Roles.Admin)]
    [ApiController]
    [Route("api/sizes")]
    public class SizesController : ControllerBase
    {
        private readonly ISizeService _sizeService;

        public SizesController(ISizeService sizeService)
        {
            _sizeService = sizeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSizes(CancellationToken ct)
        {
            var sizes = await _sizeService.GetAllAsync(ct);
            return Ok(sizes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSizeById(int id, CancellationToken ct)
        {
            var size = await _sizeService.GetByIdAsync(id, ct);
            return Ok(size);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSize(CreateSizeDto dto, CancellationToken ct)
        {
            var created = await _sizeService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetSizeById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSize(int id, CreateSizeDto dto, CancellationToken ct)
        {
            var updated = await _sizeService.UpdateAsync(id, dto, ct);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSize(int id, CancellationToken ct)
        {
            await _sizeService.DeleteAsync(id, ct);
            return NoContent();
        }
    }

    
    
        
