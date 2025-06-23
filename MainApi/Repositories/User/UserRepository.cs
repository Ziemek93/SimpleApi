using MainApi.Context;
using Microsoft.EntityFrameworkCore;

namespace MainApi.Repositories.User;

public class UserRepository : IUserRepository
{
    private readonly ApplicationContext _context;

    public UserRepository(ApplicationContext context)
    {
        _context = context;
    }
    
    public async Task<bool> UserPairExist(int firstId, int secondId, CancellationToken ct = default)
    {
        await using var context = _context.CreateDbContext();

        var usersCount = await context.Users
            .Where(x => x.UserId == firstId || x.UserId == secondId)
            .AsNoTracking()
            .CountAsync(ct);
        if (usersCount != 2)
        {
            return false;
        }

        return true;
    }
}