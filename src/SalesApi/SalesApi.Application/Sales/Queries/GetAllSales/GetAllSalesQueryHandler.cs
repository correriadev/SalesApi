using AutoMapper;
using MediatR;
using SalesApi.Domain.Entities;
using SalesApi.Domain.Repositories;
using SalesApi.ViewModel.V1.Sales;

namespace SalesApi.Application.Sales.Queries.GetAllSales;

public class GetAllSalesQueryHandler : IRequestHandler<GetAllSalesQuery, IEnumerable<SaleViewModel.Response>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public GetAllSalesQueryHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SaleViewModel.Response>> Handle(GetAllSalesQuery request, CancellationToken cancellationToken)
    {
        var sales = await _saleRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<SaleViewModel.Response>>(sales);
    }
} 