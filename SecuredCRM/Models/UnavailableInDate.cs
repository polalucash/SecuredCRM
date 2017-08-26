using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecuredCRM.Models
{
    public class UnavailableInDate
	{
		[Key,ForeignKey("ApplicationUser") ,Column(Order = 0)]
		public string ApplicationUserId { get; set; }

		[Key, Column(Order = 1)]
		[Display(Name = "התחלה")]
		[DataType(DataType.DateTime), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
		public DateTime StartDate { get; set; }

		[Key ,Column(Order = 2)]
		[Display(Name = "סופי")]
		[DataType(DataType.DateTime), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
		public DateTime EndDate { get; set; }

		public virtual ApplicationUser ApplicationUser { get; set; }

		public override bool Equals(Object obj)
		{
			if (obj != null && obj.GetType() == typeof(UnavailableInDate))
			{
				UnavailableInDate other = (UnavailableInDate)obj;
				if (other.ApplicationUserId != null || other.StartDate != null || other.EndDate != null||
					(this.StartDate.Day == other.StartDate.Day &&
					this.StartDate.Month == other.StartDate.Month &&
					this.StartDate.Year == other.StartDate.Year &&
					this.EndDate.Day == other.EndDate.Day &&
					this.EndDate.Month == other.EndDate.Month &&
					this.EndDate.Year == other.EndDate.Year &&
					this.ApplicationUserId == other.ApplicationUserId)) return true;
			}

			return false;
		}
	}
}