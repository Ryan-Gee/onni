using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace onni.Models
{
    public partial class Comments
    {
        [Key]
        [Column("CommentID")]
		[DisplayName("ID")]
		public int CommentId { get; set; }
        [Required]
        [StringLength(50)]
		[DisplayName("User")]
		public string UserName { get; set; }
        [Column("ProjectID")]
		[DisplayName("Project")]
		public int ProjectId { get; set; }
        [Column(TypeName = "datetime")]
		[DisplayName("Date")]
		public DateTime CommentDate { get; set; }
        [Required]
		[DisplayName("Comment")]
		public string BodyContent { get; set; }

        [ForeignKey("ProjectId")]
        [InverseProperty("Comments")]
        public Projects Project { get; set; }
    }

}
