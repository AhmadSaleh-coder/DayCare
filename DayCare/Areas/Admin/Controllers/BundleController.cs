using DayCare.DataAccess.Data;
using DayCare.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DayCare.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class BundleController : Controller
    {
        private readonly ApplicationDbContext _db;
        public BundleController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<Bundle> objBundlesList = _db.Bundles.Where(u => u.IsDeleted == null).ToList();
            return View(objBundlesList);
        }

        //Get
        public IActionResult Create()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Bundle obj)
        {
            _db.Bundles.Add(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");   
        }

        //Get
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var bundleFromDb = _db.Bundles.FirstOrDefault(u => u.Id == id);
            if (bundleFromDb == null)
            {
                return NotFound();
            }

            return View(bundleFromDb);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Bundle obj)
        {
            _db.Bundles.Update(obj);
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
            var bundleFromDb = _db.Bundles.FirstOrDefault(u => u.Id == id);
            if (bundleFromDb == null)
            {
                return NotFound();
            }

            return View(bundleFromDb);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _db.Bundles.FirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            obj.IsDeleted = DateTime.Now.ToLocalTime();
            _db.Bundles.Update(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");

        }
    }
}
