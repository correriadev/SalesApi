using AutoMapper;
using MediatR;
using SalesApi.Domain.Entities;
using SalesApi.Domain.Repositories;
using SalesApi.ViewModel.V1.Sales;

namespace SalesApi.Application.Sales.Commands.CreateSale;

public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, SaleViewModel.Response>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateSaleCommandHandler(
        ISaleRepository saleRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _saleRepository = saleRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<SaleViewModel.Response> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        ValidateRequest(request);

        var sale = _mapper.Map<Sale>(request);
        
        await _saleRepository.AddAsync(sale, cancellationToken);
        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (result <= 0)
        {
            throw new Exception("Failed to save sale to the database.");
        }

        return _mapper.Map<SaleViewModel.Response>(sale);
    }

    private static void ValidateRequest(CreateSaleCommand request)
    {
        if (string.IsNullOrWhiteSpace(request.CustomerName))
        {
            throw new ArgumentException("Customer name cannot be empty.", nameof(request.CustomerName));
        }

        if (string.IsNullOrWhiteSpace(request.CustomerEmail))
        {
            throw new ArgumentException("Customer email cannot be empty.", nameof(request.CustomerEmail));
        }

        if (request.Items == null || !request.Items.Any())
        {
            throw new ArgumentException("Sale must have at least one item.", nameof(request.Items));
        }
    }
} 