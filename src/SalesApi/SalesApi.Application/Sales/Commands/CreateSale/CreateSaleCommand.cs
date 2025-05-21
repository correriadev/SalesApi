using MediatR;
using SalesApi.ViewModel.V1.Sales;

namespace SalesApi.Application.Sales.Commands.CreateSale;

public record CreateSaleCommand : IRequest<SaleViewModel.Response>
{
    public string CustomerName { get; init; } = string.Empty;
    public string CustomerEmail { get; init; } = string.Empty;
    public List<SaleItemViewModel> Items { get; init; } = new();
} 