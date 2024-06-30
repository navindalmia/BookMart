using BookMart.DataAccess.Repository.IRepository;
using BookMart.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookMartWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.ProductRepository.GetAll().ToList();
            return View(objProductList);
        }
        public IActionResult Create()
        {
            return View();


        }
        [HttpPost]
        public IActionResult Create(Product obj)
        {
            ////test custom validation
            //if (obj.DisplayOrder.ToString().Equals(obj.Name))
            //{

            //    ModelState.AddModelError("", "The Display order cannot be same as Name");
            //}

            if (ModelState.IsValid)
            {
                _unitOfWork.ProductRepository.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Created Successfully";
                return RedirectToAction("Index");
            }
            return View();


        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Product? product = _unitOfWork.ProductRepository.Get(u => u.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            return View(product);


        }

        [HttpPost]
        public IActionResult Edit(Product obj)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.ProductRepository.Update(obj);
                _unitOfWork.Save();
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

            Product? product = _unitOfWork.ProductRepository.Get(u => u.Id == id);

            if (product == null)
            {
                return BadRequest();
            }
            return View(product);


        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Product? product = _unitOfWork.ProductRepository.Get(u => u.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            _unitOfWork.ProductRepository.Remove(product);
            _unitOfWork.Save();
            TempData["success"] = "Deleted Successfully";
            return RedirectToAction("Index");


        }
    }
}
