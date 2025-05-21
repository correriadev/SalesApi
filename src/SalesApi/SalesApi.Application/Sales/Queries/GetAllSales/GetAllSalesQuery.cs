using MediatR;
using SalesApi.ViewModel.V1.Sales;

namespace SalesApi.Application.Sales.Queries.GetAllSales;

public record GetAllSalesQuery : IRequest<IEnumerable<SaleViewModel.Response>>; 