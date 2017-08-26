using System.Collections.Generic;

namespace SecuredCRM.Models
{
    public class TutorViewModel
	{
		public virtual ApplicationUser ApplicationUser { get; set; }
		public virtual List<Course> Courses { get; set; }
		public virtual List<Tigbur> Tigburs { get; set; }
		public virtual List<UnavailableInDate> UnavailableInDates { get; set; }
	}
}

//namespace SecuredCRM.Models
//{
//	public class Functionality
//	{
//		private ApplicationDbContext db = new ApplicationDbContext();
//		public IQueryable<Course> GetCoursesByUserId(string UserId)
//		{
//			return ( db.CourseTutors
//				.Where(u => u.ApplicationUser.Id == UserId)
//				.Select(u => u.Course));
//		}
//		public IQueryable<Tigbur> GetTigbursByUserId(string UserId)
//		{
//			return db.Tigburs.Where(x => x.CourseTutor.ApplicationUser.Id == UserId);
//		}

//		public IQueryable<UnavailableInDate> GetUnavailableInDatesByUserId(string UserId)
//		{
//			return db.UnavailableInDates.Where(x => x.ApplicationUser.Id == UserId);
//		}
//		public UnavailableInDate CreateUnavailableInDate(string UserId, DateTime start, DateTime end)
//		{
//			if (UserId == null || start == null || end == null)
//			{
//				throw new ArgumentNullException();
//			}
//			var UnavailableInDates = GetUnavailableInDatesByUserId(UserId);
//			var user = db.Users.Find(UserId);
//			if (user == null) throw new ArgumentNullException();
//			foreach (var un in UnavailableInDates)
//			{
//				if (un.StartDate > start && un.EndDate > end)
//				{
//					break;

//				}
//				else if (un.StartDate < start && un.EndDate < end)
//				{
//					break;
//				}
//				else throw new ArgumentOutOfRangeException("collision of dates thet alresdy exists");
//			}
//			var newUnavailableInDate = db.UnavailableInDates.Add(new UnavailableInDate
//			{
//				ApplicationUser = user,
//				StartDate = start,
//				EndDate = end
//			});
//			db.SaveChanges();
//			return newUnavailableInDate;
//		}
//		public UnavailableInDate DeleteUnavailableInDate(UnavailableInDate dates)
//		{
//			if (dates == null)
//			{
//				throw new ArgumentNullException();
//			}
//			var DeleteUnavailableInDate = db.UnavailableInDates.Remove(dates);
//			db.SaveChanges();
//			return DeleteUnavailableInDate;
//		}

//		private UnavailableInDate GetUnavailableInDatesByDates(UnavailableInDate dates) {
//			return db.UnavailableInDates.Where(un => un == dates).SingleOrDefault();
//		}


//		public void Dispose()
//		{
//			db.Dispose();
//		}
//	}
//}
