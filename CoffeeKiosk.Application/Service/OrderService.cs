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
    private readonly IProductRepository _productRepository;
    private readonly ISizeRepository _sizeRepository;

    public OrderService(IOrderRepository iorderrepo
        , IUnitOfWork iunitOfWork
        , IAuthRepository authRepository
        , IMapper mapper, ILogger<OrderService> logger,
        IProductRepository productRepository
        , ISizeRepository sizeRepository)
    {
        _iorderrepo = iorderrepo;
        _iunitOfWork = iunitOfWork;
        _authRepository = authRepository;
        _mapper = mapper;
        _logger = logger;
        _productRepository = productRepository;
        _sizeRepository = sizeRepository;
    }

    public async Task<OrderResponseDto> CreateAsync(int kioskUserId, CreateOrderDto dto, CancellationToken ct)
    {
        if (dto.Items == null || dto.Items.Count == 0)
            throw new InvalidOperationException("Order must contain at least one item");

        var order = new Order
        {
            Status = OrderStatus.Placed,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = kioskUserId,
            Items = new List<OrderItem>()
        };

        decimal total = 0;

        foreach (var itemDto in dto.Items)
        {
            var product = await _productRepository.GetProductAsync(itemDto.ProductId, ct);
            if (product == null)
                throw new KeyNotFoundException($"Product {itemDto.ProductId} not found");
            if (!product.IsAvailable)
                throw new InvalidOperationException("Product is not available");

            var size = await _sizeRepository.GetByIdAsync(itemDto.SizeId, ct);
            if (size == null)
                throw new KeyNotFoundException($"Size {itemDto.SizeId} not found");

            if (itemDto.Quantity <= 0)
                throw new InvalidOperationException("Quantity must be greater than zero");

            var unitPrice = product.BasePrice + size.PriceModifier;

            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                SizeId = size.Id,
                Quantity = itemDto.Quantity,
                UnitPrice = unitPrice
            });

            total += unitPrice * itemDto.Quantity;
        }

        order.TotalPrice = total;

        var todayCount = await _iorderrepo.GetTodayOrderCountAsync(ct);
        order.OrderNumber = (todayCount + 1).ToString();

        await _iorderrepo.AddAsync(order, ct);
        await _iunitOfWork.SaveChangesAsync(ct);

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