using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DayCare.Models
{
    public class ParentLink
    {
        public int Id { get; set; }
        public string RandomLink { get; set; }
        public string Status { get; set; } = "Valid";
        [EmailAddress]
        public string? SendTo { get; set; } = null;
        public int SchoolId { get; set; }

        [ForeignKey("SchoolId")]
        public School School { get; set; }
    }
}
