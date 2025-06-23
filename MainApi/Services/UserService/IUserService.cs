using MainApi.Models;
using MainApi.Models.Enums;

namespace MainApi.Services.UserService;

public interface IUserService
{
     Task<ResponseTuple<bool, ResponseEnum>> CheckChatUsersAsync(int firstId, int secondId, CancellationToken ct = default);
}