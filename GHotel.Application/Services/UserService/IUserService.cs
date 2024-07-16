using GHotel.Application.Models.User;

namespace GHotel.Application.Services.UserService;

public interface IUserService
{
    Task<UserResponseModel> Register(UserRequestModel userRequestModel, CancellationToken cancellationToken);
    Task<UserResponseModel> GetByEmail(string email, CancellationToken cancellationToken);
    Task<UserResponseModel> Get(string id, CancellationToken cancellationToken);
    Task<UserResponseModel> ConfirmEmail(string userId, CancellationToken cancellationToken);
    Task<UserResponseModel> Login(LoginModel loginModel, CancellationToken cancellationToken);
}
