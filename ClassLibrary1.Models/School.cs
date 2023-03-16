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
    public class School
    {
        [Key]
        public int Id { get; set; }

        public string SchoolName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime? IsDeleted { get; set; } = null;




    }
}
