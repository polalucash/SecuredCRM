using System;
using System.ComponentModel.DataAnnotations;

namespace SecuredCRM.Models
{
    public class UnavailableDateVM
	{
		[Required]
		public string ApplicationUserId { get; set; }
		[Required]
		[DataType(DataType.DateTime)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
		[Display(Name = "תאריך התחלתי ישן")]
		public DateTime OldStartDate { get; set; }
		[Required]
		[DataType(DataType.DateTime)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
		[Display(Name = "תאריך סופי ישן")]
		public DateTime OldEndDate { get; set; }
		[Required]
		[Display(Name = "תאריך התחלה")]
		[DataType(DataType.DateTime)]
		[ DisplayFormat( ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
		public DateTime NewStartDate { get; set; }
		[Required]
		[Display(Name = "תאריך סופי")]
		[DataType(DataType.DateTime)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
		public DateTime NewEndDate { get; set; }
	}

	public class UnavailableDateCreateVM
	{
		[Required]
		[Display(Name = "תאריך התחלה")]
		[DataType(DataType.Date), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
		public DateTime StartDate { get; set; }
		[Required]
		[Display(Name = "תאריך סופי")]
		[DataType(DataType.Date), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
		public DateTime EndDate { get; set; }

	}
}