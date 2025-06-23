using MainApi.Models;
using MainApi.Models.Enums;
using MainApi.Repositories.User;

namespace MainApi.Services.UserService;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ResponseTuple<bool, ResponseEnum>> CheckChatUsersAsync(int firstId, int secondId, CancellationToken ct = default)
    {
        var response = new ResponseTuple<bool, ResponseEnum>();

        var result = await _userRepository.UserPairExist(firstId, secondId, ct);

        if (!result)
        {
            return response.Make(false, ResponseEnum.NotFound, "Users don't exist");
        }
        
        return response.Make(true, ResponseEnum.NotFound, "Users don't exist");
        
    }
}