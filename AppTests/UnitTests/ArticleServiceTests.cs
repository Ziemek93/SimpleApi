using AutoFixture;
using FluentAssertions;
using MainApi.Models.ArticlesDao;
using MainApi.Models.ArticlesDto;
using MainApi.Models.Entities;
using MainApi.Repositories.Articles;
using MainApi.Services.ArticleService;
using Moq;
using Xunit;

namespace AppTests.UnitTests;

public class ArticleServiceTests
{
    private readonly Mock<IArticleRepository> _mockArticleRepository;
    private readonly ArticleService _articleService;
    private readonly Fixture _fixture;

    public ArticleServiceTests()
    {
        _mockArticleRepository = new Mock<IArticleRepository>();
        _articleService = new ArticleService(_mockArticleRepository.Object);
        _fixture = new Fixture();
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task GetArticlesAsync_WhenRepositoryReturnsArticles_ShouldReturnSuccessResult()
    {
        // Arrange
        var articles = _fixture.CreateMany<Article>(3).ToList();
        _mockArticleRepository.Setup(x => x.GetArticlesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(articles);

        Func<Article, ArticleDto> mapper = article => new ArticleDto
        {
            Name = article.Name,
            Description = article.Description,
            Visibility = false,
            Tags = article.Tags.Select(x=>x.Title).ToList()
        };

        // Act
        var result = await _articleService.GetArticlesAsync(mapper);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(3);
        _mockArticleRepository.Verify(x => x.GetArticlesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetArticleAsync_WhenArticleExists_ShouldReturnSuccessResult()
    {
        // Arrange
        var article = _fixture.Create<Article>();
        _mockArticleRepository.Setup(x => x.GetArticleAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(article);

        Func<Article, ArticleDto> mapper = article => new ArticleDto
        {
            Name = article.Name,
            Description = article.Description,
            Visibility = false,
            Tags = article.Tags.Select(x=>x.Title).ToList()
        };

        // Act
        var result = await _articleService.GetArticleAsync(1, mapper);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Name.Should().Be(article.Name);
        _mockArticleRepository.Verify(x => x.GetArticleAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetArticleAsync_WhenArticleDoesNotExist_ShouldReturnFailureResult()
    {
        // Arrange
        _mockArticleRepository.Setup(x => x.GetArticleAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Article?)null);

        Func<Article, ArticleDto> mapper = article => new ArticleDto
        {
            Name = article.Name,
            Description = article.Description,
            Visibility = false,
            Tags = article.Tags.Select(x=>x.Title).ToList()
        };

        // Act
        var result = await _articleService.GetArticleAsync(999, mapper);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Article not found");
        _mockArticleRepository.Verify(x => x.GetArticleAsync(999, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddArticleAsync_WhenCategoryExists_ShouldReturnSuccessResult()
    {
        // Arrange
        var article = _fixture.Create<Article>();
        _mockArticleRepository.Setup(x => x.CategoryExistAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockArticleRepository.Setup(x => x.AddArticleAsync(It.IsAny<Article>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _articleService.AddArticleAsync(article);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(article);
        _mockArticleRepository.Verify(x => x.CategoryExistAsync(article.CategoryId, It.IsAny<CancellationToken>()), Times.Once);
        _mockArticleRepository.Verify(x => x.AddArticleAsync(article, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddArticleAsync_WhenCategoryDoesNotExist_ShouldReturnFailureResult()
    {
        // Arrange
        var article = _fixture.Create<Article>();
        _mockArticleRepository.Setup(x => x.CategoryExistAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _articleService.AddArticleAsync(article);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Category don't exist");
        _mockArticleRepository.Verify(x => x.CategoryExistAsync(article.CategoryId, It.IsAny<CancellationToken>()), Times.Once);
        _mockArticleRepository.Verify(x => x.AddArticleAsync(It.IsAny<Article>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AddArticleAsync_WhenRepositoryReturnsZero_ShouldReturnFailureResult()
    {
        // Arrange
        var article = _fixture.Create<Article>();
        _mockArticleRepository.Setup(x => x.CategoryExistAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockArticleRepository.Setup(x => x.AddArticleAsync(It.IsAny<Article>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        var result = await _articleService.AddArticleAsync(article);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Article can't be added.");
        _mockArticleRepository.Verify(x => x.AddArticleAsync(article, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EditArticleAsync_WhenRepositoryReturnsTrue_ShouldReturnSuccessResult()
    {
        // Arrange
        var articleDao = _fixture.Create<ArticleDao>();
        _mockArticleRepository.Setup(x => x.EditArticleAsync(It.IsAny<int>(), It.IsAny<ArticleDao>(), CancellationToken.None)
            )
            .ReturnsAsync(true);

        // Act
        var result = await _articleService.EditArticleAsync(1, articleDao);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();
        _mockArticleRepository.Verify(x => x.EditArticleAsync(1, articleDao, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task EditArticleAsync_WhenRepositoryReturnsFalse_ShouldReturnFailureResult()
    {
        // Arrange
        var articleDao = _fixture.Create<ArticleDao>();
        _mockArticleRepository.Setup(x => x.EditArticleAsync(It.IsAny<int>(), It.IsAny<ArticleDao>(), CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var result = await _articleService.EditArticleAsync(1, articleDao);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Article update failed.");
        _mockArticleRepository.Verify(x => x.EditArticleAsync(1, articleDao, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task DeleteArticleAsync_WhenRepositoryReturnsTrue_ShouldReturnSuccessResult()
    {
        // Arrange
        _mockArticleRepository.Setup(x => x.DeleteArticleAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _articleService.DeleteArticleAsync(1);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();
        _mockArticleRepository.Verify(x => x.DeleteArticleAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteArticleAsync_WhenRepositoryReturnsFalse_ShouldReturnFailureResult()
    {
        // Arrange
        _mockArticleRepository.Setup(x => x.DeleteArticleAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _articleService.DeleteArticleAsync(1);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Article deletion error");
        _mockArticleRepository.Verify(x => x.DeleteArticleAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CheckArticleAsync_WhenArticleExists_ShouldReturnSuccessResult()
    {
        // Arrange
        var article = _fixture.Create<Article>();
        _mockArticleRepository.Setup(x => x.GetArticleAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(article);

        // Act
        var result = await _articleService.CheckArticleAsync(1);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeTrue();
        _mockArticleRepository.Verify(x => x.GetArticleAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CheckArticleAsync_WhenArticleDoesNotExist_ShouldReturnFailureResult()
    {
        // Arrange
        _mockArticleRepository.Setup(x => x.GetArticleAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Article?)null);

        // Act
        var result = await _articleService.CheckArticleAsync(999);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Article not found");
        _mockArticleRepository.Verify(x => x.GetArticleAsync(999, It.IsAny<CancellationToken>()), Times.Once);
    }
} 