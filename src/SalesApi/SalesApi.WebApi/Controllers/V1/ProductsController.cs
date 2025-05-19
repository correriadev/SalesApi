using MediatR;
using Microsoft.AspNetCore.Mvc;
using SalesApi.Application.Products.Commands.CreateProduct;
using SalesApi.Application.Products.Queries.GetAllProducts;
using SalesApi.ViewModel.V1.Common;
using SalesApi.ViewModel.V1.Products;

namespace SalesApi.WebApi.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/products")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new product
    /// </summary>
    /// <param name="request">Product creation request</param>
    /// <returns>Created product</returns>
    /// <response code="201">Returns the newly created product</response>
    /// <response code="400">If the request is invalid</response>
    [HttpPost]
    [ProducesResponseType(typeof(ProductViewModel.Response), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<CreatedAtActionResult> Create([FromBody] ProductViewModel.Request request)
    {
        var command = new CreateProductCommand
        {
            Title = request.Title,
            Description = request.Description,
            Price = request.Price,
            Category = request.Category,
            Image = request.Image,
        };

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAll), result);
    }

    /// <summary>
    /// Gets all products
    /// </summary>
    /// <returns>List of products</returns>
    /// <response code="200">Returns the list of products</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductViewModel.Response>), StatusCodes.Status200OK)]
    public async Task<OkObjectResult> GetAll()
    {
        var query = new GetAllProductsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }
} 