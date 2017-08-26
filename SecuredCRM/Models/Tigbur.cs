using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecuredCRM.Models
{
    public class Tigbur
	{
		[Key, Column(Order = 0)]
		[Display(Name = "מספר")]
		public int Id { get; set; }

		[Required]
		[ForeignKey("CourseTutor"), Column(Order = 1)]
		[Display(Name = "קורס")]
		public int CourseId { get; set; }

		[Required]
		[ForeignKey("CourseTutor"), Column(Order = 2)]
		[Display(Name = "מתגבר")]
		public string ApplicationUserId { get; set; }

		[ForeignKey("Category")]
		[Display(Name = "קטגוריה")]
		public int? CategoryId { get; set; }

		[Required]
		[Display(Name = "מתוגבר")]
		public string TuteeName { get; set; }

		[Required]
		[DataType(DataType.EmailAddress)]
		[Display(Name = "אימייל המתוגבר")]
		public string TuteeEmail { get; set; }

		[Required]
		[Display(Name = "טלפון המתוגבר")]
		[DataType(DataType.PhoneNumber)]
		public string TuteePhone { get; set; }

		[Required]
		[Display(Name = "תחילת ההקצאה")]
		[DataType(DataType.DateTime), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
		public DateTime AssignmentStartDate		{ get; set; }
		//{ get { return this.AssignmentStartDate; } set { AssignmentStartDate = DateTime.Today; } }
		[Required]
		[Display(Name = "סוף ההקצאה")]
		[DataType(DataType.DateTime), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
		public DateTime AssignmentEndDate { get; set; }

		[Required]
		[Display(Name = "הוקצו")]
		public double AssignmentTotal { get; set; }

		[Display(Name = "בוצעו")]
		public double AssignmentDone { get; set; }


		[Display(Name = "אושרו")]
		public double AssignmentDoneApproved { get; set; }


		[Display(Name = "למעקב")]
		public bool FollowUp { get; set; }


		[Display(Name = "הערות")]
		public string Comments { get; set; }

		//[Display(Name = "הקצאה נסתיימה")]
		//public bool IsAssignmentActive()
		//{
		//	return AssignmentTotal > AssignmentDone ? true : false;
		//}
		//[Display(Name = "שעות אושרו")]
		//public bool IsAssignmentApproved()
		//{
		//	return AssignmentDoneApproved != AssignmentDone ? true : false;
		//}

		public virtual Category Category { get; set; }

		public virtual CourseTutor CourseTutor { get; set; }
	}
}