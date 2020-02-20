using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace onni.Models
{
    public partial class Status
    {
        public Status()
        {
            Projects = new HashSet<Projects>();
        }

        [Column("StatusID")]
        public int StatusId { get; set; }
        [Required]
        [StringLength(50)]
        public string StatusName { get; set; }

        [InverseProperty("Status")]
        public ICollection<Projects> Projects { get; set; }
    }
}
