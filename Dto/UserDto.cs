using System.ComponentModel.DataAnnotations;

namespace HaniApi.Dto
{
	public class UserDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

	public class RegisterViewModel
	{
		public string UserName { get; set; }

		public string FirstName { get; set; }
		public string LastName { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

	}

	public class ApiResult
	{
		public ResultCode ResultCode { get; set; } = ResultCode.Success;
		public string ResultMessage { get; set; }
	}

	public enum ResultCode
	{
		Success = 0,
		TwoFactoreEnabled = 1,
		Error = 2
	}
}
