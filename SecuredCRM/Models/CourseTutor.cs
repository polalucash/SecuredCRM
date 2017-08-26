using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecuredCRM.Models
{
    public class CourseTutor
	{
		[Required]
		[Key, ForeignKey("Course")]
		[Column(Order = 0), Display(Name ="קורס")]

		public int CourseId { get; set; }
		[Required]
		[Key, ForeignKey("ApplicationUser"), Display(Name = "מתגבר")]
		[Column(Order = 1)]

		public string ApplicationUserId { get; set; }
		public virtual Course Course { get; set; }
		public  virtual ApplicationUser ApplicationUser { get; set; }
		private ICollection<Tigbur> _tigburs;

		public virtual ICollection<Tigbur> Tigburs
		{
			get { return _tigburs ?? (_tigburs = new HashSet<Tigbur>()); }
			set { _tigburs = value; }
		}
	}
}