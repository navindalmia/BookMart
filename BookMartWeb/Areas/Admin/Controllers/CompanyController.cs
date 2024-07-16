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
    //[Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<CompanyController> _logger;
        public CompanyController(IUnitOfWork unitOfWork, ILogger<CompanyController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
           
        }
        public IActionResult Index()
        {
            List<Company> objCompanyList = _unitOfWork.CompanyRepository.GetAll().ToList();
            return View(objCompanyList);
        }
        public IActionResult Upsert(int? id)
        {
            _logger.LogDebug("Create invoked");
            
            if(id== null || id == 0)
            {
                //Create
                return View(new Company());
            }
            else
            {
                //Update
                Company company = _unitOfWork.CompanyRepository.Get(u=> u.Id == id);
                return View(company);

            }
            //ViewBag.CategoryList = CategoryList;
         


        }
        [HttpPost]
        public IActionResult Upsert(Company CompanyObj)
        {
            ////test custom validation
            //if (obj.DisplayOrder.ToString().Equals(obj.Name))
            //{

            //    ModelState.AddModelError("", "The Display order cannot be same as Name");
            //}

            if (ModelState.IsValid)
            {
                
               
                if (CompanyObj.Id == 0)
                {
                    _unitOfWork.CompanyRepository.Add(CompanyObj);
                }
                else
                {
                    _unitOfWork.CompanyRepository.Update(CompanyObj);
                }
                
                _unitOfWork.Save();
                TempData["success"] = "Created Successfully";
                return RedirectToAction("Index");
            }
            else
            {
               
                //ViewBag.CategoryList = CategoryList;
                return View(CompanyObj);
                
            }

        }
       
        #region API Calls

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> objCompanyList = _unitOfWork.CompanyRepository.GetAll().ToList();
            return Json(new { data = objCompanyList });

        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            Company? companyToBeDeleted = _unitOfWork.CompanyRepository.Get(u => u.Id == id);
            if(companyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            
            _unitOfWork.CompanyRepository.Remove(companyToBeDeleted);
            _unitOfWork.Save();
           
            return Json(new { success=true , message ="Delete Successful" });

        }
        #endregion
    }
}
