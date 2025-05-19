using AutoMapper;
using MediatR;
using SalesApi.Domain.Entities;
using SalesApi.ViewModel.V1.Sales;

namespace SalesApi.Application.Sales.Queries.GetAllSales;

public class GetAllSalesQueryHandler : IRequestHandler<GetAllSalesQuery, IEnumerable<SaleViewModel.Response>>
{
    private readonly IMapper _mapper;
    // TODO: Add repository

    public GetAllSalesQueryHandler(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task<IEnumerable<SaleViewModel.Response>> Handle(GetAllSalesQuery request, CancellationToken cancellationToken)
    {
        // TODO: Get from repository
        var sales = new List<Sale>();
        
        return _mapper.Map<IEnumerable<SaleViewModel.Response>>(sales);
    }
} 