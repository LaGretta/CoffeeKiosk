using AutoMapper;
using CoffeeKiosk.Application.DTOs;
using CoffeeKiosk.Application.Interfaces;
using CoffeeKiosk.Application.Interfaces.Repository;
using CoffeeKiosk.Application.Interfaces.Service;
using CoffeeKiosk.Domain.Entities;
using CoffeeKiosk.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CoffeeKiosk.Application.Service;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _iorderrepo;
    private readonly IUnitOfWork _iunitOfWork;
    private readonly IAuthRepository _authRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IOrderRepository iorderrepo
        , IUnitOfWork iunitOfWork
        , IAuthRepository authRepository
        , IMapper mapper, ILogger<OrderService> logger)
    {
        _iorderrepo = iorderrepo;
        _iunitOfWork = iunitOfWork;
        _authRepository = authRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<OrderResponseDto> CreateAsync(int kioskUserId, CreateOrderDto dto, CancellationToken ct)
    {
        var findorder = await _iorderrepo.GetByIdAsync(kioskUserId, ct);
        if (findorder == null)
            throw new KeyNotFoundException("Order not found");
        if(findorder.Status != OrderStatus.Paid)
            throw new UnauthorizedAccessException("Invalid order status");
        var order = _mapper.Map<Order>(dto);
        await  _iorderrepo.AddAsync(order, ct);
        await _iunitOfWork.SaveChangesAsync(ct);
        
        
        _logger.LogInformation("Order {OrderNumber} created with {ItemCount} items, total {Total}",
            order.OrderNumber, order.Items.Count, order.TotalPrice);
        
        return _mapper.Map<OrderResponseDto>(order);
    }

    public async Task<OrderResponseDto> GetByNumberAsync(string orderNumber, CancellationToken ct)
    {
        var find = await _iorderrepo.GetByOrderNumberAsync(orderNumber, ct);
        if (find == null)
            throw new KeyNotFoundException("Order not found");
        return _mapper.Map<OrderResponseDto>(find);
    }

    public async Task<List<OrderResponseDto>> GetQueueAsync(CancellationToken ct)
    {
         var getqueue = await _iorderrepo.GetQueueAsync(ct);
         if (getqueue == null)
             throw new KeyNotFoundException("Queue not found");
         return _mapper.Map<List<OrderResponseDto>>(getqueue);
    }
    public async Task<OrderResponseDto> PayAsync(int orderId, CancellationToken ct)
    {
    var order = await _iorderrepo.GetByIdAsync(orderId, ct);
    if (order == null)
        throw new KeyNotFoundException("Order not found");

    if (order.Status != OrderStatus.Placed)
        throw new InvalidOperationException("Only placed orders can be paid");

    order.Status = OrderStatus.Paid;
    _iorderrepo.Update(order);
    await _iunitOfWork.SaveChangesAsync(ct);

    return _mapper.Map<OrderResponseDto>(order);
    }

    public async Task<OrderResponseDto> PrepareAsync(int orderId, CancellationToken ct)
    {
    var order = await _iorderrepo.GetByIdAsync(orderId, ct);
    if (order == null)
        throw new KeyNotFoundException("Order not found");

    if (order.Status != OrderStatus.Paid)
        throw new InvalidOperationException("Only paid orders can be prepared");

    order.Status = OrderStatus.Preparing;
    _iorderrepo.Update(order);
    await _iunitOfWork.SaveChangesAsync(ct);

    return _mapper.Map<OrderResponseDto>(order);
    }

    public async Task<OrderResponseDto> ReadyAsync(int orderId, CancellationToken ct)
    {
    var order = await _iorderrepo.GetByIdAsync(orderId, ct);
    if (order == null)
        throw new KeyNotFoundException("Order not found");

    if (order.Status != OrderStatus.Preparing)
        throw new InvalidOperationException("Only preparing orders can be marked ready");

    order.Status = OrderStatus.Ready;
    _iorderrepo.Update(order);
    await _iunitOfWork.SaveChangesAsync(ct);

    return _mapper.Map<OrderResponseDto>(order);
    }

    public async Task<OrderResponseDto> CompleteAsync(int orderId, CancellationToken ct)
    {
    var order = await _iorderrepo.GetByIdAsync(orderId, ct);
    if (order == null)
        throw new KeyNotFoundException("Order not found");

    if (order.Status != OrderStatus.Ready)
        throw new InvalidOperationException("Only ready orders can be completed");

    order.Status = OrderStatus.Completed;
    _iorderrepo.Update(order);
    await _iunitOfWork.SaveChangesAsync(ct);

    return _mapper.Map<OrderResponseDto>(order);
    }

    public async Task<OrderResponseDto> CancelAsync(int orderId, CancellationToken ct)
    {
    var order = await _iorderrepo.GetByIdAsync(orderId, ct);
    if (order == null)
        throw new KeyNotFoundException("Order not found");

    if (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Cancelled)
        throw new InvalidOperationException("Cannot cancel a completed or already cancelled order");

    order.Status = OrderStatus.Cancelled;
    _iorderrepo.Update(order);
    await _iunitOfWork.SaveChangesAsync(ct);

    return _mapper.Map<OrderResponseDto>(order);
    }
}