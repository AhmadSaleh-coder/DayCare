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
    public class Bundle
    {
        [Required]
        public int Id { get; set; }

        public string Type { get; set; }
        public int Price { get; set; }
        public string Details { get; set; }
        public int TimeSpan_days { get; set; }
        public DateTime? IsDeleted { get; set; } = null;


        /*[ForeignKey("OwnerId")]
        public ApplicationUser ApplicationUser { get; set; }*/


    }
}
