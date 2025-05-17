using FluentAssertions;
using Xunit;
using NSubstitute;
using SalesApi.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SalesApi.Tests.WebApi;

public class SalesControllerTests
{
    private readonly SalesController _controller;

    public SalesControllerTests()
    {
        _controller = new SalesController();
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

    [Fact]
    public void Delete_ShouldReturnOkResult()
    {
        // Arrange
        var id = "test-id";

        // Act
        var result = _controller.Delete(id);

        // Assert
        result.Should().BeOfType<OkResult>();
    }
} 