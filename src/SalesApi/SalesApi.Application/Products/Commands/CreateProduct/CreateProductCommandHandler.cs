using AutoMapper;
using MediatR;
using SalesApi.Domain.Entities;
using SalesApi.Domain.Repositories;
using SalesApi.Domain.ValueObjects;
using SalesApi.ViewModel.V1.Products;
using System;

namespace SalesApi.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductViewModel.Response>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ProductViewModel.Response> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        ValidateRequest(request);

        var product = _mapper.Map<Product>(request);
        
        await _productRepository.AddAsync(product);
        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (result <= 0)
        {
            throw new Exception("Failed to save product to the database.");
        }

        return _mapper.Map<ProductViewModel.Response>(product);
    }

    private static void ValidateRequest(CreateProductCommand request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException("Product title cannot be empty.", nameof(request.Title));
        }

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            throw new ArgumentException("Product description cannot be empty.", nameof(request.Description));
        }

        if (string.IsNullOrWhiteSpace(request.Category))
        {
            throw new ArgumentException("Product category cannot be empty.", nameof(request.Category));
        }

        if (string.IsNullOrWhiteSpace(request.Image))
        {
            throw new ArgumentException("Product image cannot be empty.", nameof(request.Image));
        }

        if (request.Price < 0)
        {
            throw new ArgumentException("Product price cannot be negative.", nameof(request.Price));
        }
    }
} 