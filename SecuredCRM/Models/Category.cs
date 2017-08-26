using System.ComponentModel.DataAnnotations;

namespace SecuredCRM.Models
{
    public class Category
	{
		[Key]
		public int CategoryId { get; set; }
		[Display(Name = "קטגוריה")]
		[Required]
		public string Name { get; set; }
		[Display(Name = "תאור")]
		public string Description { get; set; }
	}
}