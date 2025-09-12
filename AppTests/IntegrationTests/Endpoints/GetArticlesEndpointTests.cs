// using AppTests.TestFixtures;
// using FastEndpoints;
// using FastEndpoints.Testing;
// using FluentAssertions;
// using MainApi.Models.ArticlesDto;
// using MainApi.Models.Entities;
// using MainApi.Services.ArticleService;
// using Microsoft.Extensions.DependencyInjection;
// using Moq;
// using System.Net;
// using Xunit;
//
// namespace AppTests.IntegrationTests.Endpoints;
//
// // Zmieniamy sposób, w jaki klasa jest konstruowana, aby przekazać konfigurację DI
// public class GetArticlesEndpointTests : TestClass<TestFixture>
// {
//     private readonly Mock<IArticleService> _mockArticleService;
//
//     // KROK 1: Konstruktor przyjmuje 'fixture' i przekazuje ją do bazy.
//     // KROK 2: Drugi argument konstruktora 'base' to akcja, która zostanie wykonana
//     //         na kontenerze DI tylko dla tej klasy testowej.
//     public GetArticlesEndpointTests(TestFixture fixture) : base(fixture, services =>
//     {
//         // To jest teraz poprawne miejsce na rejestrację mocka.
//         // Tworzymy mocka i od razu go rejestrujemy.
//         var mock = new Mock<IArticleService>();
//         services.AddScoped(_ => mock.Object);
//
//         // Zapisujemy referencję do mocka w polu prywatnym klasy testowej,
//         // aby móc go konfigurować w poszczególnych testach.
//         // Używamy do tego sztuczki z pobraniem serwisu, który właśnie zarejestrowaliśmy.
//         var sp = services.BuildServiceProvider();
//         var testClass = sp.GetRequiredService<GetArticlesEndpointTests>();
//         testClass._mockArticleService = mock;
//     })
//     {
//         // Ten konstruktor jest teraz pusty, cała logika jest wyżej.
//     }
//
//     [Fact]
//     public async Task GetArticles_WhenArticlesExist_ShouldReturnOkWithArticles()
//     {
//         // Arrange
//         var articles = new List<Article>
//         {
//             new() { ArticleId = 1, Title = "Test Article 1", Content = "Content 1" },
//             new() { ArticleId = 2, Title = "Test Article 2", Content = "Content 2" }
//         };
//
//         var articleDtos = articles.Select(a => new ArticleDto
//         {
//             Id = a.ArticleId,
//             Title = a.Title,
//             Content = a.Content
//         }).ToList();
//
//         _mockArticleService
//             .Setup(x => x.GetArticlesAsync(It.IsAny<CancellationToken>()))
//             .ReturnsAsync(ServiceResult<IEnumerable<ArticleDto>>.Success(articleDtos));
//
//         // Act
//         var (response, result) = await Client.GETAsync<Endpoint, IEnumerable<ArticleDto>>();
//
//         // Assert
//         response.StatusCode.Should().Be(HttpStatusCode.OK);
//         result.Should().NotBeNull();
//         result.Should().HaveCount(2);
//     }
//
//     [Fact]
//     public async Task GetArticles_WhenServiceReturnsFailure_ShouldReturnNotFound()
//     {
//         // Arrange
//         _mockArticleService
//             .Setup(x => x.GetArticlesAsync(It.IsAny<CancellationToken>()))
//             .ReturnsAsync(ServiceResult<IEnumerable<ArticleDto>>.Failure("No articles found"));
//
//         // Act
//         var (response, _) = await Client.GETAsync<Endpoint, ErrorResponse>();
//
//         // Assert
//         response.StatusCode.Should().Be(HttpStatusCode.NotFound);
//     }
//
//     [Fact]
//     public async Task GetArticles_WhenServiceThrowsException_ShouldReturnInternalServerError()
//     {
//         // Arrange
//         _mockArticleService
//             .Setup(x => x.GetArticlesAsync(It.IsAny<CancellationToken>()))
//             .ThrowsAsync(new Exception("Database error"));
//
//         // Act
//         var (response, _) = await Client.GETAsync<Endpoint, ErrorResponse>();
//
//         // Assert
//         // Asercja poprawiona: nieobsłużony wyjątek w serwisie powinien skutkować błędem 500,
//         // a nie 404. Testujemy w ten sposób odporność endpointu.
//         response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
//     }
// }