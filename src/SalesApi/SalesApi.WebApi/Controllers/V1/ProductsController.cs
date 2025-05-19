using Microsoft.AspNetCore.Mvc;
using SalesApi.ViewModel.V1.Products;
using SalesApi.ViewModel.V1.Common;

namespace SalesApi.WebApi.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/products")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(ILogger<ProductsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Creates a new product
    /// </summary>
    /// <param name="request">Product creation request</param>
    /// <returns>Created product</returns>
    /// <response code="202">Product creation request accepted</response>
    /// <response code="400">Invalid request data</response>
    [HttpPost]
    [ProducesResponseType(typeof(ProductViewModel.Response), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] ProductViewModel.Request request)
    {
        // TODO: Implement product creation logic
        return Accepted();
    }

    /// <summary>
    /// Gets all products
    /// </summary>
    /// <returns>List of products</returns>
    /// <response code="200">Returns the list of products</response>
    [HttpGet]
    [ProducesResponseType(typeof(ProductViewModel.ListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        // TODO: Implement get all products logic
        return Ok(new ProductViewModel.ListResponse());
    }
} 