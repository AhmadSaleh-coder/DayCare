using BulkyBook.Models;
using DayCare.DataAccess.Data;
using DayCare.Models;
using DayCare.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using System.Security.Claims;

namespace DayCare.Areas.SchoolManager.Controllers
{
    [Area("SchoolManager")]
    [Authorize(Roles ="SchoolManager")]
    public class TeacherController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public TeacherController(
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
            int schoolId = Convert.ToInt32(User.FindFirstValue("schoolId"));
            IEnumerable<User_School> objTeacherssList = _db.Users_Schools.Include("ApplicationUser").Include("School").Where(u=>u.Role == "Teacher" && u.SchoolId== schoolId).Where(u=>u.ApplicationUser.isDeleted == null).ToList();
            return View(objTeacherssList);
        }

        //Get
        public IActionResult Create()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeacherVM obj)
        {

            var teacher = CreateUser();

            await _userStore.SetUserNameAsync(teacher, obj.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(teacher, obj.Email, CancellationToken.None);
            teacher.FullName = obj.Name;
            var result = await _userManager.CreateAsync(teacher, obj.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(teacher, "Teacher");
                int schoolId = Convert.ToInt32(User.FindFirstValue("schoolId"));

                await _db.Users_Schools.AddAsync(new User_School() {
                   UserId= teacher.Id,
                   SchoolId = schoolId,
                   Role = "Teacher"
                 });
                
                await _db.SaveChangesAsync();
                await _userManager.AddClaimAsync(teacher, new Claim("schoolId", schoolId.ToString()));

                return RedirectToAction("Index");
            }
            return View();
        }

        //Get
        public IActionResult Edit(string? id)
        {
            
            if (id == null)
            {
                return NotFound();
            }
            var teacherFromDb = _db.ApplicationUsers.FirstOrDefault(u=> u.Id == id);

            TeacherVM teacherVM = new()
            {
                Email = teacherFromDb.Email,
                Name = teacherFromDb.FullName,
                teacherId = teacherFromDb.Id
            };

            if (teacherFromDb == null)
            {
                return NotFound();
            }

            return View(teacherVM);
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Edit(TeacherVM obj)
        {
            ApplicationUser teacherFromDb =  _userManager.FindByIdAsync(obj.teacherId).GetAwaiter().GetResult();
            await _userStore.SetUserNameAsync(teacherFromDb, obj.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(teacherFromDb, obj.Email, CancellationToken.None);
           
            teacherFromDb.FullName = obj.Name;
            await  _userManager.UpdateAsync(teacherFromDb);
            return RedirectToAction("Index");
        }

        //GET
        public IActionResult Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var teacherFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (teacherFromDb == null)
            {
                return NotFound();
            }

            TeacherVM teacherVM = new()
            {
                Name = teacherFromDb.FullName,
                Email = teacherFromDb.Email,
            };

            return View(teacherVM);
        }

        //POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(string? id)
        {
            var obj = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
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
