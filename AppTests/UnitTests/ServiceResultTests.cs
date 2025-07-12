using FluentAssertions;
using MainApi.Services;
using Xunit;

namespace AppTests.UnitTests;

public class ServiceResultTests
{
    [Fact]
    public void Success_WithData_ShouldReturnSuccessResult()
    {
        // Arrange
        var testData = "test data";

        // Act
        var result = ServiceResult<string>.Success(testData);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Data.Should().Be(testData);
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Failure_WithSingleError_ShouldReturnFailureResult()
    {
        // Arrange
        var errorMessage = "Test error message";

        // Act
        var result = ServiceResult<string>.Failure(errorMessage);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Data.Should().BeNull();
        result.Errors.Should().ContainSingle();
        result.Errors.First().Should().Be(errorMessage);
    }

    [Fact]
    public void Failure_WithMultipleErrors_ShouldReturnFailureResult()
    {
        // Arrange
        var errors = new[] { "Error 1", "Error 2", "Error 3" };

        // Act
        var result = ServiceResult<string>.Failure(errors);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Data.Should().BeNull();
        result.Errors.Should().HaveCount(3);
        result.Errors.Should().Contain("Error 1");
        result.Errors.Should().Contain("Error 2");
        result.Errors.Should().Contain("Error 3");
    }

    [Fact]
    public void ServiceResult_Success_ShouldReturnSuccessResult()
    {
        // Act
        var result = ServiceResult.Success();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ServiceResult_Failure_ShouldReturnFailureResult()
    {
        // Arrange
        var errorMessage = "Test error message";

        // Act
        var result = ServiceResult.Failure(errorMessage);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Data.Should().BeNull();
        result.Errors.Should().ContainSingle();
        result.Errors.First().Should().Be(errorMessage);
    }

    [Fact]
    public void ServiceResult_Failure_WithMultipleErrors_ShouldReturnFailureResult()
    {
        // Arrange
        var errors = new[] { "Error 1", "Error 2" };

        // Act
        var result = ServiceResult.Failure(errors);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Data.Should().BeNull();
        result.Errors.Should().HaveCount(2);
        result.Errors.Should().Contain("Error 1");
        result.Errors.Should().Contain("Error 2");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Failure_WithNullOrEmptyError_ShouldHandleGracefully(string errorMessage)
    {
        // Act
        var result = ServiceResult<string>.Failure(errorMessage);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().ContainSingle();
    }
} 