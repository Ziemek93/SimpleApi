using FluentAssertions;
using MainApi.Context;
using MainApi.Models.ArticlesDao;
using MainApi.Models.Entities;
using MainApi.Repositories.Articles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Xunit;

namespace AppTests.IntegrationTests.Database;

public class ArticleRepositoryIntegrationTests : IAsyncDisposable
{
    private readonly PostgreSqlContainer _postgreSqlContainer;
    private readonly ApplicationContext _context;
    private readonly ArticleRepository _repository;

    public ArticleRepositoryIntegrationTests()
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
        
        _repository = new ArticleRepository(_context);
    }

    [Fact]
    public async Task GetArticlesAsync_WhenArticlesExist_ShouldReturnArticles()
    {
        // Arrange
        var category = new Category { CategoryId = 1, CategoryName = "Test Category" };
        await _context.Categories.AddAsync(category);
        
        var articles = new List<Article>
        {
            new() { Name = "Article 1", Description = "Content 1", CategoryId = 1 },
            new() { Name = "Article 2", Description = "Content 2", CategoryId = 1 }
        };
        await _context.Articles.AddRangeAsync(articles);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetArticlesAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(a => a.Name == "Article 1");
        result.Should().Contain(a => a.Name == "Article 2");
    }

    [Fact]
    public async Task GetArticlesAsync_WhenNoArticlesExist_ShouldReturnEmptyList()
    {
        // Act
        var result = await _repository.GetArticlesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetArticleAsync_WhenArticleExists_ShouldReturnArticle()
    {
        // Arrange
        var category = new Category { CategoryId = 1, CategoryName = "Test Category" };
        await _context.Categories.AddAsync(category);
        
        var article = new Article { Name = "Test Article", Description = "Test Content", CategoryId = 1 };
        await _context.Articles.AddAsync(article);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetArticleAsync(article.ArticleId);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Test Article");
        result.Description.Should().Be("Test Content");
    }

    [Fact]
    public async Task GetArticleAsync_WhenArticleDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetArticleAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddArticleAsync_WhenValidArticle_ShouldAddToDatabase()
    {
        // Arrange
        var category = new Category { CategoryId = 1, CategoryName = "Test Category" };
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();

        var article = new Article { Name = "New Article", Description = "New Content", CategoryId = 1 };

        // Act
        var result = await _repository.AddArticleAsync(article);

        // Assert
        result.Should().BeGreaterThan(0);
        
        var savedArticle = await _context.Articles.FirstOrDefaultAsync(a => a.Name == "New Article");
        savedArticle.Should().NotBeNull();
        savedArticle!.Name.Should().Be("New Article");
    }

    [Fact]
    public async Task CategoryExistAsync_WhenCategoryExists_ShouldReturnTrue()
    {
        // Arrange
        var category = new Category { CategoryId = 1, CategoryName = "Test Category" };
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.CategoryExistAsync(1);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CategoryExistAsync_WhenCategoryDoesNotExist_ShouldReturnFalse()
    {
        // Act
        var result = await _repository.CategoryExistAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task EditArticleAsync_WhenArticleExists_ShouldUpdateArticle()
    {
        // Arrange
        var category = new Category { CategoryId = 1, CategoryName  = "Test Category" };
        await _context.Categories.AddAsync(category);
        
        var article = new Article { Name = "Original Title", Description = "Original Content", CategoryId = 1 };
        await _context.Articles.AddAsync(article);
        await _context.SaveChangesAsync();

        var updateDao = new ArticleDao { ArticleName = "Updated Title", ArticleDescription = "Updated Content" };

        // Act
        var result = await _repository.EditArticleAsync(article.ArticleId, updateDao);

        // Assert
        result.Should().BeTrue();
        
        var updatedArticle = await _context.Articles.FindAsync(article.ArticleId);
        updatedArticle.Should().NotBeNull();
        updatedArticle!.Name.Should().Be("Updated Title");
        updatedArticle.Description.Should().Be("Updated Content");
    }

    [Fact]
    public async Task EditArticleAsync_WhenArticleDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var updateDao = new ArticleDao { ArticleName = "Updated Title", ArticleDescription = "Updated Content" };

        // Act
        var result = await _repository.EditArticleAsync(999, updateDao);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteArticleAsync_WhenArticleExists_ShouldDeleteArticle()
    {
        // Arrange
        var category = new Category { CategoryId = 1, CategoryName = "Test Category" };
        await _context.Categories.AddAsync(category);
        
        var article = new Article { Name= "To Delete", Description = "Content", CategoryId = 1 };
        await _context.Articles.AddAsync(article);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteArticleAsync(article.ArticleId);

        // Assert
        result.Should().BeTrue();
        
        var deletedArticle = await _context.Articles.FindAsync(article.ArticleId);
        deletedArticle.Should().BeNull();
    }

    [Fact]
    public async Task DeleteArticleAsync_WhenArticleDoesNotExist_ShouldReturnFalse()
    {
        // Act
        var result = await _repository.DeleteArticleAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
        await _postgreSqlContainer.DisposeAsync();
    }
} 