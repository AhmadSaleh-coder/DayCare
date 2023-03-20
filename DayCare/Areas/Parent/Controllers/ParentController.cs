using BulkyBook.Models;
using DayCare.DataAccess.Data;
using DayCare.Models;
using DayCare.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;

namespace DayCare.Areas.Parent.Controllers
{
    [Area("Parent")]
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

        public IActionResult Create(string? token)
        {   if (token == null)
            {
                return NotFound();
            }

            ParentLink parentLink = _db.ParentLinks.FirstOrDefault(u => u.RandomLink == token);

            if (parentLink == null || parentLink.Status == "InValid")
            {
                return NotFound();
            }

            ParentVM parentVm = new()
            {
                Token = token
                
            };


            return View(parentVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ParentVM obj)
        {
            var parent = CreateUser();
            await _userStore.SetUserNameAsync(parent, obj.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(parent, obj.Email, CancellationToken.None);
            parent.FullName = obj.Name;
            parent.PhoneNumber = obj.PhoneNumber;
            var result = await _userManager.CreateAsync(parent, obj.Password);
            if(result.Succeeded)
            {
                await _userManager.AddToRoleAsync(parent, "Parent");
                ParentLink parentLink = _db.ParentLinks.FirstOrDefaultAsync(u => u.RandomLink == obj.Token).GetAwaiter().GetResult();
                parentLink.Status = "InValid";
                

                await _db.Users_Schools.AddAsync(new User_School()
                {
                    UserId = parent.Id,
                    SchoolId = parentLink.SchoolId,
                    Role = "Parent"
                }) ;
                _db.SaveChangesAsync();
            }

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
