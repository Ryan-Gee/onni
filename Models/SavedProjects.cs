using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace onni.Models
{
    public partial class SavedProjects
    {
        [Key]
        [Column("SavedID")]
		[DisplayName("ID")]
		public int SavedId { get; set; }
        [Required]
        [StringLength(50)]
		[DisplayName("User")]
		public string UserName { get; set; }
        [Column("ProjectID")]
		[DisplayName("Project")]
		public int ProjectId { get; set; }
        [Column(TypeName = "datetime")]
		[DisplayName("Date")]
		public DateTime SavedDate { get; set; }

        [ForeignKey("ProjectId")]
        [InverseProperty("SavedProjects")]
        public Projects Project { get; set; }
    }
}
