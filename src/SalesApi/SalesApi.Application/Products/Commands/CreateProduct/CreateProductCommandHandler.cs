using AutoMapper;
using MediatR;
using SalesApi.Domain.Entities;
using SalesApi.Domain.Interfaces;
using SalesApi.Domain.Repositories;
using SalesApi.ViewModel.V1.Products;

namespace SalesApi.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductViewModel.Response>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IProductPublisher _productPublisher;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IProductPublisher productPublisher)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _productPublisher = productPublisher;
    }

    public async Task<ProductViewModel.Response> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = _mapper.Map<Product>(request);
        
        await _productRepository.AddAsync(product);
        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (result <= 0)
        {
            throw new Exception("Failed to save product to the database.");
        }

        await _productPublisher.PublishCreateProductAsync(product);

        return _mapper.Map<ProductViewModel.Response>(product);
    }
} 