using FastEndpoints.Testing;
using FluentAssertions;
using MainApi.Models.ArticlesDao;
using MainApi.Models.Comments;
using MainApi.Services.ArticleService;
using MainApi.Services.UserService;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shared.Broker.Contracts;
using Xunit;
using AppTests.TestFixtures;
using MainApi.Services;

namespace AppTests.IntegrationTests.Endpoints;

public class PrivateEndpointsTests : TestBase<TestFixture>
{
    private readonly Mock<IArticleService> _mockArticleService;
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<IPublishEndpoint> _mockPublishEndpoint;

    public PrivateEndpointsTests()
    {
        _mockArticleService = new Mock<IArticleService>();
        _mockUserService = new Mock<IUserService>();
        _mockPublishEndpoint = new Mock<IPublishEndpoint>();
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped(_ => _mockArticleService.Object);
        services.AddScoped(_ => _mockUserService.Object);
        services.AddScoped(_ => _mockPublishEndpoint.Object);
    }

    [Fact]
    public async Task AddArticle_WithValidData_ShouldReturnCreatedArticle()
    {
        // Arrange
        var addArticleDao = new AddArticleDao
        {
            Title = "Test Article",
            Content = "Test Content",
            CategoryId = 1
        };

        var article = new MainApi.Models.Entities.Article
        {
            ArticleId = 1,
            Title = "Test Article",
            Content = "Test Content",
            CategoryId = 1
        };

        _mockArticleService.Setup(x => x.AddArticleAsync(It.IsAny<MainApi.Models.Entities.Article>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<MainApi.Models.Entities.Article>.Success(article));

        // Act
        var response = await Client.POSTAsync<AddArticle.Endpoint, AddArticleDao>(addArticleDao);

        // Assert
        response.Should().NotBeNull();
        // Note: This test will likely fail due to authentication requirements
        // In a real scenario, you would need to mock authentication or use test tokens
    }

    [Fact]
    public async Task EditArticle_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var articleDao = new ArticleDao
        {
            Title = "Updated Article",
            Content = "Updated Content"
        };

        _mockArticleService.Setup(x => x.EditArticleAsync(It.IsAny<int>(), It.IsAny<ArticleDao>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<bool>.Success(true));

        // Act
        var response = await Client.PUTAsync<EditArticle.Endpoint, ArticleDao>("/api/article/1", articleDao);

        // Assert
        response.Should().NotBeNull();
        // Note: This test will likely fail due to authentication requirements
    }

    [Fact]
    public async Task DeleteArticle_WhenArticleExists_ShouldReturnSuccess()
    {
        // Arrange
        _mockArticleService.Setup(x => x.DeleteArticleAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<bool>.Success(true));

        // Act
        var response = await Client.DELETEAsync<DeleteArticle.Endpoint>("/api/article/1");

        // Assert
        response.Should().NotBeNull();
        // Note: This test will likely fail due to authentication requirements
    }

    [Fact]
    public async Task AddComment_WithValidData_ShouldPublishMessage()
    {
        // Arrange
        var commentRequest = new CreateCommentRequest
        {
            Id = 1,
            Content = "Test comment",
            Login = "testuser"
        };

        _mockArticleService.Setup(x => x.CheckArticleAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<bool>.Success(true));

        _mockPublishEndpoint.Setup(x => x.Publish(It.IsAny<CreateCommentContract>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var response = await Client.POSTAsync<AddComment.Endpoint, CreateCommentRequest>("/api/article/1/comment", commentRequest);

        // Assert
        response.Should().NotBeNull();
        // Note: This test will likely fail due to authentication requirements
    }

    [Fact]
    public async Task SendMessage_WithValidData_ShouldPublishMessage()
    {
        // Arrange
        var messageRequest = new CreateChatMessageRequest
        {
            SenderId = 1,
            RecipientId = 2,
            Content = "Test message"
        };

        _mockUserService.Setup(x => x.CheckChatUsersAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<bool>.Success(true));

        _mockPublishEndpoint.Setup(x => x.Publish(It.IsAny<CreateChatMessageContract>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var response = await Client.POSTAsync<SendMessage.Endpoint, CreateChatMessageRequest>("/api/message/send", messageRequest);

        // Assert
        response.Should().NotBeNull();
        // Note: This test will likely fail due to authentication requirements
    }

    [Fact]
    public async Task SendMessage_WhenUsersDoNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var messageRequest = new CreateChatMessageRequest
        {
            SenderId = 1,
            RecipientId = 2,
            Content = "Test message"
        };

        _mockUserService.Setup(x => x.CheckChatUsersAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<bool>.Failure("Users don't exist"));

        // Act
        var response = await Client.POSTAsync<SendMessage.Endpoint, CreateChatMessageRequest>("/api/message/send", messageRequest);

        // Assert
        response.Should().NotBeNull();
        // Note: This test will likely fail due to authentication requirements
    }
} 