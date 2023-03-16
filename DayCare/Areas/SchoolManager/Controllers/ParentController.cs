using BulkyBook.Models;
using DayCare.DataAccess.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

        public ParentController(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            RoleManager<IdentityRole> roleManger,
            ApplicationDbContext db
            )


        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _roleManager = roleManger;
            _db = db;
        }
     
        public IActionResult Index()
        {
            return View();
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
