using Microsoft.AspNet.Identity;
using SecuredCRM.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace SecuredCRM.Controllers
{
    [Authorize(Roles ="Tutor")]
	public class TutorUserController : Controller
	{
		public TutorUserController()
		{
		}
		private ApplicationDbContext db = new ApplicationDbContext();

		[HttpGet]
		public async Task<ActionResult> Index()
		{
			var usrId = User.Identity.GetUserId();
			var myCourses = await db.CourseTutors.Where(a => a.ApplicationUser.Id == usrId).Select(c => c.Course).ToListAsync();
			var myTigburs = await db.Tigburs.Include(t => t.Category).Include(t => t.CourseTutor).Where(a => a.CourseTutor.ApplicationUser.Id == usrId).ToListAsync();
			var myUnavailableInDates = await db.UnavailableInDates.Where(u => u.ApplicationUser.Id == usrId).ToListAsync();
			return View(new IndexTutorViewModel()
			{
				myId = usrId,
				myCourses = myCourses,
				myTigburs = myTigburs,
				myUnavailableInDates = myUnavailableInDates
			});
		}

		// GET: UnavailableInDates/Crete/1
		[HttpGet]
		public ActionResult UnavailableDateCreate()
		{
			return View();
		}


		// POST: UnavailableInDates/Create/1
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UnavailableDateCreate(UnavailableDateCreateVM uv)
		{
			var userId = User.Identity.GetUserId();
			try
			{
				if (ModelState.IsValid && uv != null && uv.StartDate != null && uv.EndDate != null)
				{
					if (IsOkToCreateUnavailableInDates(userId, uv.StartDate, uv.EndDate))
					{
						if( IsExistUnavailableInDate(userId, uv.StartDate, uv.EndDate) != null)	return RedirectToAction("Index");
						UnavailableInDate newUnavailableInDate = db.UnavailableInDates.Add(NewUnavailableInDate(userId, uv.StartDate, uv.EndDate));
						var success = await db.SaveChangesAsync();
						return RedirectToAction("Index");
					}
				}
			}
			catch (DataException  dex )
			{
				//Log the error (uncomment dex variable name and add a line here to write a log.
				ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
			}
			return RedirectToAction("Index");
		}

		// GET: UnavailableInDates/Delete/5
		[HttpGet]
		public async Task<ActionResult> DeleteUnavailableInDate(UnavailableInDate un)
		{
			if (un == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			UnavailableInDate old = IsExistUnavailableInDate(un.ApplicationUserId, un.StartDate, un.EndDate);
			return old != null ? (ActionResult)View(old) : HttpNotFound();
		}

		// POST: UnavailableInDates/Delete/5
		[HttpPost, ActionName("DeleteUnavailableInDate")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteConfirmedUnavailableInDate(UnavailableInDate un)
		{
			UnavailableInDate old = IsExistUnavailableInDate(un.ApplicationUserId, un.StartDate, un.EndDate);
			db.UnavailableInDates.Remove(old);
			db.Entry(old).State = EntityState.Deleted;
			await db.SaveChangesAsync();
			return RedirectToAction("Index");
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		// HELPERS

		public UnavailableInDate IsExistUnavailableInDate(string ApplicationUserId, DateTime StartDate, DateTime EndDate)
		{
			if (ApplicationUserId == null || StartDate == null || EndDate == null)
			{
				return null;
			}

			var unavailableDate = db.UnavailableInDates
				.SingleOrDefault(e => e.ApplicationUserId == ApplicationUserId &&
									e.StartDate == StartDate &&
									e.EndDate == EndDate);
			if (unavailableDate == null)
			{
				return null;
			}
			return unavailableDate;
		}
		public ICollection<UnavailableInDate> GetUnavailableInDatesListForUser(string ApplicationUserId)
		{
			return db.UnavailableInDates
				.Include(x => x.ApplicationUser)
				.Where(e => e.ApplicationUserId == ApplicationUserId)
				.ToList();
		}
		public bool IsOkToUpdateUnavailableInDates(string ApplicationUserId, DateTime NewStartDate, DateTime NewEndDate, DateTime OldStartDate, DateTime OldEndDate)
		{
			if ((ApplicationUserId == null || NewStartDate == null || NewEndDate == null || OldStartDate == null || OldEndDate == null) ||
				db.Users.Find(ApplicationUserId) == null || NewStartDate > NewEndDate) return false;
			UnavailableInDate old = IsExistUnavailableInDate(ApplicationUserId, OldStartDate, OldEndDate);
			var unavailableDateList = GetUnavailableInDatesListForUser(ApplicationUserId);
			if (old == null || unavailableDateList.Count == 0) return false;

			foreach (var item in unavailableDateList)
				if (item.StartDate > NewStartDate && item.EndDate > NewEndDate) continue;
				else if (item.StartDate < NewStartDate && item.EndDate < NewEndDate) continue;
				else if (item.Equals(old)) continue;
				else return false;
			return true;
		}
		public bool IsOkToCreateUnavailableInDates(string ApplicationUserId, DateTime StartDate, DateTime EndDate)
		{
			if ((StartDate == null || EndDate == null || ApplicationUserId == null) || (StartDate > EndDate) ||
				db.Users.Find(ApplicationUserId) == null) return false;

			var unavailableDateList = GetUnavailableInDatesListForUser(ApplicationUserId);
			UnavailableInDate old = IsExistUnavailableInDate(ApplicationUserId, StartDate, EndDate);
			foreach (var item in unavailableDateList)
				if (item.StartDate > StartDate && item.EndDate > EndDate) continue;
				else if (item.StartDate < StartDate && item.EndDate < EndDate) continue;
				else if (item.Equals(old)) continue;
				else return false;
			return true;
		}
		public UnavailableInDate NewUnavailableInDate(string ApplicationUserId, DateTime StartDate, DateTime EndDate)
		{
			if ((StartDate == null || EndDate == null || ApplicationUserId == null) || (StartDate > EndDate) ||
				db.Users.Find(ApplicationUserId) == null) return null;
			var exist = IsExistUnavailableInDate(ApplicationUserId, StartDate, EndDate);
			if (exist != null) return exist;
			return new UnavailableInDate()
			{
				ApplicationUserId = ApplicationUserId,
				StartDate = new DateTime(year: StartDate.Year, month: StartDate.Month, day: StartDate.Day),
				EndDate = new DateTime(year: EndDate.Year, month: EndDate.Month, day: EndDate.Day)
			};
		}
		public new void Dispose()
		{
			GC.SuppressFinalize(this);
			db.Dispose();
		}

		[HttpGet]
		public async Task<ActionResult> ShowMyTigburs()
		{
			var userId = User.Identity.GetUserId();
			return View(await db.Tigburs
				.Where(u => u.CourseTutor.ApplicationUser.Id == userId)
				.Include(t => t.CourseTutor.Course)
				.Include(t => t.CourseTutor.ApplicationUser).AsNoTracking()
				.ToListAsync());
		}

		[HttpGet]
		public async Task<ActionResult> ShowMyCourses()
		{
			var userId = User.Identity.GetUserId();
			return View(await db.CourseTutors
				.Where(u => u.ApplicationUser.Id == userId)
				.Select(t => t.Course)
				.ToListAsync<Course>());
		}

		[HttpGet]
		public async Task<ActionResult> ShowMyUnavailableDates()
		{
			var userId = User.Identity.GetUserId();
			return View(await db.UnavailableInDates
				.Where(u => u.ApplicationUser.Id == userId)
				.ToListAsync());
		}

	}
}


		//[HttpGet]
		//public async Task<ActionResult> EditUnavailableDates(UnavailableInDate un)
		//{
		//	UnavailableInDate unavailableDate = IsExistUnavailableInDate(un.ApplicationUserId, un.StartDate, un.EndDate);
		//	if (unavailableDate == null)
		//	{
		//		return HttpNotFound();
		//	}

		//	return View(new UnavailableDateVM()
		//	{
		//		ApplicationUserId = unavailableDate.ApplicationUserId,

		//		OldStartDate = new DateTime(unavailableDate.StartDate.Year, unavailableDate.StartDate.Month, unavailableDate.StartDate.Day),
		//		NewStartDate = new DateTime(unavailableDate.StartDate.Year, unavailableDate.StartDate.Month, unavailableDate.StartDate.Day),

		//		OldEndDate = new DateTime(unavailableDate.EndDate.Year, unavailableDate.EndDate.Month, unavailableDate.EndDate.Day),
		//		NewEndDate = new DateTime(unavailableDate.EndDate.Year, unavailableDate.EndDate.Month, unavailableDate.EndDate.Day)
		//	});
		//}
		//[ValidateAntiForgeryToken]
		//[HttpPost]
		//public async Task<ActionResult> EditUnavailableDates([Bind(Include = "ApplicationUserId,OldStartDate,OldEndDate,NewStartDate,NewEndDate")] UnavailableDateVM unavailableDateVM)
		//{
		//	if (ModelState.IsValid && unavailableDateVM != null &&
		//		IsOkToUpdateUnavailableInDates(unavailableDateVM.ApplicationUserId, unavailableDateVM.NewStartDate, unavailableDateVM.NewEndDate, unavailableDateVM.OldStartDate, unavailableDateVM.OldEndDate))
		//	{
		//		UnavailableInDate ToBechanged = NewUnavailableInDate(unavailableDateVM.ApplicationUserId, unavailableDateVM.NewStartDate, unavailableDateVM.NewEndDate);
		//		UnavailableInDate old = IsExistUnavailableInDate(unavailableDateVM.ApplicationUserId, unavailableDateVM.OldStartDate, unavailableDateVM.OldEndDate);
		//		if (ToBechanged != null && old != null)
		//		{
		//			try
		//			{
		//				await DeleteUnavailableInDateAsync(old);
		//			}
		//			catch (Exception dex)
		//			{
		//				ModelState.AddModelError("", dex.Message);
		//			}
		//			try
		//			{
		//				await UnavailableDateCreateAsync(ToBechanged);
		//			}
		//			catch (Exception dex)
		//			{

		//				ModelState.AddModelError("", dex.Message);
		//			}
		//			return RedirectToAction("Index");
		//		}
		//	}

		//	ModelState.AddModelError("", "Unable to save changes. " +
		//				"Try again, and if the problem persists, " +
		//				"see your system administrator.");
		//	return RedirectToAction("Index");
		//}
		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public async Task<object> UnavailableDateCreateAsync(UnavailableInDate toBechanged)
		//{
		//	if (ModelState.IsValid && toBechanged != null)
		//	{
		//		try
		//		{
		//			var newUnavailableInDate = db.UnavailableInDates.Add(toBechanged);
		//			return await db.SaveChangesAsync();
		//		}
		//		catch (DataException dex)
		//		{
		//			//Log the error (uncomment dex variable name and add a line here to write a log.
		//			ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
		//			return dex;
		//		}
		//	}
		//	return HttpNotFound();
		//}
		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public async Task<Object> DeleteUnavailableInDateAsync(UnavailableInDate toBedeleted)
		//{
		//	if (ModelState.IsValid && toBedeleted != null)
		//	{
		//		try
		//		{
		//			UnavailableInDate old = IsExistUnavailableInDate(toBedeleted.ApplicationUserId, toBedeleted.StartDate, toBedeleted.EndDate);
		//			db.UnavailableInDates.Remove(old);
		//			db.Entry(old).State = EntityState.Deleted;
		//			return await db.SaveChangesAsync();
		//		}
		//		catch (DataException dex)
		//		{
		//			//Log the error (uncomment dex variable name and add a line here to write a log.
		//			ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
		//			return dex;
		//		}
		//	}
		//	return HttpNotFound();
		//}