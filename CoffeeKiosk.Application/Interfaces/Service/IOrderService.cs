using CoffeeKiosk.Application.DTOs;

namespace CoffeeKiosk.Application.Interfaces.Service;

public interface IOrderService
{
    Task<OrderResponseDto> CreateAsync(int kioskUserId, CreateOrderDto dto, CancellationToken ct);
    Task<OrderResponseDto> GetByNumberAsync(string orderNumber, CancellationToken ct);
    Task<List<OrderResponseDto>> GetQueueAsync(CancellationToken ct);

    Task<OrderResponseDto> PayAsync(int orderId, CancellationToken ct);
    Task<OrderResponseDto> PrepareAsync(int orderId, CancellationToken ct);
    Task<OrderResponseDto> ReadyAsync(int orderId, CancellationToken ct);
    Task<OrderResponseDto> CompleteAsync(int orderId, CancellationToken ct);
    Task<OrderResponseDto> CancelAsync(int orderId, CancellationToken ct);
}