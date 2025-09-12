# AppTests - Testy jednostkowe i integracyjne dla MainApi

Ten projekt zawiera kompleksowe testy jednostkowe i integracyjne dla projektu MainApi.

## Struktura testów

### Testy jednostkowe (`UnitTests/`)
- **ArticleServiceTests.cs** - Testy dla serwisu artykułów
- **UserServiceTests.cs** - Testy dla serwisu użytkowników
- **ServiceResultTests.cs** - Testy dla klasy ServiceResult

### Testy integracyjne (`IntegrationTests/`)

#### Endpoints (`IntegrationTests/Endpoints/`)
- **GetArticlesEndpointTests.cs** - Testy dla endpointu pobierania artykułów
- **GetArticleEndpointTests.cs** - Testy dla endpointu pobierania pojedynczego artykułu
- **AuthEndpointsTests.cs** - Testy dla endpointów autoryzacji
- **PrivateEndpointsTests.cs** - Testy dla prywatnych endpointów (wymagających autoryzacji)

#### Database (`IntegrationTests/Database/`)
- **ArticleRepositoryIntegrationTests.cs** - Testy integracyjne dla repozytorium artykułów
- **UserRepositoryIntegrationTests.cs** - Testy integracyjne dla repozytorium użytkowników

### Pomocnicze klasy (`TestHelpers/`)
- **AuthenticationHelper.cs** - Pomocnicze metody do generowania tokenów JWT dla testów

### Fixtures (`TestFixtures/`)
- **TestFixture.cs** - Klasa bazowa dla testów integracyjnych z konfiguracją bazy danych

## Uruchamianie testów

### Wymagania
- .NET 8.0
- Docker (dla testów integracyjnych z bazą danych)

### Uruchamianie wszystkich testów
```bash
dotnet test
```

### Uruchamianie tylko testów jednostkowych
```bash
dotnet test --filter "Category=Unit"
```

### Uruchamianie tylko testów integracyjnych
```bash
dotnet test --filter "Category=Integration"
```

### Uruchamianie testów z pokryciem kodu
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Konfiguracja

### Plik konfiguracyjny (`appsettings.test.json`)
Zawiera konfigurację dla środowiska testowego:
- Connection strings dla testowej bazy danych
- Konfiguracja RabbitMQ
- Ustawienia JWT
- Konfiguracja AuthApi

### Testcontainers
Testy integracyjne używają Testcontainers do uruchamiania izolowanych instancji PostgreSQL.

## Technologie używane w testach

- **xUnit** - Framework testowy
- **FluentAssertions** - Asercje
- **Moq** - Mocking
- **AutoFixture** - Generowanie danych testowych
- **FastEndpoints.Testing** - Testy dla FastEndpoints
- **Testcontainers** - Kontenery dla testów integracyjnych
- **Microsoft.AspNetCore.Mvc.Testing** - Testy dla ASP.NET Core

## Struktura testów

### Testy jednostkowe
Testy jednostkowe sprawdzają logikę biznesową w izolacji, używając mocków dla zależności zewnętrznych.

### Testy integracyjne
Testy integracyjne sprawdzają:
- Interakcję między komponentami
- Integrację z bazą danych
- Działanie endpointów API
- Autoryzację i uwierzytelnianie

### Testy bazodanowe
Testy bazodanowe używają Testcontainers do uruchamiania izolowanych instancji PostgreSQL, zapewniając:
- Izolację testów
- Spójność danych
- Automatyczne czyszczenie po testach

## Najlepsze praktyki

1. **Nazewnictwo testów**: Używaj wzorca `[Method]_[Scenario]_[ExpectedResult]`
2. **Struktura testów**: Arrange-Act-Assert (AAA)
3. **Mockowanie**: Mockuj tylko zewnętrzne zależności
4. **Izolacja**: Każdy test powinien być niezależny
5. **Czytelność**: Używaj opisowych nazw i komentarzy

## Rozwiązywanie problemów

### Testy nie uruchamiają się
- Sprawdź czy Docker jest uruchomiony
- Sprawdź czy wszystkie pakiety NuGet są zainstalowane
- Sprawdź czy projekt MainApi jest poprawnie skonfigurowany

### Testy integracyjne nie przechodzą
- Sprawdź czy Testcontainers może uruchomić kontenery
- Sprawdź czy porty nie są zajęte
- Sprawdź logi kontenerów

### Problemy z autoryzacją w testach
- Użyj `AuthenticationHelper` do generowania tokenów testowych
- Sprawdź czy konfiguracja JWT jest poprawna
- Mockuj zewnętrzne serwisy autoryzacji 