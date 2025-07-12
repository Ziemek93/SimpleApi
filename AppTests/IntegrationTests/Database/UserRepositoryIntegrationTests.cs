using FluentAssertions;
using MainApi.Context;
using MainApi.Models.Entities;
using MainApi.Repositories.User;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace AppTests.IntegrationTests.Database;

public class UserRepositoryIntegrationTests : IAsyncDisposable
{
    private readonly PostgreSqlContainer _postgreSqlContainer;
    private readonly ApplicationContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryIntegrationTests()
    {
        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15")
            .WithDatabase("testdb")
            .WithUsername("testuser")
            .WithPassword("testpassword")
            .Build();

        _postgreSqlContainer.StartAsync().Wait();

        var connectionString = _postgreSqlContainer.GetConnectionString();
        
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseNpgsql(connectionString)
            .Options;

        _context = new ApplicationContext(options);
        _context.Database.EnsureCreatedAsync().Wait();
        
        _repository = new UserRepository(_context);
    }

    [Fact]
    public async Task UserPairExist_WhenBothUsersExist_ShouldReturnTrue()
    {
        // Arrange
        var users = new List<User>
        {
            new() { UserId = 1, UserName = "user1" },
            new() { UserId = 2, UserName = "user2" }
        };
        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.UserPairExist(1, 2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task UserPairExist_WhenFirstUserDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var user = new User { UserId = 2, UserName = "user2" };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.UserPairExist(1, 2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UserPairExist_WhenSecondUserDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var user = new User { UserId = 1, UserName = "user1" };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.UserPairExist(1, 2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UserPairExist_WhenNeitherUserExists_ShouldReturnFalse()
    {
        // Act
        var result = await _repository.UserPairExist(1, 2);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(10, 20)]
    [InlineData(100, 200)]
    public async Task UserPairExist_WithDifferentUserIds_ShouldCheckCorrectUsers(int firstId, int secondId)
    {
        // Arrange
        var users = new List<User>
        {
            new() { UserId = firstId, UserName = $"user{firstId}" },
            new() { UserId = secondId, UserName = $"user{secondId}" }
        };
        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.UserPairExist(firstId, secondId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task UserPairExist_WithSameUserId_ShouldReturnTrue()
    {
        // Arrange
        var user = new User { UserId = 1, UserName = "user1" };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.UserPairExist(1, 1);

        // Assert
        result.Should().BeTrue();
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
        await _postgreSqlContainer.DisposeAsync();
    }
} 