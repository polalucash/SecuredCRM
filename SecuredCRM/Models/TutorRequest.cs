using System.ComponentModel.DataAnnotations;

namespace SecuredCRM.Models
{
    public class TutorRequest
	{
		public TutorRequest()
		{
			IsAnswered = false;
			Answer = "";
		}
		[Key]
		public int Id { get; set; }
		[Required]
		[Display(Name = "סוג")]
		public RequestType Requestype { get; set; }
		[Required]
		[Display(Name = "בקשה")]
		public string Request { get; set; }
		[Display(Name = "טופל")]
		public bool IsAnswered { get; set; }
		public string Answer { get; set; }
		[Required]
		public string ApplicationUserId { get; set; }
		public virtual ApplicationUser ApplicationUser { get; set; }

	}
	public enum RequestType
	{
		[Display(Name = "לא פעיל")]
		UnavailableIndatesRequest,
		[Display(Name = "שינוי פרטים אישיים")]
		DetailChangeRequest,
		[Display(Name = "כללית")]
		GeneralRequest,
		[Display(Name = "חוסר תקינות")]
		WentWrongRequest,
		[Display(Name = "הפסקת פעילות")]
		EndRequest,
		[Display(Name = "אישור חניך")]
		TutorApprovalRequest
	}
	public class TutorUserRequestViewModel
	{
		[Key]
		public int Id { get; set; }
		[Display(Name = "סוג")]
		public RequestType Requestype { get; set; }

		[Display(Name = "בקשה")]
		public string Request { get; set; }
		[Required]
		public string ApplicationUserId { get; set; }
		public virtual ApplicationUser ApplicationUser { get; set; }
	}
}