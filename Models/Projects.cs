using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
		[DisplayName("Project ID")]
		public int ProjectId { get; set; }
        [Required]
        [StringLength(50)]
		[DisplayName("Title")]
		public string ProjectName { get; set; }
        [Required]
        [StringLength(50)]
		[DisplayName("Creator")]
		public string UserName { get; set; }
        [Column(TypeName = "datetime")]
		[DisplayName("Creation Date")]
		public DateTime CreatedDate { get; set; }
        [Required]
		[DisplayName("Description")]
		public string BodyContent { get; set; }
		public string Files { get; set; }
        public string Images { get; set; }
		[DisplayName("Views")]
		public int ViewCounts { get; set; }
		[DisplayName("Likes")]
		public int LikeCounts { get; set; }
        [Column("StatusID")]
		[DisplayName("Status")]
		public int StatusId { get; set; }
		[Column("ParentProjectID")]
		[DisplayName("Parent")]
		public int? ParentProjectId { get; set; }
        public string Tags { get; set; }
        [Column("CategoryID")]
		[DisplayName("Cateogry")]
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

	public class  ProjectUpload
	{
		public int ProjectId { get; set; }
		[DisplayName("Title")]
		public string ProjectName { get; set; }
		public string UserName { get; set; }
		public DateTime CreatedDate { get; set; }
		[DisplayName("Description")]
		public string BodyContent { get; set; }
		[DisplayName("Files")]
		public string Files { get; set; }
		[DisplayName("Images")]
		public string Images { get; set; }
		public int ViewCounts { get; set; }
		public int LikeCounts { get; set; }
		[DisplayName("Status")]
		public int StatusId { get; set; }
		[DisplayName("Parent Project")]
		public int? ParentProjectId { get; set; }
		public string Tags { get; set; }
		[DisplayName("Category")]
		public int? CategoryId { get; set; }
	}
    public class ProjectsInYear
    {
        public int Mouth { get; set; }
        public int Count { get; set; }

    }
	public class ActivityInYear
	{
		public int Month { get; set; }
		public int yearProjects { get; set; }
		public int yearComments { get; set; }
	}
    public class CommentsInYear
    {
        public int Mouth { get; set; }
        public int Count { get; set; }
    }
	public class BestOfYear
	{
		public Projects views { get; set; }
		public Projects likes { get; set; }
	}

    
}
