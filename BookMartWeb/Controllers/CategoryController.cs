using BookMartWeb.Data;
using BookMartWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookMartWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db) {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _db.Categories.ToList();
            return View(objCategoryList);
        }
        public IActionResult Create()
        {
            return View();
            

        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            //test custom validation
            if (obj.DisplayOrder.ToString().Equals(obj.Name))
            {

                ModelState.AddModelError("", "The Display order cannot be same as Name");
            }

            if (ModelState.IsValid)
            {
                _db.Categories.Add(obj);
                _db.SaveChanges();
                TempData["success"] = "Created Successfully";
                return RedirectToAction("Index");
            }
            return View();


        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id==0)
            {
                return NotFound();
            }

           Category? category = _db.Categories.Find(id);

            if (category == null)
            {
                return BadRequest();
            }
            return View(category);


        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
    
            if (ModelState.IsValid)
            {
                _db.Categories.Update(obj);
                _db.SaveChanges();
                TempData["success"] = "Updated Successfully";
                return RedirectToAction("Index");
            }
            return View();


        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Category? category = _db.Categories.Find(id);

            if (category == null)
            {
                return BadRequest();
            }
            return View(category);


        }
        [HttpPost,ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Category? category = _db.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            _db.Categories.Remove(category);
            _db.SaveChanges(true);
            TempData["success"] = "Deleted Successfully";
            return RedirectToAction("Index");


        }
    }
}
