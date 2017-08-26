using System.ComponentModel.DataAnnotations;

namespace SecuredCRM.Models
{
    public class TutorIndexModel
	{
		[Key]
		[Display(Name = "מפתח")]
		public string Id { get; set; }

		//[RegularExpression(@"^[0-9]{9}+", ErrorMessage = "בעל 9 ספרות")]
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

		[Display(Name = "מספר טלפון")]
		public string PhoneNumber { get; set; }

	}
}