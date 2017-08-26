using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SecuredCRM.Models
{
    public class RoleViewModel
	{
		public string Id { get; set; }
		[Required(AllowEmptyStrings = false)]
		[Display(Name = "הרשאה")]
		public string Name { get; set; }
		[Display(Name = "תאור")]
		public string Description { get; set; }
	}

	public class EditUserViewModel
	{
		[Key]
		[Display(Name = "מפתח")]
		[Required]
		public string Id { get; set; }

		[Display(Name = "שם פרטי")]
		public string FirstName { get; set; }

		[Display(Name = "שם משפחה")]
		public string LastName { get; set; }

		[Display(Name = "קמפוס")]
		public string Campus { get; set; }

		[Display(Name = "תעודת זהות")]
		public string TrueId { get; set; }

		[EmailAddress]
		[Display(Name = "אימייל")]
		public string Email { get; set; }

		[Display(Name = "מספר טלפון")]
		public string PhoneNumber { get; set; }

		public IEnumerable<SelectListItem> RolesList { get; set; }
	}

}