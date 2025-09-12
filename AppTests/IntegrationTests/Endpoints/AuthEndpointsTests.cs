using System.Net;
using System.Text.Json;
using AppTests.TestFixtures;
using AutoFixture;
using FastEndpoints;
using FastEndpoints.Testing;
using FluentAssertions;
using Flurl.Http;
using Flurl.Http.Testing;
using MainApi.Endpoints.Public.Auth.Register;
using MainApi.Models.Auth;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Request = MainApi.Endpoints.Public.Auth.Register.Request;
using Response = MainApi.Endpoints.Public.Auth.Register.Response;

namespace AppTests.IntegrationTests.Endpoints;

// Używamy IClassFixture, aby mieć dostęp do tej samej instancji TestFixture we wszystkich testach
public class AuthEndpointsTests(TestFixture fixture) : TestBase<TestFixture>
{
    [Fact]
    public async Task Register_WithValidData_ShouldReturnSuccessAndCallApi()
    {
        // Arrange
        var registerRequest = new Request
        {
            Username = "newuser",
            Password = "newpassword123"
        };

        var fakeAuthApiResponse = new UserDto(1, registerRequest.Username);

        // Używamy wbudowanej metody z Flurl.Http.Testing do "zaślepenia" odpowiedzi.
        // To jest znacznie czytelniejsze niż Moq.
        fixture.httpTest.RespondWith(JsonSerializer.Serialize(fakeAuthApiResponse), 200);

        // Act
        var (httpResponse, result) = await fixture.Client.POSTAsync<Endpoint, Request, Response>(registerRequest);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Message.Should().Be($"The user [{registerRequest.Username}] has been created.");

        // Dodatkowa, potężna asercja: weryfikujemy, czy wywołanie HTTP zostało wykonane.
        fixture.httpTest.ShouldHaveCalled("*/api/register")
            .WithVerb(HttpMethod.Post)
            .Times(1);
    }

    [Fact]
    public async Task Register_WhenAuthApiFails_ShouldReturnBadRequest()
    {
        // Arrange
        var registerRequest = new Request
        {
            Username = "faileduser",
            Password = "newpassword123"
        };

        // Symulujemy odpowiedź błędu z zewnętrznego API
        fixture.httpTest.RespondWith("User already exists", 400);

        // Act
        var (httpResponse, _) = await fixture.Client.POSTAsync<Endpoint, Request, ErrorResponse>(registerRequest);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        // Weryfikujemy, że mimo błędu, wywołanie zostało podjęte
        fixture.httpTest.ShouldHaveCalled("*/api/register").Times(1);
    }

    [Theory]
    [InlineData("", "password", "test@test.com")]
    [InlineData("username", "", "test@test.com")]
    [InlineData("user", "pass", "not-an-email")]
    public async Task Register_WithInvalidData_ShouldReturnValidationErrorAndNotCallApi(string username, string password, string email)
    {
        // Arrange
        var registerRequest = new Request
        {
            Username = username,
            Password = password
        };

        // Act
        var (httpResponse, result) = await fixture.Client.POSTAsync<Endpoint, Request, ValidationProblemDetails>(registerRequest);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Errors.Should().NotBeEmpty();

        // Kluczowa asercja: sprawdzamy, czy zewnętrzne API NIE zostało wywołane,
        // ponieważ walidacja FastEndpoints zatrzymała żądanie.
        fixture.httpTest.ShouldNotHaveMadeACall();
    }
}