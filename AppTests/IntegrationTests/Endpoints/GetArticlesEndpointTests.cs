using FastEndpoints.Testing;
using FluentAssertions;
using MainApi.Models.ArticlesDto;
using MainApi.Models.Entities;
using MainApi.Services.ArticleService;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using AppTests.TestFixtures;

namespace AppTests.IntegrationTests.Endpoints;

public class GetArticlesEndpointTests : TestBase<TestFixture>
{
    private readonly Mock<IArticleService> _mockArticleService;

    public GetArticlesEndpointTests()
    {
        _mockArticleService = new Mock<IArticleService>();
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped(_ => _mockArticleService.Object);
    }

    [Fact]
    public async Task GetArticles_WhenArticlesExist_ShouldReturnOkWithArticles()
    {
        // Arrange
        var articles = new List<Article>
        {
            new() { ArticleId = 1, Title = "Test Article 1", Content = "Content 1" },
            new() { ArticleId = 2, Title = "Test Article 2", Content = "Content 2" }
        };

        var articleDtos = articles.Select(a => new ArticleDto
        {
            Id = a.ArticleId,
            Title = a.Title,
            Content = a.Content
        });

        _mockArticleService.Setup(x => x.GetArticlesAsync(It.IsAny<Func<Article, ArticleDto>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<IEnumerable<ArticleDto>>.Success(articleDtos));

        // Act
        var response = await Client.GETAsync<GetArticles.Endpoint>();

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(200);
        
        var responseData = await response.GetResultAsync<IEnumerable<ArticleDto>>();
        responseData.Should().NotBeNull();
        responseData.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetArticles_WhenNoArticlesExist_ShouldReturnNotFound()
    {
        // Arrange
        _mockArticleService.Setup(x => x.GetArticlesAsync(It.IsAny<Func<Article, ArticleDto>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<IEnumerable<ArticleDto>>.Failure("No articles found"));

        // Act
        var response = await Client.GETAsync<GetArticles.Endpoint>();

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetArticles_WhenServiceThrowsException_ShouldReturnNotFound()
    {
        // Arrange
        _mockArticleService.Setup(x => x.GetArticlesAsync(It.IsAny<Func<Article, ArticleDto>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var response = await Client.GETAsync<GetArticles.Endpoint>();

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(404);
    }
} 