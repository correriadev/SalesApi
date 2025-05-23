using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SalesApi.Application.Products.Commands.CreateProduct;
using SalesApi.Application.Products.Queries.GetAllProducts;
using SalesApi.ViewModel.V1.Common;
using SalesApi.ViewModel.V1.Products;

namespace SalesApi.WebApi.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public ProductsController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Creates a new product
    /// </summary>
    /// <param name="request">Product creation request</param>
    /// <returns>Created product</returns>
    /// <response code="201">Returns the newly created product</response>
    /// <response code="400">If the request is invalid</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProductViewModel.Response>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ProductViewModel.Response>>> CreateProduct(ProductViewModel.Request request)
    {
        var command = _mapper.Map<CreateProductCommand>(request);
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<ProductViewModel.Response>.Success(result, "Product successfully created"));
    }

    /// <summary>
    /// Gets all products
    /// </summary>
    /// <returns>List of products</returns>
    /// <response code="200">Returns the list of products</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductViewModel.Response>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductViewModel.Response>>>> GetAllProducts()
    {
        var query = new GetAllProductsQuery();
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<IEnumerable<ProductViewModel.Response>>.Success(result, "Products retrieved successfully"));
    }
} 