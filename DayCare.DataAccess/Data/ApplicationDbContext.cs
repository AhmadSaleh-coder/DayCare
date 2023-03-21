using BulkyBook.Models;
using DayCare.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DayCare.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext <ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<Bundle> Bundles { get; set; }
        public DbSet<User_School> Users_Schools { get; set; }
        public DbSet<RegisterLink> RegisterLinks { get; set; }

    }




}
