using AutoFixture;
using FluentAssertions;
using MainApi.Models;
using MainApi.Models.Enums;
using MainApi.Repositories.User;
using MainApi.Services.UserService;
using Moq;
using Xunit;

namespace AppTests.UnitTests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly UserService _userService;
    private readonly Fixture _fixture;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _userService = new UserService(_mockUserRepository.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task CheckChatUsersAsync_WhenUsersExist_ShouldReturnSuccessResult()
    {
        // Arrange
        var firstId = 1;
        var secondId = 2;
        _mockUserRepository.Setup(x => x.UserPairExist(firstId, secondId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.CheckChatUsersAsync(firstId, secondId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();
        _mockUserRepository.Verify(x => x.UserPairExist(firstId, secondId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CheckChatUsersAsync_WhenUsersDoNotExist_ShouldReturnFailureResult()
    {
        // Arrange
        var firstId = 1;
        var secondId = 2;
        _mockUserRepository.Setup(x => x.UserPairExist(firstId, secondId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _userService.CheckChatUsersAsync(firstId, secondId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Users don't exist");
        _mockUserRepository.Verify(x => x.UserPairExist(firstId, secondId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(10, 20)]
    [InlineData(100, 200)]
    public async Task CheckChatUsersAsync_WithDifferentUserIds_ShouldCallRepositoryWithCorrectParameters(int firstId, int secondId)
    {
        // Arrange
        _mockUserRepository.Setup(x => x.UserPairExist(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _userService.CheckChatUsersAsync(firstId, secondId);

        // Assert
        _mockUserRepository.Verify(x => x.UserPairExist(firstId, secondId, It.IsAny<CancellationToken>()), Times.Once);
    }
} 