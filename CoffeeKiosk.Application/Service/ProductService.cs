using AutoMapper;
using CoffeeKiosk.Application.DTOs;
using CoffeeKiosk.Application.Interfaces;
using CoffeeKiosk.Application.Interfaces.Repository;
using CoffeeKiosk.Application.Interfaces.Service;
using CoffeeKiosk.Domain.Entities;

namespace CoffeeKiosk.Application.Service;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IProductRepository _productRepository;

    public ProductService(IUnitOfWork unitOfWork
        , IMapper mapper
        , IProductRepository productRepository)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _productRepository = productRepository;
    }

    public async Task<List<ProductResponseDto>> GetAllAsync(CancellationToken ct)
    {
        var getall = await _productRepository.GetAllProductsAsync(ct);
        return _mapper.Map<List<ProductResponseDto>>(getall);
    }

    public async Task<ProductResponseDto> GetByIdAsync(int id, CancellationToken ct)
    {
        var get = await _productRepository.GetProductAsync(id, ct);
        if(get == null)
            throw new KeyNotFoundException("Product not found");
        return _mapper.Map<ProductResponseDto>(get);
    }

    public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto, CancellationToken ct)
    {
         var product = _mapper.Map<Product>(dto);
         dto.IsAvailable = true;
          
         await _productRepository.CreateProductAsync(product, ct);
         await _unitOfWork.SaveChangesAsync(ct);
         return _mapper.Map<ProductResponseDto>(product);
    }

    public async Task<ProductResponseDto> UpdateAsync(int id, UpdateProductDto dto, CancellationToken ct)
    {
        var getforupdate = await _productRepository.GetProductAsync(id, ct);
        if(getforupdate == null)
            throw new KeyNotFoundException("Product not found");
        _mapper.Map(dto, getforupdate);
        _productRepository.UpdateProduct(getforupdate);
        await _unitOfWork.SaveChangesAsync(ct);
        return _mapper.Map<ProductResponseDto>(getforupdate);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
         var get  = await _productRepository.GetProductAsync(id, ct);
         if(get == null)
            throw new KeyNotFoundException("Product not found");
         _productRepository.DeleteProduct(get);
         await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<List<ProductResponseDto>> GetMenuAsync(CancellationToken ct)
    {
        var all = await _productRepository.GetAllProductsAsync(ct);
        var available = all.Where(p => p.IsAvailable).ToList();
        return _mapper.Map<List<ProductResponseDto>>(available);
    }
}