using GLSPM.Application.AppServices.Interfaces;
using GLSPM.Application.Dtos;
using GLSPM.Application.Dtos.Identity;
using GLSPM.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HeyRed.Mime;
using GLSPM.Domain.Dtos.Identity;
using GLSPM.Domain.Dtos;

namespace GLSPM.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly ILogger<AccountsController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthenticationAppService _authenticationAppService;

        public AccountsController(ILogger<AccountsController> logger,
            UserManager<ApplicationUser> userManager,
            IAuthenticationAppService authenticationAppService)
        {
            _logger = logger;
            _userManager = userManager;
            _authenticationAppService = authenticationAppService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register()
        {
            return Ok();
        }

        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginUserDto input)
        {
            _logger.LogInformation("Attimpting to login a user...");
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("invalid model passed.");
                var response = new SingleObjectResponse<object>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "invalid model passed.",
                    Error = ModelState
                };
                return BadRequest(response);
            }
            if (!await _authenticationAppService.ValidateUser(input))
            {
                _logger.LogWarning("User not found");
                var response = new SingleObjectResponse<object>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "User not found",
                    Error = "incorrect username or password"
                };
                return NotFound(response);
            }
            else
            {
                _logger.LogInformation("User logged");
                _logger.LogInformation("Attempting to preparing the response...");
                //preparing the user data
                var authUser = _authenticationAppService.User;
                var loginresponse = new LoginResponseDto()
                {
                    UserID = authUser.Id,
                    Email = authUser.Email,
                    Username = authUser.UserName,
                    Avatar = Url.Action(nameof(UserAvatar), "Accounts", new { userid = authUser.Id }, Request.Scheme)
                };
                loginresponse.Roles = await _userManager.GetRolesAsync(authUser);
                loginresponse.IsAppAdmin = loginresponse.Roles.Contains("Admin");
                _logger.LogInformation("User data is ready");
                //preparing and setting the token model
                var tokenmodel = await _authenticationAppService.CreateUserToken();
                loginresponse.Token = tokenmodel.Token;
                loginresponse.TokenExpirationDate = tokenmodel.Expiration;
                _logger.LogInformation("Token is ready");
                var response = new SingleObjectResponse<LoginResponseDto>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Login Success",
                    Data = loginresponse
                };
                return Ok(response);
            }
        }

        [Authorize]
        [HttpGet("UserAvatar/{userID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UserAvatar(string userID)
        {
            var user = await _userManager.FindByIdAsync(userID);
            if (user != null)
            {
                user.ImagePath ??= Path.GetFullPath("./Files/Imgs/Userimage.png");
                return PhysicalFile(user.ImagePath, MimeTypesMap.GetMimeType(user.ImagePath), $"avatar{Path.GetExtension(user.ImagePath)}");
            }
            return NotFound(new SingleObjectResponse<object>
            {
                Success = false,
                StatusCode = StatusCodes.Status404NotFound,
                Message = "User not found",
                Error = "Couldn't find a user related to the passed id"
            });
        }
    }
}
