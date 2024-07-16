using GHotel.Application.Models.User;

namespace GHotel.API.Infrastructure.Authentication;

public interface ISignInManager
{
    Task LogInMain(UserResponseModel user, CancellationToken cancellationToken);
    Task<UserResponseModel> Register(string email, bool emailConfirmed, string? password, CancellationToken cancellationToken);
    Task SignOut(string scheme);
    Task<bool> SendRegistrationEmailConfirmationAsync(string userId, string userEmail, CancellationToken cancellationToken);
    string? GetUserIdFromToken(string token);
}
