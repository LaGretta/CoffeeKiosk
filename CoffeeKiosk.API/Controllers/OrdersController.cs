using System.Security.Claims;
using CoffeeKiosk.Application.Common;
using CoffeeKiosk.Application.DTOs;
using CoffeeKiosk.Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeKiosk.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [Authorize(Roles = Roles.KioskOrStaff)]
    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderDto dto, CancellationToken ct)
    {
        var userId = GetUserId();
        var created = await _orderService.CreateAsync(userId, dto, ct);
        return CreatedAtAction(nameof(GetByNumber), new { number = created.OrderNumber }, created);
    }

    [AllowAnonymous]
    [HttpGet("{number}")]
    public async Task<IActionResult> GetByNumber(string number, CancellationToken ct)
    {
        var order = await _orderService.GetByNumberAsync(number, ct);
        return Ok(order);
    }

    [Authorize(Roles = Roles.StaffOrAdmin)]
    [HttpGet("queue")]
    public async Task<IActionResult> GetQueue(CancellationToken ct)
    {
        var queue = await _orderService.GetQueueAsync(ct);
        return Ok(queue);
    }

    [Authorize(Roles = Roles.KioskOrStaff)]
    [HttpPatch("{id}/pay")]
    public async Task<IActionResult> Pay(int id, CancellationToken ct)
    {
        var result = await _orderService.PayAsync(id, ct);
        return Ok(result);
    }

    [Authorize(Roles = Roles.Staff)]
    [HttpPatch("{id}/prepare")]
    public async Task<IActionResult> Prepare(int id, CancellationToken ct)
    {
        var result = await _orderService.PrepareAsync(id, ct);
        return Ok(result);
    }

    [Authorize(Roles = Roles.Staff)]
    [HttpPatch("{id}/ready")]
    public async Task<IActionResult> Ready(int id, CancellationToken ct)
    {
        var result = await _orderService.ReadyAsync(id, ct);
        return Ok(result);
    }

    [Authorize(Roles = Roles.Staff)]
    [HttpPatch("{id}/complete")]
    public async Task<IActionResult> Complete(int id, CancellationToken ct)
    {
        var result = await _orderService.CompleteAsync(id, ct);
        return Ok(result);
    }

    [Authorize(Roles = Roles.StaffOrAdmin)]
    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id, CancellationToken ct)
    {
        var result = await _orderService.CancelAsync(id, ct);
        return Ok(result);
    }

    private int GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null)
            throw new UnauthorizedAccessException("User id not found in token");
        return int.Parse(claim.Value);
    }
}