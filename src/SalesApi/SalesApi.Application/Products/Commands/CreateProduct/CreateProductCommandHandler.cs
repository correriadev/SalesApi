using AutoMapper;
using MediatR;
using SalesApi.Domain.Entities;

namespace SalesApi.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductViewModel.Response>
{
    private readonly IMapper _mapper;
    // TODO: Add repository

    public CreateProductCommandHandler(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task<ProductViewModel.Response> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = _mapper.Map<Product>(request);
        product.CreatedAt = DateTime.UtcNow;

        // TODO: Save to repository

        return _mapper.Map<ProductViewModel.Response>(product);
    }
} 