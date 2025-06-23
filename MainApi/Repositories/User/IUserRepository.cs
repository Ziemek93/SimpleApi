namespace MainApi.Repositories.User;

public interface IUserRepository
{
    Task<bool> UserPairExist(int firstId, int secondId, CancellationToken ct = default);
}