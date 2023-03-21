using BulkyBook.Models;
using DayCare.DataAccess.Data;
using DayCare.Models;
using DayCare.Models.ViewModels;
using DayCare.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg;
using System.ComponentModel;
using System.Security.Claims;

namespace DayCare.Areas.SchoolManager.Controllers
{
    [Area("SchoolManager")]
    [Authorize(Roles ="SchoolManager")]
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
            int schoolId = Convert.ToInt32(User.FindFirstValue("schoolId"));
            IEnumerable<User_School> objParentsList = _db.Users_Schools.Include("ApplicationUser").Include("School").Where(u => u.Role == "Parent" && u.SchoolId == schoolId).Where(u => u.ApplicationUser.isDeleted == null).ToList();

            return View(objParentsList);
        }

        public IActionResult Create()
        {

           
            return View();
        }
        [HttpPost]
        public IActionResult Create(RegisterLink parentLink)
        {   
           int schoolId =Convert.ToInt32(User.FindFirstValue("schoolId"));
         
            parentLink.SchoolId = schoolId;
            parentLink.RandomLink= Guid.NewGuid().ToString();
            parentLink.Role = "Parent";
            _db.Add(parentLink);
            _db.SaveChanges();

            var emailSender = new EmailSender(_configuration);
            emailSender.SendEmailAsync(parentLink.SendTo, "ab", $"<a href='https://localhost:44365/Parent/Parent/Create?token={parentLink.RandomLink}'> Register </a>");

            return View("EmailCheck");
        }

        public IActionResult Edit(string? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            var parentFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);

            ParentVM parentVM = new()
            {
                Email = parentFromDb.Email,
                Name = parentFromDb.FullName,
                PhoneNumber = parentFromDb.PhoneNumber,
                parentId = parentFromDb.Id
            };

            if (parentFromDb == null)
            {
                return NotFound();
            }

            return View(parentVM);
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ParentVM obj)
        {
            ApplicationUser parentFromDb = _userManager.FindByIdAsync(obj.parentId).GetAwaiter().GetResult();
            await _userStore.SetUserNameAsync(parentFromDb, obj.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(parentFromDb, obj.Email, CancellationToken.None);

            parentFromDb.FullName = obj.Name;
            parentFromDb.PhoneNumber = obj.PhoneNumber;

            await _userManager.UpdateAsync(parentFromDb);
            return RedirectToAction("Index");
        }

        //Get
        public IActionResult Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var parentFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (parentFromDb == null)
            {
                return NotFound();
            }

            ParentVM parentVM = new()
            {
                Name = parentFromDb.FullName,
                Email = parentFromDb.Email,
            };

            return View(parentVM);
        }

        //POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(string? id) {
            var obj = _db.ApplicationUsers.FirstOrDefault(u => u.Id==id);
            if (obj == null)
            {
                return NotFound();
            }
            obj.isDeleted = DateTime.Now.ToLocalTime();
            _db.ApplicationUsers.Update(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
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
