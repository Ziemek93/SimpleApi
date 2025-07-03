using Bogus;
using MainApi.Context;
using MainApi.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Flurl.Http;
using Flurl.Http.Configuration;
using MainApi.Endpoints.Public.Auth.Register; 
using MainApi.Models.Auth;
using MainApi.Options;
using Microsoft.Extensions.Options;

namespace MainApi.Services;

public class DatabaseSeeder
{
    private readonly ApplicationContext _context;
    private readonly IFlurlClient _flurlClient;
    private readonly AuthApiOptions _authApiOptions;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        ApplicationContext context,
        IFlurlClientCache flurlClientCache,
        IOptions<AuthApiOptions> authApiOptions,
        ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _authApiOptions = authApiOptions.Value;
        _flurlClient = flurlClientCache.GetOrAdd(_authApiOptions.Base, _authApiOptions.Base);
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        // if ((await _context.Database.GetPendingMigrationsAsync()).Any())
        // {
        //     await _context.Database.MigrateAsync();
        // }

        
        var fstUser = await SeedUserAsync("test1", "PasswordPassword123!");
        var scdUser = await SeedUserAsync("test2", "PasswordPassword123!");
        
        await SeedArticlesAndCategoriesAsync(fstUser);
    }

    private async Task<User?> SeedUserAsync(string username, string password, CancellationToken ct = default)
    {
        if (await _context.Users.AnyAsync(u => u.UserName == username, ct))
        {
            _logger.LogInformation("User '{Username}' exists. Skipping creation.", username);
            return await _context.Users.FirstAsync(u => u.UserName == username, ct);
        }

        _logger.LogInformation("Started creating user '{Username}' via AuthApi...", username);

        try
        {
            var registerRequest = new Request
            {
                Username = username,
                Password = password
            };

            var path = $"{_authApiOptions.Base}{_authApiOptions.Paths.Register}";

            var flurlResponse = await _flurlClient
                .Request(path)
                .WithHeader("Accept", "application/json")
                .WithHeader("Content-Type", "application/json")
                .WithTimeout(30)
                .PostJsonAsync(registerRequest, cancellationToken: ct);
            
            if (!flurlResponse.ResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogError("Authapi error: '{Username}'. Status: {StatusCode}", username, flurlResponse.StatusCode);
                return null;
            }

            var userDto = await flurlResponse.GetJsonAsync<UserDto>();
            
            var newUser = new User
            {
                UserId = userDto.Id,
                UserName = userDto.Username
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User '{Username}' with id {UserId}) created.", newUser.UserName, newUser.UserId);
            return newUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durning user seedign '{Username}'.", username);
            return null;
        }
    }

    private async Task SeedArticlesAndCategoriesAsync(User? user)
    {
        if (user is null)
        {
            _logger.LogWarning("Cannot add new article, user missing.");
            return;
        }

        if (await _context.Categories.AnyAsync())
        {
            _logger.LogInformation("Categories exist. Skipping creation.");
            return;
        }

        var categories = new List<Category>
        {
            new Category { CategoryName = "Tech", CategoryDescription = "Technology articles", UserId = user.UserId },
            new Category { CategoryName = "Life", CategoryDescription = "Lifestyle articles", UserId = user.UserId }
        };
        await _context.Categories.AddRangeAsync(categories);
        await _context.SaveChangesAsync(); 

        var tags = new List<Tag>
        {
            new Tag { Title = "C#" },
            new Tag { Title = "Blazor" },
            new Tag { Title = "AI" }
        };
        await _context.Tags.AddRangeAsync(tags);
        await _context.SaveChangesAsync(); 
        
        var articlesFaker = new Faker<Article>()
            .RuleFor(x => x.Name, e => e.Lorem.Sentence(3))
            .RuleFor(x => x.Description, e => e.Lorem.Sentence(20, 30))
            .RuleFor(x => x.CategoryId, e => categories[(e.IndexFaker) % categories.Count].CategoryId)
            .RuleFor(x => x.UserId, _ => 1)
            .RuleFor(x => x.Visibility, e => e.Random.Bool());

        var articles = articlesFaker.Generate(20);
        
        await _context.Articles.AddRangeAsync(articles);
        await _context.SaveChangesAsync(); 

        _logger.LogInformation("Demo data added successfully.");
    }
}