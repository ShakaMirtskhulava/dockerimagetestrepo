using System.Security.Claims;
using Asp.Versioning;
using GHotel.API.Infrastructure.Authentication;
using GHotel.API.Infrastructure.Authentication.Google;
using GHotel.API.Infrastructure.Configuration;
using GHotel.API.Infrastructure.Error;
using GHotel.API.Models.User;
using GHotel.Application.Exceptions;
using GHotel.Application.Models.User;
using GHotel.Application.Services.UserService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GHotel.API.Controllers.V1;

[ApiController]
[Route("v{version:apiversion}/[controller]")]
[ApiVersion(1)]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    private readonly ISignInManager _signinManager;
    private readonly FrontEndConfiguration _frontEndConfiguration;

    public UserController(ILogger<UserController> logger, ISignInManager authenticationManager, IUserService userService,IOptions<FrontEndConfiguration> frontEndConfiguration)
    {
        _logger = logger;
        _signinManager = authenticationManager;
        _userService = userService;
        _frontEndConfiguration = frontEndConfiguration.Value;
    }

    /// <summary>
    /// Get's the currently authenticated user
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <response code="200">if the user is found</response>
    /// <response code="401">if the user is unauthenticated</response>
    /// <response code="500">if there is problem on the server</response>
    [Authorize(Policy = "Authenticated")]
    [HttpGet("GetCurrentUser")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(UserResponseModel), 200)]
    [ProducesResponseType(typeof(APIError), 401)]
    [ProducesResponseType(typeof(APIError), 500)]
    public async Task<UserResponseModel> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        return await _userService.Get(userId,cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Challeng the google scheme and redirects to the google login page
    /// </summary>
    [Authorize(Policy = "NotAuthenticated")]
    [HttpGet("GoogleLogin")]
    public async Task GoogleLogin()
    {
        var properties = new AuthenticationProperties { RedirectUri = Url.Action(nameof(GoogleResponse)) };
        await HttpContext.ChallengeAsync(GoogleAuthenticationDefaults.AuthenticationScheme, properties).ConfigureAwait(false);
    }

    [HttpGet("GoogleResponse")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GoogleResponse(CancellationToken cancellationToken)
    {
        var email = User.FindFirst(ClaimTypes.Email)!.Value;
        try
        {
            var targetUser = await _userService.GetByEmail(email, cancellationToken).ConfigureAwait(false);
            await _signinManager.LogInMain(targetUser, cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("User @{0} logged in",targetUser);
        }
        catch (UserWithEmailNotFoundException)
        {
            var newUser = await _signinManager.Register(email, true,null, cancellationToken).ConfigureAwait(false);
            await _signinManager.LogInMain(newUser, cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("New user registered and logged in @{0}",newUser);
        }
        return Redirect($"{_frontEndConfiguration.Origin}{_frontEndConfiguration.GoogleResponsePage}");
    }

    /// <summary>
    /// Confirm the email of the user
    /// </summary>
    /// <response code="200">User email is confirmed</response>
    /// <response code="400">if the token is invalid</response>
    /// <response code="500">if there is problem on the server</response>
    [HttpPost("Login")]
    [ProducesResponseType(typeof(UserResponseModel), 200)]
    [ProducesResponseType(typeof(APIError), 400)]
    [ProducesResponseType(typeof(APIError), 500)]
    public async Task<UserResponseModel> Login(LoginModel userLoginModel, CancellationToken cancellationToken)
    {
        var user = await _userService.Login(userLoginModel, cancellationToken).ConfigureAwait(false);
        await _signinManager.LogInMain(user, cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("User @{0} logged in", user);
        return user;
    }

    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <response code="200">if the user is registered successfully</response>
    /// <response code="400">if the email is taken</response>
    /// <response code="500">if there is problem on the server</response>
    [HttpPost("Register")]
    public async Task<string> Register(RegisterModel userRegisterModel,CancellationToken cancellationToken)
    {
        var newUser = await _signinManager.Register(userRegisterModel.Email, false,userRegisterModel.Password, cancellationToken).ConfigureAwait(false);
        await _signinManager.SendRegistrationEmailConfirmationAsync(newUser.Id,newUser.Email,cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("New user registered and logged in @{0}",newUser);
        return "Confirm Your Email";
    }

    /// <summary>
    /// Confirm the email of the user
    /// </summary>
    /// <response code="200">User email is confirmed</response>
    /// <response code="400">if the token is invalid</response>
    /// <response code="500">if there is problem on the server</response>
    [HttpPut("RegistrationEmailConfirmation")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(APIError), 400)]
    [ProducesResponseType(typeof(APIError), 500)]
    public async Task RegistrationEmailConfirmation([FromQuery] string token,CancellationToken cancellationToken)
    {
        var userId = _signinManager.GetUserIdFromToken(token)!;
        if(userId == null)
            throw new Exception("Invalid Token");
        await _userService.ConfirmEmail(userId, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes the cookies and logs out the user
    /// </summary>
    /// <response code="200">if the user is successfully logged out</response>
    /// <response code="401">if the user is unauthenticated</response>
    /// <response code="500">if there is problem on the server</response>
    [Authorize(Policy = "Authenticated")]
    [HttpGet("Logout")]
    [Produces("application/json")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(APIError), 401)]
    [ProducesResponseType(typeof(APIError), 500)]
    public async Task Logout()
    {
        await _signinManager.SignOut(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);
    }

}
