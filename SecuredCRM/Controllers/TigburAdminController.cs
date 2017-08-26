using PagedList;
using SecuredCRM.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
namespace SecuredCRM.Controllers
{
    [Authorize(Roles = "TutorAdmin,Admin")]
	public class TigburAdminController : Controller
	{
		private ApplicationDbContext db = new ApplicationDbContext();

		public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
		{
			ViewBag.CurrentSort = sortOrder;
			ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "tutee_desc" : "";
			ViewBag.DateSortParm = sortOrder == "startDate" ? "date_desc" : "startDate";

			if (searchString != null)
			{
				page = 1;
			}
			else
			{
				searchString = currentFilter;
			}

			ViewBag.CurrentFilter = searchString;

			var tigburs = from s in db.Tigburs
						  .Include(x => x.CourseTutor.ApplicationUser)
						  .Include(x => x.CourseTutor.Course)
						  .Include(x => x.Category)
						  select s;

			if (!String.IsNullOrEmpty(searchString))
			{
				tigburs = tigburs.Where(s => s.TuteeName.Contains(searchString) || s.CourseTutor.ApplicationUser.FirstName.Contains(searchString) || s.CourseTutor.ApplicationUser.LastName.Contains(searchString) ||
				  s.CourseTutor.Course.Name.Contains(searchString));
			}
			switch (sortOrder)
			{
				case "tutee_desc":
					tigburs = tigburs.OrderByDescending(s => s.TuteeName);
					break;
				case "startDate":
					tigburs = tigburs.OrderBy(s => s.AssignmentStartDate);
					break;
				case "date_desc":
					tigburs = tigburs.OrderByDescending(s => s.AssignmentStartDate);
					break;
				default:
					tigburs = tigburs.OrderBy(s => s.TuteeName);
					break;
			}
			int pageSize = 3;
			int pageNumber = (page ?? 1);
			return View(tigburs.ToPagedList(pageNumber, pageSize));
		}



