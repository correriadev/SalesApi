using MediatR;
using SalesApi.ViewModel.V1.Products;

namespace SalesApi.Application.Products.Queries.GetAllProducts;

public record GetAllProductsQuery : IRequest<IEnumerable<ProductViewModel.Response>>; 