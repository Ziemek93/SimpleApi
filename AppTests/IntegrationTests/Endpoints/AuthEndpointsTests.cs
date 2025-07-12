using FastEndpoints.Testing;
using FluentAssertions;
using Flurl.Http;
using MainApi.Endpoints.Public.Auth.Login;
using MainApi.Endpoints.Public.Auth.Register;
using MainApi.Models.Auth;
using Moq;
using System.Net;
using System.Text.Json;
using Xunit;
using AppTests.TestFixtures;
using Flurl;
using Microsoft.AspNetCore.Mvc;

namespace AppTests.IntegrationTests.Endpoints;

public class AuthEndpointsTests(TestFixture testFixture) : TestBase<TestFixture>
{
    [Fact]
    public async Task Register_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        // POPRAWKA: Odwołujemy się bezpośrednio do "Request", ponieważ jest w zakresie dzięki "using".
        var registerRequest = new Request
        {
            Username = "newuser",
            Email = "test@test.com",
            Password = "newpassword123"
        };

        // Konfigurujemy naszego mocka, aby symulował pomyślną odpowiedź z AuthApi
        var fakeAuthApiResponse = new UserDto { Id = Guid.NewGuid(), Username = registerRequest.Username };
        TestFixture.FlurlClientMock
            .Setup(x => x.Request(It.IsAny<Url>()))
            .Returns(new FlurlRequest());

        TestFixture.FlurlClientMock
            .Setup(x => x.Request().PostJsonAsync(It.IsAny<object>(), default, default))
            .ReturnsAsync(new FlurlResponse(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(fakeAuthApiResponse))
            }));

        // Act
        // POPRAWKA: Wskazujemy na klasę "Endpoint" oraz jej zagnieżdżone typy "Request" i "Response".
        var (response, result) = await App.Client.POSTAsync<Endpoint, Request, Response>(registerRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result.Message.Should().Be("Użytkownik zarejestrowany pomyślnie.");
    }

    [Fact]
    public async Task Register_WhenAuthApiFails_ShouldReturnBadRequest()
    {
        // Arrange
        var registerRequest = new Request // <-- POPRAWKA
        {
            Username = "faileduser",
            Email = "fail@test.com",
            Password = "newpassword123"
        };

        // Konfigurujemy mocka, aby symulował błąd (np. duplikat użytkownika) z AuthApi
        TestFixture.FlurlClientMock
            .Setup(x => x.Request(It.IsAny<Url>()))
            .Returns(new FlurlRequest());

        TestFixture.FlurlClientMock
            .Setup(x => x.Request().PostJsonAsync(It.IsAny<object>(), default, default))
            .ReturnsAsync(new FlurlResponse(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("User already exists")
            }));

        // Act
        var (response, _) = await Client.POSTAsync<Endpoint, Request, ErrorResponse>(registerRequest); // <-- POPRAWKA

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Theory]
    [InlineData("", "password", "test@test.com")]
    [InlineData("username", "", "test@test.com")]
    [InlineData("user", "pass", "not-an-email")]
    public async Task Register_WithInvalidData_ShouldReturnValidationError(string username, string password, string email)
    {
        // Arrange
        var registerRequest = new Request // <-- POPRAWKA
        {
            Username = username,
            Password = password,
            Email = email
        };

        // Act
        var (response, result) = await Client.POSTAsync<Endpoint, Request, ValidationProblemDetails>(registerRequest); // <-- POPRAWKA

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Errors.Should().NotBeEmpty();

        if (string.IsNullOrEmpty(username))
            result.Errors.Should().ContainKey("Username");

        if (string.IsNullOrEmpty(password))
            result.Errors.Should().ContainKey("Password");

        if (email == "not-an-email")
            result.Errors.Should().ContainKey("Email");
    }
}