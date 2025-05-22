using MediatR;
using Microsoft.AspNetCore.Mvc;
using SalesApi.Application.Sales.Commands.CreateSale;
using SalesApi.Application.Sales.Queries.GetAllSales;
using SalesApi.ViewModel.V1.Common;
using SalesApi.ViewModel.V1.Sales;

namespace SalesApi.WebApi.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
public class SalesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SalesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new sale
    /// </summary>
    /// <param name="request">Sale creation request</param>
    /// <returns>Created sale</returns>
    /// <response code="201">Returns the newly created sale</response>
    /// <response code="400">If the request is invalid</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SaleViewModel.Response>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<SaleViewModel.Response>>> CreateSale(SaleViewModel.Request request)
    {
        var command = new CreateSaleCommand
        {
            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            Items = request.Items
        };

        var result = await _mediator.Send(command);
        return Ok(ApiResponse<SaleViewModel.Response>.Success(result, "Sale successfully created"));
    }

    /// <summary>
    /// Gets all sales
    /// </summary>
    /// <returns>List of sales</returns>
    /// <response code="200">Returns the list of sales</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SaleViewModel.Response>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<SaleViewModel.Response>>>> GetAllSales()
    {
        var query = new GetAllSalesQuery();
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<IEnumerable<SaleViewModel.Response>>.Success(result, "Sales retrieved successfully"));
    }

    /// <summary>
    /// Cancels a sale
    /// </summary>
    /// <param name="id">Sale ID</param>
    /// <returns>Cancellation result</returns>
    /// <response code="200">Sale cancelled successfully</response>
    /// <response code="404">Sale not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(SaleViewModel.CancelResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(Guid id)
    {
        // TODO: Implement sale cancellation logic
        return Ok(new SaleViewModel.CancelResponse());
    }
} 