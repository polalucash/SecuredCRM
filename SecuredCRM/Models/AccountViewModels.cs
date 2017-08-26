using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SecuredCRM.Models
{

    public class SendCodeViewModel
	{
		public string SelectedProvider { get; set; }
		public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
		public string ReturnUrl { get; set; }
	}

	public class VerifyCodeViewModel
	{
		[Required]
		public string Provider { get; set; }

		[Required]
		[Display(Name = "קוד")]
		public string Code { get; set; }
		public string ReturnUrl { get; set; }

		[Display(Name = "זכור את הדפדפן")]
		public bool RememberBrowser { get; set; }
	}

	public class ForgotViewModel
	{
		[Required]
		[Display(Name = "אימייל")]
		public string Email { get; set; }
	}

	public class LoginViewModel
	{
		[Required]
		[Display(Name = "אימייל")]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "סיסמה")]
		public string Password { get; set; }

		[Display(Name = "זכור אותי")]
		public bool RememberMe { get; set; }
	}

	public class RegisterViewModel
	{
		//[RegularExpression("^[A-Z]{0,1}[a-z]+$")]

		//[RegularExpression(@"^[0-9]+", ErrorMessage = "בעל 9 ספרות")]
		[Display(Name = "תעודת זהות")]
		public string TrueId { get; set; }

		[Display(Name = "שם פרטי")]
		public string FirstName { get; set; }

		[Display(Name = "שם משפחה")]
		public string LastName { get; set; }

		[Display(Name = "קמפוס")]
		public string Campus { get; set; }

		[EmailAddress]
		[Display(Name = "אימייל")]
		public string Email { get; set; }

		[Phone]
		[Display(Name = "מספר טלפון")]
		public string PhoneNumber { get; set; }

		//[Required]
		//[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		//[DataType(DataType.Password)]
		//[Display(Name = "סיסמה")]
		//public string Password { get; set; }

		//[DataType(DataType.Password)]
		//[Display(Name = "חזור על הסיסמה")]
		//[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		//public string ConfirmPassword { get; set; }


	}

	public class ResetPasswordViewModel
	{
		[Required]
		[EmailAddress]
		[Display(Name = "אימייל")]
		public string Email { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "סיסמה")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "חזור על הסיסמה")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		public string Code { get; set; }
	}

	public class ForgotPasswordViewModel
	{
		[Required]
		[EmailAddress]
		[Display(Name = "אימייל")]
		public string Email { get; set; }
	}
	public class ManageUserViewModel
	{
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "סיסמה נוכחית")]
		public string OldPassword { get; set; }

		[Required]
		[StringLength(100, ErrorMessage =
			"The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "סיסמה חדשה")]
		public string NewPassword { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "חזור על הסיסמה")]
		[Compare("NewPassword", ErrorMessage =
			"The new password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
	}
}