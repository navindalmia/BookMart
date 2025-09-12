using BookMart.DataAccess.Repository.IRepository;
using BookMart.Models;
using BookMart.Models.ViewModels;
using BookMart.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookMartWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<ProductController> _logger;
        public ProductController(IUnitOfWork unitOfWork, ILogger<ProductController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.ProductRepository.GetAll(includeProperties: "Category").ToList();
            return View(objProductList);
        }
        public IActionResult Upsert(int? id)
        {
            _logger.LogDebug("Create invoked");
            ProductVM productVM = new ProductVM()
            {
                CategoryList = _unitOfWork.CategoryRepository
                .GetAll().Select(
                u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }
                )
                , Product = new Product()
            };
            if(id== null || id == 0)
            {
                //Create
                return View(productVM);
            }
            else
            {
                //Update
                productVM.Product = _unitOfWork.ProductRepository.Get(u=> u.Id == id,includeProperties:"ProductImages");
                return View(productVM);

            }
            //ViewBag.CategoryList = CategoryList;
         


        }
        [HttpPost]
        public IActionResult Upsert(ProductVM obj, List<IFormFile>? files)
        {
            ////test custom validation
            //if (obj.DisplayOrder.ToString().Equals(obj.Name))
            //{

            //    ModelState.AddModelError("", "The Display order cannot be same as Name");
            //}

            if (ModelState.IsValid)
            {
                if (obj.Product.Id == 0)
                {
                    _unitOfWork.ProductRepository.Add(obj.Product);
                }
                else
                {
                    _unitOfWork.ProductRepository.Update(obj.Product);
                }

                _unitOfWork.Save();
           

                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\product-" + obj.Product.Id;
                            
                         //string Path.Combine(wwwRootPath, @"images\product");

                        string finalPath = Path.Combine(wwwRootPath, productPath);
                        if (!Directory.Exists(finalPath))
                        {
                            Directory.CreateDirectory(finalPath);
                        }
                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }
                        ProductImage productImage = new()
                        {
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductId = obj.Product.Id
                        };
                        if(obj.Product.ProductImages ==null)
                        {
                            obj.Product.ProductImages = new List<ProductImage>();   
                        }
                        obj.Product.ProductImages.Add(productImage);



                    }

                    _unitOfWork.ProductRepository.Update(obj.Product);
                    _unitOfWork.Save();
                    /*   if(!string.IsNullOrEmpty(obj.Product.ImageUrl))
                       {
                           var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
                           if (System.IO.File.Exists(oldImagePath))
                           {
                               System.IO.File.Delete(oldImagePath);
                           }
                       }*/

                    /* using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                     {
                         file.CopyTo(fileStream);
                     }
                     obj.Product.ImageUrl = @"\images\product\" + fileName;*/
                }
               
                TempData["success"] = "Created Successfully";
                return RedirectToAction("Index");
            }
            else
            {
                ProductVM productVM = new ProductVM()
                {
                    CategoryList = _unitOfWork.CategoryRepository
               .GetAll().Select(
               u => new SelectListItem
               {
                   Text = u.Name,
                   Value = u.Id.ToString()
               }
               )
               ,
                    Product = new Product()
                };
                //ViewBag.CategoryList = CategoryList;
                return View(productVM);
                
            }

        }

        public IActionResult DeleteImage(int imageId)
        {
            ProductImage image = _unitOfWork.ProductImageRepository.Get(u=>u.Id==imageId);

            DeleteImageUtil(image);

            //if (image != null && image.ImageUrl != null)
            //{
            //    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, image.ImageUrl.TrimStart('\\'));
            //    if (System.IO.File.Exists(oldImagePath))
            //    {
            //        System.IO.File.Delete(oldImagePath);
            //    }
            //    _unitOfWork.ProductImageRepository.Remove(image);
            //    _unitOfWork.Save();
            //    TempData["success"] = "Deleted Successfully";
            //}
                return RedirectToAction(nameof(Upsert),new {id=image.ProductId});
        }

        public void DeleteImageUtil(ProductImage image)
        {
            if (image != null && image.ImageUrl != null)
            {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, image.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
                _unitOfWork.ProductImageRepository.Remove(image);
                _unitOfWork.Save();
                TempData["success"] = "Deleted Successfully";
            }
        }


        /*public IActionResult Edit(int? id)
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
        */
        /*public IActionResult Delete(int? id)
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
        */
        #region API Calls

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.ProductRepository.GetAll(includeProperties:"Category").ToList();
            return Json(new { data = objProductList });

        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            Product? productToBeDeleted = _unitOfWork.ProductRepository.Get(u => u.Id == id);
            if(productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            DeleteProductImageDirectory(productToBeDeleted.Id);
           /* var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }*/
            _unitOfWork.ProductRepository.Remove(productToBeDeleted);
            _unitOfWork.Save();
           
            return Json(new { success=true , message ="Delete Successful" });

        }
        void DeleteProductImageDirectory(int productId)
        {
            string productPath = @"images\products\product-" + productId;
            var directoryToBeDeleted = Path.Combine(_webHostEnvironment.WebRootPath, productPath);
            if (Directory.Exists(directoryToBeDeleted))
            {
                Directory.Delete(directoryToBeDeleted, recursive: true);
            }
        }



        #endregion
    }
}
