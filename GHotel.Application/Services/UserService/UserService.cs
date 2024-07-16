using GHotel.Application.Exceptions;
using GHotel.Application.Models.User;
using GHotel.Application.UOW;
using GHotel.Application.Utilities;
using GHotel.Domain.Entities;
using Mapster;

namespace GHotel.Application.Services.UserService;

public class UserService : IUserService
{
    private readonly IUserUnitOfWork _userUnitOfWork;
    private readonly IPasswordHasherUtility _passwordHasherUtility;

    public UserService(IUserUnitOfWork userUnitOfWork, IPasswordHasherUtility passwordHasherUtility)
    {
        _userUnitOfWork = userUnitOfWork;
        _passwordHasherUtility = passwordHasherUtility;
    }

    public async Task<UserResponseModel> Get(string id, CancellationToken cancellationToken)
    {
        var targetUser = await _userUnitOfWork.UserRepository.Get(id, cancellationToken).ConfigureAwait(false);
        if (targetUser == null)
            throw new UserNotFoundException($"User with id: {id} not found");
        return targetUser.Adapt<UserResponseModel>();
    }

    public async Task<UserResponseModel> GetByEmail(string email, CancellationToken cancellationToken)
    {
        var targetUser = await _userUnitOfWork.UserRepository.GetByEmail(email, cancellationToken).ConfigureAwait(false);
        if (targetUser == null)
            throw new UserWithEmailNotFoundException($"User with email: {email} not found");
        return targetUser.Adapt<UserResponseModel>();
    }

    public async Task<UserResponseModel> Register(UserRequestModel userRequestModel, CancellationToken cancellationToken)
    {
        var emailIsTaken = await _userUnitOfWork.UserRepository.Any(us => us.Email == userRequestModel.Email && us.EmailConfirmed, cancellationToken).ConfigureAwait(false);
        if (emailIsTaken)
            throw new EmailIsTakenException($"Specified email: {userRequestModel.EmailConfirmed} is already taken");

        var newUser = userRequestModel.Adapt<MyUser>();

        if(userRequestModel.Password is not null)
            newUser.PasswordHashe = _passwordHasherUtility.GenerateHash(userRequestModel.Password);

        var userRole = await _userUnitOfWork.RoleRepository.Get(ro => ro.Name == "User", cancellationToken).ConfigureAwait(false);
        newUser.Roles = new List<MyRole>() { userRole! };

        var result = await _userUnitOfWork.UserRepository.Add(newUser, cancellationToken).ConfigureAwait(false);
        await _userUnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return result.Adapt<UserResponseModel>();
    }

    public async Task<UserResponseModel> Login(LoginModel loginModel, CancellationToken cancellationToken)
    {
        var targetUser = await _userUnitOfWork.UserRepository.GetByEmail(loginModel.Email, cancellationToken).ConfigureAwait(false);
        if (targetUser == null)
            throw new UserWithEmailNotFoundException($"User with email: {loginModel.Email} not found");

        if(!targetUser.EmailConfirmed)
            throw new UserNotFoundException($"User with specified email {loginModel.Email} is not found");

        if (!_passwordHasherUtility.VerifyHash(targetUser.PasswordHashe, loginModel.Password))
            throw new InvalidPasswordException("Invalid password");

        return targetUser.Adapt<UserResponseModel>();
    }

    public async Task<UserResponseModel> ConfirmEmail(string userId, CancellationToken cancellationToken)
    {
        var targetUser = await _userUnitOfWork.UserRepository.Get(userId, cancellationToken).ConfigureAwait(false);
        if (targetUser == null)
            throw new UserNotFoundException($"User with id: {userId} not found");

        targetUser.EmailConfirmed = true;
        var updatedUser = await _userUnitOfWork.UserRepository.Update(targetUser, cancellationToken).ConfigureAwait(false);
        await _userUnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return updatedUser.Adapt<UserResponseModel>();
    }

}
