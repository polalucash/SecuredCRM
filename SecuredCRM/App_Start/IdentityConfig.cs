using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace SecuredCRM.Models
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public class ApplicationUserManager : UserManager<ApplicationUser>
	{
		public ApplicationUserManager(IUserStore<ApplicationUser> store)
			: base(store)		{	}

		public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options,
			IOwinContext context)
		{
			var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
			// Configure validation logic for usernames
			manager.UserValidator = new UserValidator<ApplicationUser>(manager)
			{
				AllowOnlyAlphanumericUserNames = false,
				RequireUniqueEmail = true
			};

			// Configure validation logic for passwords
			manager.PasswordValidator = new PasswordValidator
			{
				RequiredLength = 6,
				RequireNonLetterOrDigit = true,
				RequireDigit = true,
				RequireLowercase = true,
				RequireUppercase = true,
			};
			// Configure user lockout defaults
			manager.UserLockoutEnabledByDefault = true;
			manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
			manager.MaxFailedAccessAttemptsBeforeLockout = 5;
			// Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
			// You can write your own provider and plug in here.
			//manager.RegisterTwoFactorProvider("PhoneCode",
			//	new PhoneNumberTokenProvider<ApplicationUser>
			//{
			//	MessageFormat = "Your security code is: {0}"
			//});
			manager.RegisterTwoFactorProvider("EmailCode",
				new EmailTokenProvider<ApplicationUser>
			{
				Subject = "SecurityCode",
				BodyFormat = "Your security code is {0}"
			});
			manager.EmailService = new EmailService();
			manager.SmsService = new SmsService();
			var dataProtectionProvider = options.DataProtectionProvider;
			if (dataProtectionProvider != null)
			{
				manager.UserTokenProvider =
					new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider
					.Create("ASP.NET Identity"));
			}
			return manager;
		}
	}

	// Configure the RoleManager used in the application. RoleManager is defined in the ASP.NET Identity core assembly
	public class ApplicationRoleManager : RoleManager<ApplicationRole>
	{
		public ApplicationRoleManager(IRoleStore<ApplicationRole,string> roleStore)
			: base(roleStore)
		{
		}

		public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
		{
			return new ApplicationRoleManager(new RoleStore<ApplicationRole>(context.Get<ApplicationDbContext>()));
		}

	}

	public class EmailService : IIdentityMessageService
	{
		private const string SmtpClient = "smtp.gmail.com";
		private const int PORT = 587;
		public Task SendAsync(IdentityMessage message)
		{
			SmtpClient client = new SmtpClient(host: SmtpClient,port: PORT);
			client.DeliveryMethod = SmtpDeliveryMethod.Network;
			client.UseDefaultCredentials = true;
			client.EnableSsl = true;
			client.Credentials = new NetworkCredential(
					   ConfigurationManager.AppSettings["mailAccount"],
					   ConfigurationManager.AppSettings["mailPassword"]
					   );

			// Create the message:
			var mail = new MailMessage(from: ConfigurationManager.AppSettings["mailAccount"],
										to: message.Destination,
										subject: message.Subject,
										body: message.Body);
			// Send:
			return client.SendMailAsync(mail);
		}
	}

	public class SmsService : IIdentityMessageService
	{
		public Task SendAsync(IdentityMessage message)
		{

			// Twilio does not return an async Task, so we need this:
			return Task.FromResult(0);
		}
	}

	// This is useful if you do not want to tear down the database each time you run the application.
	// public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
	// This example shows you how to create a new database if the Model changes
	public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
	{
		protected override void Seed(ApplicationDbContext context) {
			InitializeIdentityForEF(context);
			base.Seed(context);
		}

		//Create User="tutorsceacc@gmail.com" with password="Admin@123456" in the Admin role
		public static void InitializeIdentityForEF(ApplicationDbContext db) {
			var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
			var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();

			const string password = "Admin@123456";

			//const string name1 = "tutorsceacc@gmail.com";
			//const string roleName1 = "Admin";

			//const string name1 = "scetutorco@gmail.com";
			//const string roleName1 = "TutorAdmin";

			var users = new List<ApplicationUser>
			{
				new ApplicationUser{FirstName = "חיה",LastName = "קוקו",UserName = "tutorsceacc@gmail.com",Email = "tutorsceacc@gmail.com",PhoneNumber = "1234567890",Campus = "BS",TrueId = "0123654789", EmailConfirmed=true},
				//0
				new ApplicationUser{FirstName = "שרון",LastName = "קיקנו",UserName = "scetutorco@gmail.com",Email = "scetutorco@gmail.com",PhoneNumber = "1234512890",Campus = "BS",TrueId = "0236598789", EmailConfirmed=true},
				//1
				new ApplicationUser{FirstName="חן",LastName="שיבב",UserName = "tutor@gmail.com", Email = "tutor@gmail.com", PhoneNumber = "1234567890",Campus = "BS",  TrueId = "0123654789", EmailConfirmed=true},
				//2
				new ApplicationUser{FirstName="אריה",LastName="סבאן",UserName = "tut@gmail.com", Email = "tut@gmail.com", PhoneNumber = "1231567290",Campus = "BS",  TrueId = "0123094789", EmailConfirmed=true},
				//3
				new ApplicationUser{FirstName="קרון",LastName="גל",UserName = "Kaskasof1@gmail.com", Email = "Kaskasof1@gmail.com", PhoneNumber = "1234513890", Campus = "BS", TrueId = "0123654789", EmailConfirmed=true},
				//4
				new ApplicationUser{FirstName="תהילה",LastName="אהרונוב",UserName = "asof1@gmail.com", Email = "asof1@gmail.com", PhoneNumber = "1224513890", Campus = "BS", TrueId = "0113654789", EmailConfirmed=true},
				//5
				new ApplicationUser{FirstName="יאן",LastName="שמעונוב",UserName = "a3sof1@gmail.com", Email = "a3sof1@gmail.com", PhoneNumber = "1225813890", Campus = "BS", TrueId = "0044654789", EmailConfirmed=true},
				//6
				new ApplicationUser{FirstName="אוהד",LastName="ג'סטיס",UserName = "tu22tor@gmail.com", Email = "tu22tor@gmail.com", PhoneNumber = "1534567859",Campus = "BS",  TrueId = "0139654649", EmailConfirmed=true},
				//7
				new ApplicationUser{FirstName="Laura",LastName="Kapoor",UserName = "tu2nxor@gmail.com", Email = "tu2nxor@gmail.com", PhoneNumber = "1334553859",Campus = "BS",  TrueId = "9296545449", EmailConfirmed=true},
				//8
				new ApplicationUser{FirstName="חנן",LastName="סלומונוב",UserName = "a332vf1@gmail.com", Email = "a332vf1@gmail.com", PhoneNumber = "0044654789", Campus = "BS", TrueId = "1534567859", EmailConfirmed=true}
				//9
			};

			//Create Role Admin if it does not exist

			var user = userManager.FindByName(users[0].UserName);
			if (user == null)
			{
				foreach (var i in users)
				{
					var result = userManager.Create(i, password);
					result = userManager.SetLockoutEnabled(i.Id, true);
				}
			}

			var roles = new List<ApplicationRole>
			{
				new ApplicationRole("Admin"),
				new ApplicationRole("TutorAdmin"),
				new ApplicationRole("Tutor")
			};
			var role = roleManager.FindByName(roles[0].Name);
			if (role == null)
			{
				foreach (var i in roles)
				{
					var roleresult = roleManager.Create(i);
				}
			}
			// Add user admin to Role Admin if not already added

			var rolesForUser0 = userManager.GetRoles(users[0].Id);
			var roleResult0 = userManager.AddToRole(users[0].Id, roles[0].Name);
			var rolesForUser1 = userManager.GetRoles(users[1].Id);
			var roleResult1 = userManager.AddToRole(users[1].Id, roles[1].Name);

			for(var i =2; i< users.Count; i++)
			{
				var rolesForUser = userManager.GetRoles(users[i].Id);
				var roleResult = userManager.AddToRole(users[i].Id, roles[2].Name);
			}

			////////////////////////////////////////////////////////////

			/////////////////////////////////////////////////
			var unavailableInDates = new List< UnavailableInDate>()
			{
				new UnavailableInDate{ApplicationUserId = users[3].Id, StartDate = new DateTime(2017, 7, 5), EndDate = new DateTime(2017,7,12)},
				new UnavailableInDate{ApplicationUserId = users[4].Id, StartDate = new DateTime(2017, 11, 5), EndDate = new DateTime(2017,11,12)},
				new UnavailableInDate{ApplicationUserId = users[5].Id, StartDate = new DateTime(2017, 11, 5), EndDate = new DateTime(2017,11,12)},
				new UnavailableInDate{ApplicationUserId = users[6].Id, StartDate = new DateTime(2017, 9, 1), EndDate = new DateTime(2017,10,29)},
				new UnavailableInDate{ApplicationUserId = users[7].Id, StartDate = new DateTime(2017, 10, 5), EndDate = new DateTime(2017,10,12)},
				new UnavailableInDate{ApplicationUserId = users[9].Id, StartDate = new DateTime(2017, 8, 1), EndDate =new DateTime(2017,8,10)},
				new UnavailableInDate{ApplicationUserId = users[6].Id, StartDate = new DateTime(2017, 7, 5), EndDate = new DateTime(2017,7,12)}
			};

			foreach (var s in unavailableInDates)
			{
				db.UnavailableInDates.Add(s);
			}
			db.SaveChanges();

			var Courses = new Course[] {
				new Course{ Description = "",   Name = "בדיקות תוכנה"},//0
				new Course{ Description = "",   Name = "בטיחות תוכנה"},//1
				new Course{ Description = "", Name= "אמינות" },//2
				new Course{ Description = "", Name= "סימולציה" },//3
				new Course{ Description =  "", Name = "חדווא"},//4
				new Course{ Description =  "", Name = "אלגוריתמים 1"},//5
				new Course{ Description = "", Name= "אלגוריתמים 2" },//6
				new Course{ Description = "היום, אלגוריתמים 1", Name= "מבוא לאלגוריתמים" },//7
				new Course{ Description = "", Name= "חישוביות וסיבוכיות" },//8
				new Course{ Description = "", Name= "אלגברה ליניארית" }//9
			};

			foreach (var s in Courses)
			{
				db.Courses.Add(s);
			}
			db.SaveChanges();

			var courseTutors = new CourseTutor[]
			{
				new CourseTutor{ApplicationUserId = users[3].Id, CourseId =Courses[0].Id},
				new CourseTutor{ApplicationUserId = users[4].Id, CourseId =Courses[1].Id},
				new CourseTutor{ApplicationUserId = users[5].Id, CourseId =Courses[2].Id},
				new CourseTutor{ApplicationUserId = users[6].Id, CourseId =Courses[3].Id},
				new CourseTutor{ApplicationUserId = users[7].Id, CourseId =Courses[4].Id},
				new CourseTutor{ApplicationUserId = users[8].Id, CourseId =Courses[5].Id},
				new CourseTutor{ApplicationUserId = users[4].Id, CourseId =Courses[6].Id},
				new CourseTutor{ApplicationUserId = users[5].Id, CourseId =Courses[0].Id},
				new CourseTutor{ApplicationUserId = users[6].Id, CourseId =Courses[1].Id},
				new CourseTutor{ApplicationUserId = users[7].Id, CourseId =Courses[2].Id},
				new CourseTutor{ApplicationUserId = users[8].Id, CourseId =Courses[3].Id},
				new CourseTutor{ApplicationUserId = users[9].Id, CourseId =Courses[4].Id}
			};

			foreach (var s in courseTutors)
			{
				db.CourseTutors.Add(s);
			}
			db.SaveChanges();

			var categories = new Category[]
			{
				new Category{Name = "בסיסי",   Description = "כללי" },
				new Category{Name =  "לשנה ראשונה",   Description = "משרד הביטחון" }
			};

			foreach (var s in categories)
			{
				db.Categories.Add(s);
			}
			db.SaveChanges();

			/////////////////////////////////////////


			var Tigburs = new Tigbur[]
			{
				new Tigbur
				{
					TuteeName = "אוהד כהן",
					TuteeEmail = "Ohad@gmail.com",
					TuteePhone = "0542283364",
					CourseTutor = courseTutors.SingleOrDefault(i =>i == courseTutors[0]),
					CategoryId = categories[0].CategoryId,
					AssignmentStartDate = DateTime.Today,
					AssignmentEndDate = DateTime.Today.AddMonths(1),
					AssignmentTotal = 6,
					AssignmentDone = 0,
					AssignmentDoneApproved = 0,
					FollowUp =false,
					Comments = "החסיר משיעור אחד"
				},
				new Tigbur
				{
					TuteeName = "שרון צמח",
					TuteeEmail = "sharon@gmail.com",
					TuteePhone = "0542283364",
					CourseTutor = courseTutors.SingleOrDefault(i =>i == courseTutors[1]),
					CategoryId = categories[1].CategoryId,
					AssignmentStartDate = DateTime.Today,
					AssignmentEndDate = DateTime.Today.AddMonths(1),
					AssignmentTotal = 6,
					AssignmentDone = 0,
					AssignmentDoneApproved = 0,
					FollowUp =false,
					Comments = "צתקשה"
				},
				new Tigbur
				{
					TuteeName = "הילי פרידמן",
					TuteeEmail = "hili2@gmail.com",
					TuteePhone = "0578733224",
					CourseTutor = courseTutors.SingleOrDefault(i =>i == courseTutors[2]),
					CategoryId = categories[1].CategoryId,
					AssignmentStartDate = DateTime.Today,
					AssignmentEndDate = DateTime.Today.AddMonths(1),
					AssignmentTotal = 6,
					AssignmentDone = 0,
					AssignmentDoneApproved = 0,
					FollowUp =false,
					Comments = ""
				},
				new Tigbur
				{
					TuteeName = "לירון סילברמן",
					TuteeEmail = "silverman@gmail.com",
					TuteePhone = "0542283309",
					CourseTutor = courseTutors.SingleOrDefault(i =>i == courseTutors[3]),
					CategoryId = categories[0].CategoryId,
					AssignmentStartDate = DateTime.Today,
					AssignmentEndDate = DateTime.Today.AddMonths(1),
					AssignmentTotal = 6,
					AssignmentDone = 0,
					AssignmentDoneApproved = 0,
					FollowUp =false,
					Comments = "מחכה לאישור מהמלגה"
				},
				new Tigbur
				{
					TuteeName = "אוהד לוי",
					TuteeEmail = "levilevi@gmail.com",
					TuteePhone = "0542233364",
					CourseTutor = courseTutors.SingleOrDefault(i =>i == courseTutors[4]),
					CategoryId = categories[0].CategoryId,
					AssignmentStartDate = DateTime.Today,
					AssignmentEndDate = DateTime.Today.AddMonths(1),
					AssignmentTotal = 6,
					AssignmentDone = 0,
					AssignmentDoneApproved = 0,
					FollowUp =false,
					Comments = ""
				},
				new Tigbur
				{
					TuteeName = "שלמה קשלו",
					TuteeEmail = "shaulim@gmail.com",
					TuteePhone = "0542283309",
					CourseTutor = courseTutors.SingleOrDefault(i =>i == courseTutors[5]),
					CategoryId = categories[0].CategoryId,
					AssignmentStartDate = DateTime.Now,
					AssignmentEndDate = DateTime.Now.AddMonths(1),
					AssignmentTotal = 6,
					AssignmentDone = 0,
					AssignmentDoneApproved = 0,
					FollowUp =false,
					Comments = ""
				},
				 new Tigbur
				{
					TuteeName = "עזרה קלו",
					TuteeEmail = "haaronv@gmail.com",
					TuteePhone = "0542283123",
					CourseTutor = courseTutors.SingleOrDefault(i =>i == courseTutors[6]),
					CategoryId = categories[1].CategoryId,
					AssignmentStartDate = DateTime.Now,
					AssignmentEndDate = DateTime.Now.AddMonths(1),
					AssignmentTotal = 6,
					AssignmentDone = 0,
					AssignmentDoneApproved = 0,
					FollowUp =false,
					Comments = "מחכה למתגבר פנוי"
				}
			};


			foreach (var s in Tigburs)
			{
				db.Tigburs.Add(s);
			}
			db.SaveChanges();
		}
	}

	public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
	{
		public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager) :
			base(userManager, authenticationManager) { }

		public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
		{
			return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
		}

		public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
		{
			return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
		}
	}
}