		// GET: Tigburs
		//public async Task<ActionResult> Index()
		//{
		//	var tigburs = db.Tigburs.Include(t => t.Category).Include(t => t.CourseTutor);
		//	return View(await tigburs.ToListAsync());
		//}
		// GET: Tigburs/Details/5
		public async Task<ActionResult> Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Tigbur tigbur = await db.Tigburs.FindAsync(id);
			if (tigbur == null)
			{
				return HttpNotFound();
			}
			return View(tigbur);
		}
		// GET: Tigburs/Delete/5
		[HttpGet]
		public async Task<ActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Tigbur tigbur = await db.Tigburs.FindAsync(id);
			if (tigbur == null)
			{
				return HttpNotFound();
			}
			return View(tigbur);
		}
		// POST: Tigburs/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteConfirmed(int id)
		{
			Tigbur tigbur = await db.Tigburs.FindAsync(id);
			db.Tigburs.Remove(tigbur);
			await db.SaveChangesAsync();
			return RedirectToAction("Index");
		}
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}


		// method gets a list of all Categorys sorted by name, creates a SelectList collection for a drop-down list, and passes the collection to the view in a ViewBag "CategoryId" property.
		// The method accepts the optional "selectedCategorie" parameter that allows the calling code to specify the item that will be selected when the drop-down list is rendered.
		private void PopulateCategorysDropDownList(object selectedCategorie = null)
		{
			ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", selectedCategorie);
		}

		// method gets a list of all Courses sorted by name, creates a SelectList collection for a drop-down list, and passes the collection to the view in a ViewBag "CourseId" property.
		// The method accepts the optional "selectedCourse" parameter that allows the calling code to specify the item that will be selected when the drop-down list is rendered.
		private void PopulateCoursesDropDownList(object selectedCourse = null)
		{
			ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name", selectedCourse);
		}

		// method gets a list of all Tutors sorted by name, creates a SelectList collection for a drop-down list, and passes the collection to the view in a ViewBag "TutorId" property.
		// The method accepts the optional "selectedTutor" parameter that allows the calling code to specify the item that will be selected when the drop-down list is rendered.
		private void PopulateTutorsDropDownList(object selectedTutor = null)
		{
			ViewBag.ApplicationUserId = new SelectList(db.CourseTutors.Select(a => a.ApplicationUser).Distinct(), "Id", "Name", selectedTutor);
		}

		// method gets a list of all Tutors sorted by name and selectedCourse from Course drop-down list , creates a SelectList collection for a drop-down list, and passes the collection to the view in a ViewBag "TutorId" property.
		// the method is calld from "SelectTutor"  after "SelectCourse" View perform this.form.submit() onchange of the Course drop-down list.
		private void PopulateTutorsDropDownListByValue()
		{
			int id = Convert.ToInt32(Request["Id"]);
			ViewBag.ApplicationUserId = new SelectList(db.CourseTutors.Where(x => x.CourseId == id).Select(a => a.ApplicationUser).Distinct(), "Id", "Name");
		}

		private void PopulateTutorsDropDownListByValue(int id)
		{
			ViewBag.ApplicationUserId = new SelectList(db.CourseTutors.Where(x => x.CourseId == id).Select(a => a.ApplicationUser).Distinct(), "Id", "Name");
		}

		// The method calls the PopulateCoursesDropDownList method without setting the selected item, because for a new course is not established yet.
		// method calls SelectCourse View to select a course to add to the new tigbur.
		// on this.form.submit() the View call SelectTutor ActionResult.
		public ActionResult SelectCourse()
		{
			PopulateCoursesDropDownList();
			return View();
		}

		// method set TempData["CourseId"] to selectedCourse Value for to save the selected Course for the Insert ActionResult.
		// The method calls the PopulateTutorsDropDownListByValue.
		// method calls SelectTutor View to select a Tutor to add to the new tigbur.
		// on this.form.submit() the View call CreateTigbur ActionResult.
		public ActionResult SelectTutor(int? id)
		{
			TempData["CourseId"] = id.Value;
			PopulateTutorsDropDownListByValue();
			return View();
		}

		// method set TempData["ApplicationUserId"] to selectedCourse Value for to save the selected Tuter for the Insert ActionResult.
		// The method calls the PopulateCategorysDropDownList method without setting the selected item, because for a new Category is not established yet.
		// method calls CreateTigbur View to input data on the new tigbur.
		// on submit  Insert ActionResult is called to add the tigbur to the database.
		public ActionResult CreateTigbur(string id)
		{
			TempData["ApplicationUserId"] = id;
			PopulateCategorysDropDownList();
			return View();
		}
		/// <summary>
		/// method  get new tigbur data, check if tigbur exists if not add the tigbur to the database and redirect to action "Index".
		/// is tigbur exists redirect to action "Index" without saving the tigbur.
		/// </summary>
		/// <param name="tigbur"></param>
		/// <returns></returns>
		///
		///
		///
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Insert(int CourseId, string ApplicationUserId, int CategoryId,
			string TuteeName, string TuteeEmail, string TuteePhone,
			double AssignmentTotal, bool FollowUp, string Comments = null)
		{
			try
			{
				Tigbur tigbur = new Tigbur()
				{
					TuteeName = TuteeName,
					TuteeEmail = TuteeEmail,
					TuteePhone = TuteePhone,
					ApplicationUserId = ApplicationUserId,
					CourseId = CourseId,
					CategoryId = CategoryId,
					AssignmentStartDate = DateTime.Today,
					AssignmentEndDate = DateTime.Today.AddMonths(1),
					AssignmentTotal = AssignmentTotal,
					AssignmentDone = 0,
					AssignmentDoneApproved = 0,
					FollowUp = FollowUp,
					Comments = Comments
				};
				db.Tigburs.Add(tigbur);
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			catch (DataException dex)
			{
				//Log the error (uncomment dex variable name and add a line here to write a log.
				ViewBag.errorMessage = dex.Message;
				return RedirectToAction("Error");
			}
		}

		/// <summary>
		/// method get as parameter tigbur id and search in database for the tigbur, if no tigbur is found return HttpNotFound.
		/// The method calls the PopulateCoursesDropDownList method with setting the selected item as tigbur.CourseId.
		/// method calls SelectCourseForEdit View to select a course  to the Edit tigbur.
		/// on View submit the SelectTutorForEdit ActionResult is called.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public async Task<ActionResult> SelectCourseForEdit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Tigbur tigbur = await db.Tigburs.FindAsync(id);
			if (tigbur == null)
			{
				return HttpNotFound();
			}
			PopulateCoursesDropDownList(tigbur.CourseId);
			return View(tigbur);

		}

		/// <summary>
		///  method get as parameter tigbur model  if no tigbur was receivedis return HttpNotFound.
		///  method set TempData["CourseId"] to selectedCourse Value for to save the selected Course for the Edit ActionResult.
		///  The method calls the PopulateTutorsDropDownListByValue.
		///  method calls SelectTutorForEdit View to select a Tutor  to the Edit tigbur.
		///  on View submit the Edit ActionResult is called.
		/// </summary>
		/// <param name="tigbur"></param>
		/// <returns></returns>
		public ActionResult SelectTutorForEdit(Tigbur tigbur)
		{
			if (tigbur == null)
			{
				return HttpNotFound();
			}

			TempData["CourseId"] = tigbur.CourseId;
			var course = db.Courses.Find(tigbur.CourseId);
			TempData["CourseName"] = course.Name;
			PopulateTutorsDropDownListByValue(tigbur.CourseId);
			return View(tigbur);
		}


		/// <summary>
		///  method set TempData["ApplicationUserId"] to selectedCourse Value for to save the selected Tuter for the UpdateTigbur ActionResult.
		///  The method calls the PopulateCategorysDropDownList method without setting the selected item, because for a new Category is not established yet.
		///  method calls Edit View to input Edit data on the tigbur.
		///  on submit UpdateTigbur ActionResult is called to Update the tigbur to the database.
		/// </summary>
		/// <param name="tigbur"></param>
		/// <returns></returns>
		///

		public ActionResult Edit(Tigbur tigbur)
		{
			if (tigbur == null)
			{
				return HttpNotFound();
			}
			TempData["ApplicationUserId"] = tigbur.ApplicationUserId;

			var Tutor = db.Users.Find(tigbur.ApplicationUserId);

			TempData["ApplicationUserName"] = Tutor.Name;
			PopulateCategorysDropDownList();
			return View(tigbur);
		}
		/// <summary>
		/// method  get new tigbur data, check if tigbur exists and Update it and  redirect to action "Index".
		/// is tigbur do not exists redirect to action "Index" without saving the tigbur.
		/// </summary>
		/// <param name="tigbur"></param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateTigbur([Bind(Include = "Id,CourseId,ApplicationUserId,CategoryId,TuteeName,TuteeEmail,TuteePhone,AssignmentStartDate,AssignmentEndDate,AssignmentTotal,AssignmentDone,AssignmentDoneApproved,FollowUp,Comments")] Tigbur tigbur)
		{
			if (ModelState.IsValid)
			{
				db.Entry(tigbur).State = EntityState.Modified;
				await db.SaveChangesAsync();
				return RedirectToAction("Index");
			}
			return View(tigbur);

		}
	}
}
