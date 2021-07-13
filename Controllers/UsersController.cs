using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HaniApi.Dto;
using HaniApi.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using HaniApi.Config;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace HaniApi.Controllers
{
	[Authorize]
	[ApiController]
	[Route("[controller]")]
	public class UsersController : Controller
	{
		private readonly UserManager<HaniUser> _userManager;
		private readonly ILogger<UsersController> _logger;
		private readonly AppSettings _appSettings;
		private readonly SignInManager<HaniUser> _signInManager;

		public UsersController(
			UserManager<HaniUser> userManager,
			SignInManager<HaniUser> signInManager,
			ILogger<UsersController> logger,
			IOptions<AppSettings> appSettings)
		{
			_userManager = userManager;
			_logger = logger;
			_signInManager = signInManager;
			_appSettings = appSettings.Value;
		}

		[AllowAnonymous]
		[HttpPost("Login")]
		public async Task<IActionResult> Login([FromBody] UserDto Input)
		{
			var result = await _signInManager.PasswordSignInAsync(Input.Username, Input.Password, true, lockoutOnFailure: true);
			if (result.Succeeded)
			{
				var tokenHandler = new JwtSecurityTokenHandler();
				var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
				var tokenDescriptor = new SecurityTokenDescriptor
				{
					Subject = new ClaimsIdentity(new Claim[]
					{
						new Claim(ClaimTypes.Name,Input.Username)
					}),
					Expires = DateTime.Now.AddDays(30),
					SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
				};
				var token = tokenHandler.CreateToken(tokenDescriptor);
				var tokenString = tokenHandler.WriteToken(token);

				return new JsonResult(new ApiResult { ResultMessage = tokenString });
			}

			if (result.IsLockedOut)
			{
				return new JsonResult(new ApiResult { ResultCode = ResultCode.Error, ResultMessage = "User account locked out." });
			}
			else
			{
				return new JsonResult(new ApiResult { ResultCode = ResultCode.Error, ResultMessage = "You login info is invalid" });
			}

		}

		[HttpPost("Register")]
		[AllowAnonymous]
		public async Task<JsonResult> Register([FromBody] RegisterViewModel Input)
		{
			var user = new HaniUser
			{
				UserName = Input.UserName,
				FirstName = Input.FirstName,
				LastName = Input.LastName,
				EmailConfirmed = true
			};
			var result = await _userManager.CreateAsync(user, Input.Password);
			if (result.Succeeded)
			{
				_logger.LogInformation("User created a new account with password.");


				await _signInManager.SignInAsync(user, isPersistent: false);
				return new JsonResult(new ApiResult { ResultMessage = "Register Succeed" });

			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (var error in result.Errors)
				{
					stringBuilder.Append(error.Description);
					stringBuilder.Append("\r\n");
				}
				return new JsonResult(new ApiResult
				{
					ResultCode = ResultCode.Error,
					ResultMessage = $"Register Failed: {stringBuilder}"
				});
			}

		}
	}
}
