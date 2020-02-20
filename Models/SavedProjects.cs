using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace onni.Models
{
    public partial class SavedProjects
    {
        [Key]
        [Column("SavedID")]
        public int SavedId { get; set; }
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }
        [Column("ProjectID")]
        public int ProjectId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime SavedDate { get; set; }

        [ForeignKey("ProjectId")]
        [InverseProperty("SavedProjects")]
        public Projects Project { get; set; }
    }
}
