using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SecuredCRM.Models
{
    public class ApplicationUser : IdentityUser
	{
		//public ApplicationUser() : base()
		//{
		//	EmailConfirmed = false;
		//}

		[Required]
		[EmailAddress]
		[Display(Name = "אימייל")]
		override public string Email { get; set; }
		[Phone]
		[Required]
		[Display(Name = "טלפון")]
		override public string PhoneNumber { get; set; }
		[Required]
		[Display(Name = "שם פרטי")]
		public string FirstName { get; set; }
		[Required]
		[Display(Name = "שם משפחה")]
		public string LastName { get; set; }
		[Required]
		[Display(Name = "תעודת זהות")]
		public string TrueId { get; set; }
		[Display(Name = "קמפוס")]
		public string Campus { get; set; }
		[Display(Name = "שם מלא")]
		public string Name
		{
			get {
				return FirstName + " " + LastName;
			}
		}

		private ICollection<UnavailableInDate> _unavailableInDates;
		private ICollection<CourseTutor> _courseTutors;
		private ICollection<TutorRequest> _tutorRequests;

		public virtual ICollection<TutorRequest> TutorRequests
		{
			get { return _tutorRequests ?? (_tutorRequests = new HashSet<TutorRequest>()) ; }
			set { _tutorRequests = value; }
		}

		public virtual ICollection<CourseTutor> CourseTutors
		{
			get { return _courseTutors ?? (_courseTutors = new HashSet<CourseTutor>()); }
			set { _courseTutors = value; }
		}

		public virtual ICollection<UnavailableInDate> UnavailableInDates
		{
			get { return _unavailableInDates ?? (_unavailableInDates = new HashSet<UnavailableInDate>()); }
			set { _unavailableInDates = value; }
		}

		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
		{
			var userIdentity = await manager
				.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

			return userIdentity;
		}
	}

	public class ApplicationRole : IdentityRole
	{
		public ApplicationRole() : base() { }
		public ApplicationRole(string name) : base(name) { }
		[Display(Name = "תאור")]
		public string Description { get; set; }

	}

	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext()
			: base("IdentityDb2", throwIfV1Schema: false)
		{
		}

		static ApplicationDbContext()
		{
			Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
		}

		public static ApplicationDbContext Create()
		{
			return new ApplicationDbContext();
		}
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<CourseTutor>().ToTable("CourseTutor");
			modelBuilder.Entity<UnavailableInDate>().ToTable("UnavailableInDate");
			modelBuilder.Entity<Course>().ToTable("Course");
			modelBuilder.Entity<Category>().ToTable("Category");
			modelBuilder.Entity<Tigbur>().ToTable("Tigbur");
			modelBuilder.Entity<UnavailableInDate>().ToTable("UnavailableInDate").HasKey(ud => new { ud.ApplicationUserId, ud.StartDate, ud.EndDate });
		}

		public DbSet<UnavailableInDate> UnavailableInDates { get; set; }
		public DbSet<Course> Courses { get; set; }
		public DbSet<CourseTutor> CourseTutors { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Tigbur> Tigburs { get; set; }

	}
}