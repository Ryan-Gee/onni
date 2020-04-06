using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace onni.Models
{
    public partial class Categories
    {
        public Categories()
        {
            Projects = new HashSet<Projects>();
        }

        [Column("CategoriesID")]
		[DisplayName("ID")]
		public int CategoriesId { get; set; }
        [Required]
        [StringLength(50)]
		[DisplayName("Name")]
		public string CategoriesName { get; set; }

        [InverseProperty("Category")]
        public ICollection<Projects> Projects { get; set; }
    }
}
