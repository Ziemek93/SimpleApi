using Flurl.Http;
using Flurl.Http.Configuration;
using MainApi;
using MainApi.Models.Auth;
using MainApi.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;
using System.Text.Json;
using FastEndpoints.Testing;

namespace AppTests.TestFixtures;

public class TestFixture : AppFixture<Program>
{
    // Użyjemy statycznego mocka, aby można było go konfigurować w poszczególnych testach
    public static readonly Mock<IFlurlClient> FlurlClientMock = new();

    protected override void ConfigureApp(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // 1. Tworzymy mocka dla IFlurlClientCache, który zarządza klientami Flurl
            var flurlClientCacheMock = new Mock<IFlurlClientCache>();

            // 2. Konfigurujemy go tak, aby dla adresu naszego testowego AuthApi
            //    zawsze zwracał naszego statycznego, kontrolowanego mocka IFlurlClient.
            flurlClientCacheMock
                .Setup(x => x.GetOrAdd(It.IsAny<string>(), It.IsAny<string>(), null))
                .Returns(FlurlClientMock.Object);

            // 3. Zastępujemy prawdziwą implementację IFlurlClientCache w kontenerze DI
            //    naszym mockiem. Od teraz każdy endpoint w MainApi, który spróbuje
            //    użyć Flurl, dostanie naszą zaślepkę.
            services.AddSingleton(flurlClientCacheMock.Object);
        });
    }

    protected override Task SetupAsync()
    {
        // Resetujemy mocka przed każdym testem, aby testy były od siebie niezależne
        FlurlClientMock.Reset();
        return Task.CompletedTask;
    }
}