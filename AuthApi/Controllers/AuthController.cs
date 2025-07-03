using System.Security.Cryptography;
using AuthApi.Auth;
using AuthApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AuthApi.Models.Entities;
using AuthApi.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly Token _token;
    private readonly JwtOptions _jwtSettings;
    private readonly IEnumerable<M2MOptions> _m2mSettings;

    public AuthController(UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager, Token token, IOptions<JwtOptions> jwtSettings, IOptions<List<M2MOptions>> m2MSettings)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _token = token;
        _jwtSettings = jwtSettings.Value;
        _m2mSettings = m2MSettings.Value;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Register(UserRequest request)
    {
        var userExists = await _userManager.FindByNameAsync(request.Username);
        if (userExists != null)
            return BadRequest("User does not exist.");

        var user = new ApplicationUser
        {
            UserName = request.Username,
            Email = request.Username,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        if (!await _roleManager.RoleExistsAsync("User"))
            await _roleManager.CreateAsync(new IdentityRole("User"));

        await _userManager.AddToRoleAsync(user, "User");

        var userInfo = await _userManager.FindByNameAsync(request.Username);
        var response = new UserResponse(userInfo!.ClientId, request.Username);

        return Ok(response);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Login(UserRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        var userRoles = await _userManager.GetRolesAsync(user);

        if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
        {
            var token = _token.GenerateJwtToken(user, userRoles);
            return Ok(new { token });
        }

        return Unauthorized("Wrng login or passowrd.");
    }

    [HttpGet("jwks.json")]
    public async Task<IActionResult> GetPublicKey()
    {
        using var rsa = RSA.Create();
        rsa.FromXmlString(_jwtSettings.PrivateKey);
        var rsaParameters = rsa.ExportParameters(false); 

        var jwk = new JsonWebKey
        {
            Kty = "RSA",
            Use = "sig", 
            Kid = "some-rsa-key-123", 
            Alg = SecurityAlgorithms.RsaSha256,
            E = Base64UrlEncoder.Encode(rsaParameters.Exponent),
            N = Base64UrlEncoder.Encode(rsaParameters.Modulus)
        };

        var jwks = new JsonWebKeySet();
        jwks.Keys.Add(jwk);

        return Ok(jwks);
    }
    
    [HttpPost("[action]")]
    public async Task<IActionResult> TokenM2M([FromBody]M2MTokenRequest request)
    {
        var client = _m2mSettings.FirstOrDefault(c => c.ClientId == request.ClientId && c.ClientSecret == request.ClientSecret);

        if (client == null)
        {
            return Unauthorized();
        }

        var m2m = _token.GenerateM2MToken(client);
        
        return Ok(new { m2m });
    }
}