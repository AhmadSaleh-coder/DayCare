using BulkyBook.Models;
using DayCare.DataAccess.Data;
using DayCare.Models;
using DayCare.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Remoting;
using System.Security.Claims;

namespace DayCare.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class SchoolController : Controller
    {
       
     
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public SchoolController(
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
            IEnumerable<School> objSchoolsList = _db.Schools.Where(u=>u.IsDeleted == null) .ToList();
            return View(objSchoolsList);
        }

        //Get
        public IActionResult Create()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SchoolVM obj)
        {
           
            var user = CreateUser();

            await _userStore.SetUserNameAsync(user, obj.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, obj.Email, CancellationToken.None);
            user.FullName = obj.Name;

            var result = await _userManager.CreateAsync(user, obj.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "SchoolManager");
              /*  obj.School.OwnerId= user.Id;*/
                _db.Schools.Add(obj.School);
                _db.SaveChanges();
                await _userManager.AddClaimAsync(user, new Claim("schoolId", obj.School.Id.ToString()));

                return RedirectToAction("Index");
            }
                return View();
        }

        //Get
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var schoolFromDb = _db.Schools.FirstOrDefault(u=>u.Id == id);
            if(schoolFromDb == null)
            {
                return NotFound();
            }
            
            return View(schoolFromDb);
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(School obj)
        {
          _db.Schools.Update(obj);
           _db.SaveChanges();
            return RedirectToAction("Index");
        }

        //GET
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var schoolFromDb = _db.Schools.FirstOrDefault(u => u.Id == id);
            if (schoolFromDb == null)
            {
                return NotFound();
            }

            return View(schoolFromDb);
        }

        //POST
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _db.Schools.FirstOrDefault(u => u.Id==id);   
            if (obj == null)
            {
                return NotFound();
            }
            obj.IsDeleted= DateTime.Now.ToLocalTime();
             _db.Schools.Update(obj);
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
