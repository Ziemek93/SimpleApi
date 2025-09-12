// using FastEndpoints.Testing;
// using FluentAssertions;
// using MainApi.Models.ArticlesDto;
// using MainApi.Models.Entities;
// using MainApi.Services.ArticleService;
// using Microsoft.Extensions.DependencyInjection;
// using Moq;
// using Xunit;
// using AppTests.TestFixtures;
// using MainApi.Services;
//
// namespace AppTests.IntegrationTests.Endpoints;
//
// public class GetArticleEndpointTests: TestBase<TestFixture>
// {
//     private readonly Mock<IArticleService> _mockArticleService;
//     private readonly TestFixture _fixture;
//     
//     public GetArticleEndpointTests(TestFixture fixture)
//     {
//         _mockArticleService = new Mock<IArticleService>();
//         TestFixture _fixture = fixture;
//     }
//
//     protected void ConfigureServices(IServiceCollection services)
//     {
//         services.AddScoped(_ => _mockArticleService.Object);
//     }
//
//     [Fact]
//     public async Task GetArticle_WhenArticleExists_ShouldReturnOkWithArticle()
//     {
//         // Arrange
//         var articleId = 1;
//         var article = new Article { ArticleId = articleId, Title = "Test Article", Content = "Test Content" };
//         var articleDto = new ArticleDto { Id = articleId, Title = "Test Article", Content = "Test Content" };
//
//         _mockArticleService.Setup(x => x.GetArticleAsync(articleId, It.IsAny<Func<Article, ArticleDto>>(), It.IsAny<CancellationToken>()))
//             .ReturnsAsync(ServiceResult<ArticleDto>.Success(articleDto));
//
//         // Act
//         var response = await Client.GETAsync<GetArticle.Endpoint>("/api/article/1");
//
//         // Assert
//         response.Should().NotBeNull();
//         response.StatusCode.Should().Be(200);
//         
//         var responseData = await response.GetResultAsync<ArticleDto>();
//         responseData.Should().NotBeNull();
//         responseData.Id.Should().Be(articleId);
//         responseData.Title.Should().Be("Test Article");
//     }
//
//     [Fact]
//     public async Task GetArticle_WhenArticleDoesNotExist_ShouldReturnNotFound()
//     {
//         // Arrange
//         var articleId = 999;
//         _mockArticleService.Setup(x => x.GetArticleAsync(articleId, It.IsAny<Func<Article, ArticleDto>>(), It.IsAny<CancellationToken>()))
//             .ReturnsAsync(ServiceResult<ArticleDto>.Failure("Article not found"));
//
//         // Act
//         var response = await Client.GETAsync<GetArticle.Endpoint>($"/api/article/{articleId}");
//
//         // Assert
//         response.Should().NotBeNull();
//         response.StatusCode.Should().Be(404);
//     }
//
//     [Theory]
//     [InlineData(1)]
//     [InlineData(10)]
//     [InlineData(100)]
//     public async Task GetArticle_WithDifferentIds_ShouldCallServiceWithCorrectId(int articleId)
//     {
//         // Arrange
//         var articleDto = new ArticleDto { Id = articleId, Title = "Test Article", Content = "Test Content" };
//         _mockArticleService.Setup(x => x.GetArticleAsync(It.IsAny<int>(), It.IsAny<Func<Article, ArticleDto>>(), It.IsAny<CancellationToken>()))
//             .ReturnsAsync(ServiceResult<ArticleDto>.Success(articleDto));
//
//         // Act
//         var response = await Client.GETAsync<GetArticle.Endpoint>($"/api/article/{articleId}");
//
//         // Assert
//         response.Should().NotBeNull();
//         response.StatusCode.Should().Be(200);
//         _mockArticleService.Verify(x => x.GetArticleAsync(articleId, It.IsAny<Func<Article, ArticleDto>>(), It.IsAny<CancellationToken>()), Times.Once);
//     }
// } 