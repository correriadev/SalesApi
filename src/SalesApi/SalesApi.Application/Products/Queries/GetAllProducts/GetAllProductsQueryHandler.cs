using AutoMapper;
using MediatR;
using SalesApi.Domain.Entities;

namespace SalesApi.Application.Products.Queries.GetAllProducts;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductViewModel.Response>>
{
    private readonly IMapper _mapper;
    // TODO: Add repository

    public GetAllProductsQueryHandler(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductViewModel.Response>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        // TODO: Get from repository
        var products = new List<Product>();
        
        return _mapper.Map<IEnumerable<ProductViewModel.Response>>(products);
    }
} 