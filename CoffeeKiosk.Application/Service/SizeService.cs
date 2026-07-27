using AutoMapper;
using CoffeeKiosk.Domain.Entities;
using CoffeeKiosk.Application.DTOs;
using CoffeeKiosk.Application.Interfaces;
using CoffeeKiosk.Application.Interfaces.Repository;
using CoffeeKiosk.Application.Interfaces.Service;

namespace CoffeeKiosk.Application.Service;

public class SizeService : ISizeService
{
    private readonly ISizeRepository _sizeRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    
    public SizeService(ISizeRepository sizeRepository
        , IMapper mapper
        , IUnitOfWork unitOfWork)
    {
        _sizeRepository = sizeRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<SizeResponseDto>> GetAllAsync(CancellationToken ct)
    {
        var getall = await _sizeRepository.GetAllAsync(ct);
        return _mapper.Map<List<SizeResponseDto>>(getall);
    }

    public async Task<SizeResponseDto> GetByIdAsync(int id, CancellationToken ct)
    {
         var getbyid = await _sizeRepository.GetByIdAsync(id, ct);
         if(getbyid == null)
             throw  new KeyNotFoundException("Not found");
         return _mapper.Map<SizeResponseDto>(getbyid);
    }

    public async Task<SizeResponseDto> CreateAsync(CreateSizeDto dto, CancellationToken ct)
    {
        var size = _mapper.Map<Size>(dto);
         await _sizeRepository.AddAsync(size, ct);
         await _unitOfWork.SaveChangesAsync(ct);
         return _mapper.Map<SizeResponseDto>(size);
    }

    public async Task<SizeResponseDto> UpdateAsync(int id, CreateSizeDto dto, CancellationToken ct)
    {
         var find = await _sizeRepository.GetByIdAsync(id, ct);
         if(find == null)
             throw  new KeyNotFoundException("Not found");
         
         _mapper.Map(dto, find);        
         _sizeRepository.Update(find);
         await _unitOfWork.SaveChangesAsync(ct);
         return _mapper.Map<SizeResponseDto>(find);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var find = await _sizeRepository.GetByIdAsync(id, ct);
        if(find == null)
            throw  new KeyNotFoundException("Not found");
        _sizeRepository.Delete(find);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}