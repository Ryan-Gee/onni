using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace onni.Models
{
    public class ProjectCategories
    {
        public List<Projects> Projects { get; set; }
        public SelectList Categories { get; set; }
        public string ProjectCategory { get; set; }
        public string SearchString { get; set; }
    }
}
