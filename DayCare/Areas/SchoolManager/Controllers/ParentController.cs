using BulkyBook.Models;
using DayCare.DataAccess.Data;
using DayCare.Models;
using DayCare.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg;
using System.Security.Claims;

namespace DayCare.Areas.SchoolManager.Controllers
{
    [Area("SchoolManager")]
    public class ParentController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;

        public ParentController(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            RoleManager<IdentityRole> roleManger,
            ApplicationDbContext db,
            IConfiguration configuration

            )


        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _roleManager = roleManger;
            _db = db;
            _configuration = configuration;
        }
     
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {

           
            return View();
        }
        [HttpPost]
        public IActionResult Create(ParentLink parentLink)
        {   
           string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int schoolId = _db.Users_Schools.FirstOrDefault(u => u.UserId == UserId).SchoolId;
            parentLink.SchoolId = schoolId;
            parentLink.RandomLink= Guid.NewGuid().ToString();
            _db.Add(parentLink);
            _db.SaveChanges();

            var emailSender = new EmailSender(_configuration);
            emailSender.SendEmailAsync(parentLink.SendTo, "ab", $"<a href='https://localhost:44365/Parent/Parent/Create?token={parentLink.RandomLink}'> Register </a>");

            return View(nameof(Index));
        }













        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
