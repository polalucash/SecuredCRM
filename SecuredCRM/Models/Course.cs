using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SecuredCRM.Models
{
    public class Course
	{
		[Key]
		public int Id { get; set; }
		[Required(AllowEmptyStrings = false)]
		[Display(Name = "קורס")]
		public string Name { get; set; }
		[Display(Name = "תאור")]
		public string Description { get; set; }
		private ICollection<CourseTutor> _courseTutors;
		public virtual ICollection<CourseTutor> CourseTutors
		{
			get { return _courseTutors ?? (_courseTutors = new List<CourseTutor>()); }
			set { _courseTutors = value; }
		}

	}
}