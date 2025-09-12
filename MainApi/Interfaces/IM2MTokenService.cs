namespace MainApi.Interfaces;

public interface IM2MTokenService
{
    Task<string> GetTokenAsync(CancellationToken ct = default);
}