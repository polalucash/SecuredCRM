using Microsoft.AspNet.Identity.Owin;
using SecuredCRM.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SecuredCRM.Controllers
{
    [Authorize(Roles = "TutorAdmin,Admin")]
	public class TutorAdminController : Controller
	{
		public TutorAdminController()
		{
		}
		private ApplicationDbContext db = new ApplicationDbContext();
		public TutorAdminController(ApplicationUserManager userManager,
			ApplicationRoleManager roleManager)
		{
			UserManager = userManager;
			RoleManager = roleManager;
		}

		private ApplicationUserManager _userManager;
		public ApplicationUserManager UserManager
		{
			get {
				return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
			}
			set {
				_userManager = value;
			}
		}

		private ApplicationRoleManager _roleManager;
		public ApplicationRoleManager RoleManager
		{
			get {
				return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
			}
			private set {
				_roleManager = value;
			}
		}
		//
		// GET: /Tutors/
		public async Task<ActionResult> Index()
		{
			var role = await RoleManager.FindByNameAsync("Tutor");
			// Get the list of Users in this Role
			var users = new List<ApplicationUser>();

			// Get the list of Users in this Role
			foreach (var user in UserManager.Users.ToList())
			{
				if (await UserManager.IsInRoleAsync(user.Id, role.Name))
				{
					users.Add(user);
				}
			}
			return View(users);
		}

		[HttpGet]
		public async Task<ActionResult> Details(string id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			ViewBag.CoursesList = new SelectList(db.Courses, "Id", "Name");
			var usr = await UserManager.FindByIdAsync(id);
			if (usr != null)
			{
				var usrCourses = await db.CourseTutors.Where(a => a.ApplicationUser.Id == id).Select(c => c.Course).AsNoTracking().ToListAsync();
				var usrTigburs = await db.Tigburs.Include(t => t.Category).Include(t => t.CourseTutor).Where(a => a.CourseTutor.ApplicationUser.Id == id).AsNoTracking().ToListAsync();
				var usrUnavailableInDates = await db.UnavailableInDates.Where(u => u.ApplicationUser.Id == id).AsNoTracking().ToListAsync();
				if (usrUnavailableInDates != null)
				{
					return View(new TutorViewModel()
					{
						ApplicationUser = usr,
						Tigburs = usrTigburs,
						Courses = usrCourses,
						UnavailableInDates = usrUnavailableInDates
					});
				}
			}
			return HttpNotFound();
		}

		//
		// GET: /Users/Create
		public ActionResult Create()
		{
			return View();
		}

		//
		// POST: /Users/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create(RegisterViewModel userViewModel)
		{
			if (ModelState.IsValid)
			{
				   var user = new ApplicationUser
				{
					TrueId = userViewModel.TrueId,
					UserName = userViewModel.Email,
					Email = userViewModel.Email,
					FirstName = userViewModel.FirstName,
					LastName = userViewModel.LastName,
					Campus = userViewModel.Campus,
					PhoneNumber = userViewModel.PhoneNumber,
					LockoutEnabled = true,
					TwoFactorEnabled = true,
					EmailConfirmed = false
				};

				// Then create:
				var adminresult = await UserManager.CreateAsync(user, "User@123456");

				//Add User to the selected Roles
				if (adminresult.Succeeded)
				{
					var result = await UserManager.AddToRoleAsync(user.Id, "Tutor");
					if (!result.Succeeded)
					{
						ModelState.AddModelError("", result.Errors.First());
						return View();
					}
					var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
					var callbackUrl = Url.Action("ConfirmEmail", "Account",
						new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
					await UserManager.SendEmailAsync(user.Id,
						"Confirm your account",
						"Please confirm your account by clicking this link: <a href=\""
							+ callbackUrl + "\">link</a>");
					//ViewBag.Link = callbackUrl;
					return RedirectToAction("ConfrimationEmailSent");
				}
				else
				{
					ModelState.AddModelError("", adminresult.Errors.First());
					return View();

				}
				return RedirectToAction("Index");
			}
			return View();
		}
		public ActionResult ConfrimationEmailSent()
		{
			return View();
		}
		//
		// GET: /Users/Edit/1
		[HttpGet]
		public async Task<ActionResult> Edit(string id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var user = await UserManager.FindByIdAsync(id);
			if (user == null)
			{
				return HttpNotFound();
			}

			//var usrCourses = await db.CourseTutors.Where(a => a.ApplicationUser.Id == id).Select(c => c.Course).AsNoTracking().ToListAsync();

			//ViewBag.Courses = usrCourses;

			//ViewBag.CountCourses = usrCourses.Count;
			//ViewBag.CourseId = new SelectList(db.Courses, "Id", "Name");

			//var userRoles = await UserManager.GetRolesAsync(user.Id);

			return View(new TutorIndexModel()
			{

				Id = user.Id,
				Email = user.Email,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Campus = user.Campus,
				PhoneNumber = user.PhoneNumber
			});
		}


		// POST: /Users/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		//public async Task<ActionResult> Edit([Bind(Include = "Email,Id,Address,City,State,PostalCode")] EditUserViewModel editUser, params string[] selectedRole)
		public async Task<ActionResult> Edit([Bind(Include = "Email,Id,FirstName,LastName,Campus,Campus,PhoneNumber")] TutorIndexModel editUser)
		{
			if (ModelState.IsValid)
			{
				var user = await UserManager.FindByIdAsync(editUser.Id);
				if (user == null)
				{
					return HttpNotFound();
				}

				user.FirstName = editUser.FirstName;
				user.LastName = editUser.LastName;
				user.Campus = editUser.Campus;
				user.PhoneNumber = editUser.PhoneNumber;

				user.UserName = editUser.Email;
				user.Email = editUser.Email;

				var result = await UserManager.UpdateAsync(user);

				if (!result.Succeeded)
				{
					ModelState.AddModelError("", result.Errors.First());
					return View();
				}
				return RedirectToAction("Index");
			}
			ModelState.AddModelError("", "Something failed.");
			return View();
		}

		//
		// GET: /Users/Delete/5
		public async Task<ActionResult> Delete(string id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var user = await UserManager.FindByIdAsync(id);
			if (user == null)
			{
				return HttpNotFound();
			}
			return View(user);
		}

		//
		// POST: /Users/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteConfirmed(string id)
		{
			if (ModelState.IsValid)
			{
				if (id == null)
				{
					return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
				}
				var usr = await UserManager.FindByIdAsync(id);
				if (usr == null)
				{
					return HttpNotFound();
				}
				//foreach (var item in usr.CourseTutors)
				//{
				//	db.CourseTutors.Remove(item);
				//}
				//db.SaveChanges();
				//foreach (var item in usr.UnavailableInDates)
				//{
				//	db.UnavailableInDates.Remove(item);
				//}
				//db.SaveChanges();

				var result = await UserManager.DeleteAsync(usr);
				if (!result.Succeeded)
				{
					ModelState.AddModelError("", result.Errors.First());
					return View();
				}
				return RedirectToAction("Index");
			}
			return View();
		}

		public  ActionResult DeleteCourseTutor(string ApplicationUserId, int CourseId)
		{
			if (ModelState.IsValid)
			{
				if (ApplicationUserId == null || CourseId == 0)
				{
					return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
				}
				var courseTutors = db.CourseTutors.Where(x=>x.ApplicationUserId == ApplicationUserId && x.CourseId == CourseId  ).Single();
				db.CourseTutors.Remove(courseTutors);
				db.SaveChanges();
				return RedirectToAction("Details", new { id = ApplicationUserId });
			}
			return RedirectToAction("Index");
		}

		public ActionResult InsertCourse(int? id, string ApplicationUserId)
		{
			if(ModelState.IsValid)
			{
				if(!id.HasValue|| ApplicationUserId == null)
				{
					return RedirectToAction("Details", new { id = ApplicationUserId });
				}
				var courseTutors = db.CourseTutors.Find(id, ApplicationUserId);
				if(courseTutors == null)
				{
					db.CourseTutors.Add(new CourseTutor() {CourseId = id.Value,ApplicationUserId = ApplicationUserId});
					db.SaveChanges();
					return RedirectToAction("Details", new { id = ApplicationUserId });
				}
				else return RedirectToAction("Details", new { id = ApplicationUserId });


			}
			return  RedirectToAction("Index");
		}
	}
}