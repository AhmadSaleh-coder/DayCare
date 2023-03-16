using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DayCare.Models
{
    public class User_School
    {
        [Key]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }

       
        public int SchoolId { get; set; }

        [ForeignKey("SchoolId")]
        public School School { get; set; }

        public string Role { get; set; }

    }
}
