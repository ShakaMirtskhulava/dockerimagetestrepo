using System.Security.Claims;
using System.Text;
using GHotel.API.Infrastructure.Configuration;
using GHotel.Application.Models.User;
using GHotel.Application.Services.UserService;
using GHotel.Application.Utilities;
using GHotel.Infrastructure.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace GHotel.API.Infrastructure.Authentication;

public class SignInManager : ISignInManager
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserService _userService;
    private readonly FrontEndConfiguration _frontEndConfiguration;
    private readonly AppConfiguration _appConfiguration;
    private readonly EmailConfiguration _emailConfiguration;
    private readonly IEmailSenderUtility _emailSender;

    public SignInManager(IUserService userService, IHttpContextAccessor httpContextAccessor,
        IOptions<FrontEndConfiguration> frontEndConfigurationOptions,
        IEmailSenderUtility emailSenderUtilities, IOptions<AppConfiguration> appConfigurationOptions,
        IOptions<EmailConfiguration> emailConfigurationOptions)
    {
        _userService = userService;
        _httpContextAccessor = httpContextAccessor;
        _frontEndConfiguration = frontEndConfigurationOptions.Value;
        _emailSender = emailSenderUtilities;
        _appConfiguration = appConfigurationOptions.Value;
        _emailConfiguration = emailConfigurationOptions.Value;
    }

    public string? GetUserIdFromToken(string token)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_emailConfiguration.Key));
        var handler = new JsonWebTokenHandler();
        var claimsPrincipal = handler.ValidateToken(token, new TokenValidationParameters
        {
            IssuerSigningKey = key,
            ValidIssuer = _appConfiguration.BaseUrl,
            ValidateIssuer = false,
            ValidateLifetime = true,
            ValidateAudience = false
        });

        return claimsPrincipal.ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier)!.Value;
    }

    public async Task LogInMain(UserResponseModel user, CancellationToken cancellationToken)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier,user.Id),
            new Claim(ClaimTypes.Email, user.Email)
        };
        var roleClaims = new List<Claim>();
        if(user.Roles != null && user.Roles.Count >= 1)
            foreach (var role in user.Roles)
                roleClaims.Add(new Claim(ClaimTypes.Role, role));
        claims.AddRange(roleClaims);
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var userPrinciples = new ClaimsPrincipal(identity);

        await SignIn(userPrinciples, CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);
    }

    public async Task<UserResponseModel> Register(string email, bool emailConfirmed,string? password, CancellationToken cancellationToken)
    {
        var newUser = new UserRequestModel
        {
            Email = email,
            EmailConfirmed = emailConfirmed,
            Password = password
        };
        var userResponseModel = await _userService.Register(newUser, cancellationToken).ConfigureAwait(false);
        return userResponseModel;
    }

    public async Task SignIn(ClaimsPrincipal user, string scheme)
    {
        await _httpContextAccessor.HttpContext!.SignInAsync(scheme, user, new AuthenticationProperties
        {
            IsPersistent = true
        }).ConfigureAwait(false);
    }

    public async Task SignOut(string scheme)
    {
        await _httpContextAccessor.HttpContext!.SignOutAsync(scheme, new AuthenticationProperties
        {
            IsPersistent = true
        }).ConfigureAwait(false);
    }

    public async Task<bool> SendRegistrationEmailConfirmationAsync(string userId, string userEmail, CancellationToken cancellationToken)
    {
        var frontEndDomain = _frontEndConfiguration.Origin;
        var frontEndEmailConfirmationEndpointUrl = _frontEndConfiguration.EmailConfirmationPage;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_emailConfiguration.Key));
        var handler = new JsonWebTokenHandler();
        var token = handler.CreateToken(new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, userEmail)
            }),
            Issuer = frontEndDomain,
            Expires = DateTime.UtcNow.AddMinutes(10),
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
        });

        var confirmationLink = $"{frontEndDomain}{frontEndEmailConfirmationEndpointUrl}?token={token}";
        var subject = "Confirm your email";
        await _emailSender.SendEmailConfirmationAsync(subject, userEmail, confirmationLink, cancellationToken).ConfigureAwait(false);

        return true;
    }

}
