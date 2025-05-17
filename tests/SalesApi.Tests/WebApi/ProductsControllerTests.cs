using FluentAssertions;
using Xunit;
using NSubstitute;
using SalesApi.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SalesApi.Tests.WebApi;

public class ProductsControllerTests
{
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _controller = new ProductsController();
    }

    [Fact]
    public void Get_ShouldReturnOkResult()
    {
        // Act
        var result = _controller.Get();

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public void Post_ShouldReturnOkResult()
    {
        // Act
        var result = _controller.Post();

        // Assert
        result.Should().BeOfType<OkResult>();
    }
} 