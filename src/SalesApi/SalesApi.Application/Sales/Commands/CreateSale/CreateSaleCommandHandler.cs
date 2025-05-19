using AutoMapper;
using MediatR;
using SalesApi.Domain.Entities;
using SalesApi.ViewModel.V1.Sales;

namespace SalesApi.Application.Sales.Commands.CreateSale;

public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, SaleViewModel.Response>
{
    private readonly IMapper _mapper;
    // TODO: Add repository

    public CreateSaleCommandHandler(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task<SaleViewModel.Response> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = _mapper.Map<Sale>(request);
        // TODO: Save to repository

        return _mapper.Map<SaleViewModel.Response>(sale);
    }
} 