using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace onni.Models
{
    public partial class Projects
    {
        public Projects()
        {
            Comments = new HashSet<Comments>();
            InverseParentProject = new HashSet<Projects>();
            SavedProjects = new HashSet<SavedProjects>();
        }

        [Key]
        [Column("ProjectID")]
        public int ProjectId { get; set; }
        [Required]
        [StringLength(50)]
        public string ProjectName { get; set; }
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDate { get; set; }
        [Required]
        public string BodyContent { get; set; }
        public string Files { get; set; }
        public string Imeages { get; set; }
        public int ViewCounts { get; set; }
        public int LikeCounts { get; set; }
        [Column("StatusID")]
        public int StatusId { get; set; }
        [Column("ParentProjectID")]
        public int? ParentProjectId { get; set; }
        public string Tags { get; set; }
        [Column("CategoryID")]
        public int? CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        [InverseProperty("Projects")]
        public Categories Category { get; set; }
        [ForeignKey("ParentProjectId")]
        [InverseProperty("InverseParentProject")]
        public Projects ParentProject { get; set; }
        [ForeignKey("StatusId")]
        [InverseProperty("Projects")]
        public Status Status { get; set; }
        [InverseProperty("Project")]
        public ICollection<Comments> Comments { get; set; }
        [InverseProperty("ParentProject")]
        public ICollection<Projects> InverseParentProject { get; set; }
        [InverseProperty("Project")]
        public ICollection<SavedProjects> SavedProjects { get; set; }
    }
}
