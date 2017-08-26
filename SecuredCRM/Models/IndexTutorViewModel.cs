using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SecuredCRM.Models
{
    public class IndexTutorViewModel
	{
		[Key]
		public string myId { get; set; }
		public IList<Course> myCourses { get; set; }
		public IList<Tigbur> myTigburs { get; set; }
		public IList<UnavailableInDate> myUnavailableInDates { get; set; }
	}
}