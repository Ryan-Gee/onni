using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace onni.Models
{
    public partial class Comments
    {
        [Key]
        [Column("CommentID")]
        public int CommentId { get; set; }
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }
        [Column("ProjectID")]
        public int ProjectId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CommentDate { get; set; }
        [Required]
        public string BodyContent { get; set; }

        [ForeignKey("ProjectId")]
        [InverseProperty("Comments")]
        public Projects Project { get; set; }
    }

}
