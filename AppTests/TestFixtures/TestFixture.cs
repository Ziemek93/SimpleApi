using FastEndpoints.Testing;
using Flurl.Http.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration; // <-- Dodaj ten using

namespace AppTests.TestFixtures;

public class TestFixture : AppFixture<Program>
{
    // Zmieniono na PascalCase zgodnie z konwencją C#
    public HttpTest httpTest { get; private set; }

    protected override void ConfigureApp(IWebHostBuilder builder)
    {
        // --- KLUCZOWA ZMIANA ---
        // Mówimy hostowi, aby wczytał dodatkowy plik konfiguracyjny.
        // Ta konfiguracja nadpisze wartości z appsettings.json.
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            // Usuwamy istniejące źródła, aby mieć pewność, że nie ma konfliktów.
            // configBuilder.Sources.Clear(); // Opcjonalne, ale może pomóc w diagnozie

            // Dodajemy nasz plik testowy.
            // `optional: false` sprawi, że testy rzucą wyjątek, jeśli plik nie zostanie znaleziony.
            // To jest lepsze niż ciche niepowodzenie.
            configBuilder.AddJsonFile("appsettings.test.json", optional: false, reloadOnChange: false);

            // --- KROK DIAGNOSTYCZNY ---
            // Dodajmy do appsettings.test.json na chwilę klucz: "IsTestConfigLoaded": "YES"
            // i sprawdźmy, czy jest on tutaj widoczny.
            var tempConfig = configBuilder.Build();
            var debugValue = tempConfig.GetValue<string>("IsTestConfigLoaded");
            if (string.IsNullOrEmpty(debugValue))
            {
                // Jeśli testy kończą się tym wyjątkiem, to na 100% problemem jest
                // brak pliku w katalogu wyjściowym (patrz punkt 1).
                throw new InvalidOperationException(
                    "KRYTYCZNY BŁĄD: Plik appsettings.test.json nie został znaleziony lub jest pusty. " +
                    "Sprawdź właściwość 'Kopiuj do katalogu wyjściowego'!");
            }
        });
    }

    protected override Task SetupAsync()
    {
        // Przed każdym testem tworzymy nową instancję HttpTest.
        httpTest = new HttpTest();
        return Task.CompletedTask;
    }
    
    protected override Task TearDownAsync()
    {
        httpTest?.Dispose();
        return Task.CompletedTask;
    }
}